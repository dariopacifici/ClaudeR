using System.Text.Json.Serialization;

namespace Clauder.Domain.Entities;

public class MediaFile
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    [JsonIgnore]
    public Recipe? Recipe { get; set; }
    public int? ReviewId { get; set; } // Can be attached to review or recipe directly
    [JsonIgnore]
    public Review? Review { get; set; }
    public required string FileName { get; set; }
    public required string MediaType { get; set; } // "image", "video"
    public required string FileUrl { get; set; }
    public long FileSizeBytes { get; set; }
    public string? Caption { get; set; }
    public int UploadedByUserId { get; set; }
    public User? UploadedByUser { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsPreview { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
