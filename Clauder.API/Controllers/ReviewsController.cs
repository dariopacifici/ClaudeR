using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Clauder.Data;
using Clauder.Domain.Entities;

namespace Clauder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly ClauerDbContext _context;

    public ReviewsController(ClauerDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<Review>> CreateReview([FromBody] CreateReviewRequest request)
    {
        var recipe = await _context.Recipes.FindAsync(request.RecipeId);
        if (recipe == null)
            return NotFound("Recipe not found");

        if (!recipe.IsPublished)
            return BadRequest("Can only add reviews to published recipes");

        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null)
            return NotFound("User not found");

        var review = new Review
        {
            RecipeId = request.RecipeId,
            UserId = request.UserId,
            Title = request.Title,
            Content = request.Content,
            Category = request.Category,
            IsPublished = request.IsPublished
        };

        if (request.IsPublished)
            review.PublishedAt = DateTime.UtcNow;

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Review>> GetReview(int id)
    {
        var review = await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.MediaFiles)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (review == null)
            return NotFound();

        return review;
    }

    [HttpGet("recipe/{recipeId}")]
    public async Task<ActionResult<IEnumerable<Review>>> GetRecipeReviews(int recipeId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.RecipeId == recipeId && r.IsPublished)
            .Include(r => r.User)
            .Include(r => r.MediaFiles)
            .ToListAsync();

        return reviews;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Review>> UpdateReview(int id, [FromBody] UpdateReviewRequest request)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            return NotFound();

        review.Title = request.Title ?? review.Title;
        review.Content = request.Content ?? review.Content;
        review.Category = request.Category ?? review.Category;
        review.UpdatedAt = DateTime.UtcNow;

        if (request.IsPublished == true && !review.IsPublished)
        {
            review.IsPublished = true;
            review.PublishedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return review;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            return NotFound();

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class CreateReviewRequest
{
    public int RecipeId { get; set; }
    public int UserId { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string? Category { get; set; }
    public bool IsPublished { get; set; } = false;
}

public class UpdateReviewRequest
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Category { get; set; }
    public bool? IsPublished { get; set; }
}
