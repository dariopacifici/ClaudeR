using System.Text.Json.Serialization;

namespace Clauder.Domain.Entities;

public class Ingredient
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Unit { get; set; } // "g", "ml", "pz", etc.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
}
