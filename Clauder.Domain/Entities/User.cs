using System.Text.Json.Serialization;

namespace Clauder.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;
    public required string Role { get; set; } // "Amministratore", "Recensore"
    public string? AvatarUrl { get; set; }
    public string? DisplayName { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [JsonIgnore]
    public ICollection<Recipe> RecipesCreated { get; set; } = new List<Recipe>();
    [JsonIgnore]
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    [JsonIgnore]
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
