using System.Text.Json.Serialization;

namespace Clauder.Domain.Entities;

public class Review
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    [JsonIgnore]
    public Recipe? Recipe { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string? Category { get; set; } // Can categorize reviews as "Tasting Notes", "Preparation Tips", etc.
    public bool IsPublished { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();
}
