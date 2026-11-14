using System.Text.Json.Serialization;

namespace Clauder.Domain.Entities;

public class RecipeIngredient
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    [JsonIgnore]
    public Recipe? Recipe { get; set; }
    public int IngredientId { get; set; }
    public Ingredient? Ingredient { get; set; }
    public decimal Quantity { get; set; }
    public string? Unit { get; set; } // override from ingredient if needed
}
