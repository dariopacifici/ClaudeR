namespace Clauder.Domain.Entities;

public class Recipe
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public string? Instructions { get; set; }
    public int ServingsCount { get; set; }
    public int PrepTimeMinutes { get; set; }
    public int CookTimeMinutes { get; set; }

    // Categorization
    public required string Nationality { get; set; } // "Italiana", "Giapponese", etc.
    public required string RecipeType { get; set; } // "Primo", "Secondo", "Dolce", "Antipasto", "Contorno"

    // Metadata
    public int CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }
    public bool IsPublished { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Relations
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    public ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
