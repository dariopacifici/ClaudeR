using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Clauder.Data;
using Clauder.Domain.Entities;

namespace Clauder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngredientsController : ControllerBase
{
    private readonly ClauerDbContext _context;

    public IngredientsController(ClauerDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ingredient>>> GetIngredients()
    {
        return await _context.Ingredients.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Ingredient>> GetIngredient(int id)
    {
        var ingredient = await _context.Ingredients.FindAsync(id);
        if (ingredient == null)
            return NotFound();
        return ingredient;
    }

    [HttpPost]
    public async Task<ActionResult<Ingredient>> CreateIngredient([FromBody] CreateIngredientRequest request)
    {
        if (string.IsNullOrEmpty(request.Name))
            return BadRequest("Name is required");

        var existingIngredient = await _context.Ingredients
            .FirstOrDefaultAsync(i => i.Name.ToLower() == request.Name.ToLower());

        if (existingIngredient != null)
            return existingIngredient;

        var ingredient = new Ingredient
        {
            Name = request.Name,
            Description = request.Description,
            Unit = request.Unit
        };

        _context.Ingredients.Add(ingredient);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetIngredient), new { id = ingredient.Id }, ingredient);
    }

    [HttpPost("add-to-recipe")]
    public async Task<ActionResult<RecipeIngredient>> AddIngredientToRecipe([FromBody] AddIngredientToRecipeRequest request)
    {
        var recipe = await _context.Recipes.FindAsync(request.RecipeId);
        if (recipe == null)
            return NotFound("Recipe not found");

        var ingredient = await _context.Ingredients.FindAsync(request.IngredientId);
        if (ingredient == null)
        {
            ingredient = new Ingredient
            {
                Name = request.IngredientName ?? $"Ingredient {request.IngredientId}",
                Unit = request.Unit
            };
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
        }

        var recipeIngredient = new RecipeIngredient
        {
            RecipeId = request.RecipeId,
            IngredientId = ingredient.Id,
            Quantity = request.Quantity,
            Unit = request.Unit
        };

        _context.RecipeIngredients.Add(recipeIngredient);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRecipeIngredient), new { id = recipeIngredient.Id }, recipeIngredient);
    }

    [HttpGet("recipe-ingredient/{id}")]
    public async Task<ActionResult<RecipeIngredient>> GetRecipeIngredient(int id)
    {
        var recipeIngredient = await _context.RecipeIngredients
            .Include(ri => ri.Ingredient)
            .FirstOrDefaultAsync(ri => ri.Id == id);

        if (recipeIngredient == null)
            return NotFound();

        return recipeIngredient;
    }

    [HttpDelete("recipe-ingredient/{id}")]
    public async Task<IActionResult> RemoveIngredientFromRecipe(int id)
    {
        var recipeIngredient = await _context.RecipeIngredients.FindAsync(id);
        if (recipeIngredient == null)
            return NotFound();

        _context.RecipeIngredients.Remove(recipeIngredient);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class CreateIngredientRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Unit { get; set; }
}

public class AddIngredientToRecipeRequest
{
    public int RecipeId { get; set; }
    public int? IngredientId { get; set; }
    public string? IngredientName { get; set; }
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
}
