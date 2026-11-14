using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Clauder.Data;
using Clauder.Domain.Entities;

namespace Clauder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly ClauerDbContext _context;
    private readonly IWebHostEnvironment _env;

    public RecipesController(ClauerDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    private int? RequesterUserId =>
        int.TryParse(Request.Headers["X-User-Id"], out var userId) ? userId : null;

    private bool IsOwner(Recipe recipe) => RequesterUserId == recipe.CreatedByUserId;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes(
        [FromQuery] string? nationality = null,
        [FromQuery] string? recipeType = null,
        [FromQuery] int? minRating = null)
    {
        var query = _context.Recipes
            .Where(r => r.IsPublished)
            .Include(r => r.CreatedByUser)
            .Include(r => r.Ratings)
            .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.MediaFiles)
            .AsQueryable();

        if (!string.IsNullOrEmpty(nationality))
            query = query.Where(r => r.Nationality == nationality);

        if (!string.IsNullOrEmpty(recipeType))
            query = query.Where(r => r.RecipeType == recipeType);

        return await query.ToListAsync();
    }

    [HttpGet("mine")]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetMyRecipes()
    {
        if (RequesterUserId is not int userId)
            return Unauthorized();

        return await _context.Recipes
            .Where(r => r.CreatedByUserId == userId)
            .Include(r => r.Ratings)
            .Include(r => r.MediaFiles)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Recipe>> GetRecipe(int id)
    {
        var recipe = await _context.Recipes
            .Include(r => r.CreatedByUser)
            .Include(r => r.Ratings)
            .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.MediaFiles)
            .Include(r => r.Reviews)
            .ThenInclude(rv => rv.MediaFiles)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null)
            return NotFound();

        if (!recipe.IsPublished && !IsOwner(recipe))
            return NotFound();

        return recipe;
    }

    [HttpPost]
    public async Task<ActionResult<Recipe>> CreateRecipe([FromBody] CreateRecipeRequest request)
    {
        if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Description))
            return BadRequest("Title and Description are required");

        var recipe = new Recipe
        {
            Title = request.Title,
            Description = request.Description,
            Instructions = request.Instructions,
            ServingsCount = request.ServingsCount,
            PrepTimeMinutes = request.PrepTimeMinutes,
            CookTimeMinutes = request.CookTimeMinutes,
            Nationality = request.Nationality ?? "Italiana",
            RecipeType = request.RecipeType ?? "Primo",
            CreatedByUserId = request.CreatedByUserId
        };

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Recipe>> UpdateRecipe(int id, [FromBody] UpdateRecipeRequest request)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe == null)
            return NotFound();

        if (!IsOwner(recipe))
            return StatusCode(StatusCodes.Status403Forbidden);

        recipe.Title = request.Title ?? recipe.Title;
        recipe.Description = request.Description ?? recipe.Description;
        recipe.Instructions = request.Instructions ?? recipe.Instructions;
        recipe.Nationality = request.Nationality ?? recipe.Nationality;
        recipe.RecipeType = request.RecipeType ?? recipe.RecipeType;
        recipe.ServingsCount = request.ServingsCount ?? recipe.ServingsCount;
        recipe.PrepTimeMinutes = request.PrepTimeMinutes ?? recipe.PrepTimeMinutes;
        recipe.CookTimeMinutes = request.CookTimeMinutes ?? recipe.CookTimeMinutes;
        recipe.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return recipe;
    }

    [HttpPost("{id}/publish")]
    public async Task<ActionResult<Recipe>> PublishRecipe(int id)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe == null)
            return NotFound();

        if (!IsOwner(recipe))
            return StatusCode(StatusCodes.Status403Forbidden);

        recipe.IsPublished = true;
        recipe.PublishedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return recipe;
    }

    [HttpPost("{id}/unpublish")]
    public async Task<ActionResult<Recipe>> UnpublishRecipe(int id)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe == null)
            return NotFound();

        if (!IsOwner(recipe))
            return StatusCode(StatusCodes.Status403Forbidden);

        recipe.IsPublished = false;
        recipe.PublishedAt = null;

        await _context.SaveChangesAsync();
        return recipe;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipe(int id)
    {
        var recipe = await _context.Recipes
            .Include(r => r.MediaFiles)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (recipe == null)
            return NotFound();

        if (!IsOwner(recipe))
            return StatusCode(StatusCodes.Status403Forbidden);

        var uploadsFolder = Path.Combine(_env.ContentRootPath, "uploads");
        foreach (var mediaFile in recipe.MediaFiles)
        {
            var filePath = Path.Combine(uploadsFolder, mediaFile.FileName);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }

        _context.Recipes.Remove(recipe);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class CreateRecipeRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public string? Instructions { get; set; }
    public int ServingsCount { get; set; } = 4;
    public int PrepTimeMinutes { get; set; }
    public int CookTimeMinutes { get; set; }
    public string? Nationality { get; set; }
    public string? RecipeType { get; set; }
    public int CreatedByUserId { get; set; }
}

public class UpdateRecipeRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public string? Nationality { get; set; }
    public string? RecipeType { get; set; }
    public int? ServingsCount { get; set; }
    public int? PrepTimeMinutes { get; set; }
    public int? CookTimeMinutes { get; set; }
}
