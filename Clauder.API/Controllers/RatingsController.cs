using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Clauder.Data;
using Clauder.Domain.Entities;

namespace Clauder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RatingsController : ControllerBase
{
    private readonly ClauerDbContext _context;

    public RatingsController(ClauerDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<Rating>> RateRecipe([FromBody] RateRecipeRequest request)
    {
        if (request.Score < 1 || request.Score > 5)
            return BadRequest("Score must be between 1 and 5");

        var recipe = await _context.Recipes.FindAsync(request.RecipeId);
        if (recipe == null)
            return NotFound("Recipe not found");

        var existingRating = await _context.Ratings
            .FirstOrDefaultAsync(r => r.RecipeId == request.RecipeId && r.UserId == request.UserId);

        if (existingRating != null)
        {
            existingRating.Score = request.Score;
            existingRating.Comment = request.Comment;
            existingRating.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existingRating;
        }

        var rating = new Rating
        {
            RecipeId = request.RecipeId,
            UserId = request.UserId,
            Score = request.Score,
            Comment = request.Comment
        };

        _context.Ratings.Add(rating);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRating), new { id = rating.Id }, rating);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Rating>> GetRating(int id)
    {
        var rating = await _context.Ratings
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rating == null)
            return NotFound();

        return rating;
    }

    [HttpGet("recipe/{recipeId}")]
    public async Task<ActionResult<IEnumerable<Rating>>> GetRecipeRatings(int recipeId)
    {
        var ratings = await _context.Ratings
            .Where(r => r.RecipeId == recipeId)
            .Include(r => r.User)
            .ToListAsync();

        return ratings;
    }

    [HttpGet("leaderboard")]
    public async Task<ActionResult<IEnumerable<RecipeLeaderboardItem>>> GetLeaderboard(
        [FromQuery] int limit = 10)
    {
        var leaderboard = await _context.Recipes
            .Where(r => r.IsPublished)
            .Include(r => r.Ratings)
            .Include(r => r.CreatedByUser)
            .Select(r => new RecipeLeaderboardItem
            {
                RecipeId = r.Id,
                RecipeTitle = r.Title,
                CreatedBy = r.CreatedByUser!.DisplayName ?? r.CreatedByUser.Username,
                AverageRating = r.Ratings.Any() ? r.Ratings.Average(rt => rt.Score) : 0,
                TotalRatings = r.Ratings.Count,
                PublishedAt = r.PublishedAt
            })
            .OrderByDescending(item => item.AverageRating)
            .ThenByDescending(item => item.TotalRatings)
            .Take(limit)
            .ToListAsync();

        return leaderboard;
    }
}

public class RateRecipeRequest
{
    public int RecipeId { get; set; }
    public int UserId { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
}

public class RecipeLeaderboardItem
{
    public int RecipeId { get; set; }
    public required string RecipeTitle { get; set; }
    public required string CreatedBy { get; set; }
    public double AverageRating { get; set; }
    public int TotalRatings { get; set; }
    public DateTime? PublishedAt { get; set; }
}
