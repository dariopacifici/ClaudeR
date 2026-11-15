using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Clauder.Data;
using Clauder.Domain.Entities;

namespace Clauder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaFilesController : ControllerBase
{
    private const long MaxUploadBytes = 100L * 1024 * 1024;

    private readonly ClauerDbContext _context;
    private readonly IWebHostEnvironment _env;

    public MediaFilesController(ClauerDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(MaxUploadBytes)]
    public async Task<ActionResult<MediaFile>> UploadMedia([FromForm] IFormFile file, [FromForm] int recipeId, [FromForm] int? reviewId, [FromForm] int uploadedByUserId, [FromForm] string? caption)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required");

        var mediaType = file.ContentType?.StartsWith("image/") == true ? "image"
            : file.ContentType?.StartsWith("video/") == true ? "video"
            : null;
        if (mediaType == null)
            return BadRequest("Only image and video files are allowed");

        var recipe = await _context.Recipes.FindAsync(recipeId);
        if (recipe == null)
            return NotFound("Recipe not found");

        if (reviewId.HasValue)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
                return NotFound("Review not found");
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var uploadsFolder = Path.Combine(_env.ContentRootPath, "uploads");
        Directory.CreateDirectory(uploadsFolder);
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var mediaFile = new MediaFile
        {
            RecipeId = recipeId,
            ReviewId = reviewId,
            FileName = fileName,
            MediaType = mediaType,
            FileUrl = $"/uploads/{fileName}",
            FileSizeBytes = file.Length,
            Caption = caption,
            UploadedByUserId = uploadedByUserId
        };

        _context.MediaFiles.Add(mediaFile);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMediaFile), new { id = mediaFile.Id }, mediaFile);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MediaFile>> GetMediaFile(int id)
    {
        var mediaFile = await _context.MediaFiles
            .Include(m => m.UploadedByUser)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (mediaFile == null)
            return NotFound();

        return mediaFile;
    }

    [HttpGet("recipe/{recipeId}")]
    public async Task<ActionResult<IEnumerable<MediaFile>>> GetRecipeMedia(int recipeId)
    {
        var media = await _context.MediaFiles
            .Where(m => m.RecipeId == recipeId && m.ReviewId == null)
            .OrderBy(m => m.DisplayOrder)
            .ThenBy(m => m.Id)
            .Include(m => m.UploadedByUser)
            .ToListAsync();

        return media;
    }

    [HttpPost("link")]
    public async Task<ActionResult<MediaFile>> AddMediaLink([FromBody] AddMediaLinkRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
            return BadRequest("URL is required");

        var recipe = await _context.Recipes.FindAsync(request.RecipeId);
        if (recipe == null)
            return NotFound("Recipe not found");

        var maxOrder = await _context.MediaFiles
            .Where(m => m.RecipeId == request.RecipeId && m.ReviewId == null)
            .Select(m => (int?)m.DisplayOrder)
            .MaxAsync() ?? -1;

        var mediaFile = new MediaFile
        {
            RecipeId = request.RecipeId,
            FileName = "external-link",
            MediaType = request.MediaType ?? "video",
            FileUrl = request.Url.Trim(),
            FileSizeBytes = 0,
            Caption = request.Caption,
            UploadedByUserId = request.UploadedByUserId,
            DisplayOrder = maxOrder + 1
        };

        _context.MediaFiles.Add(mediaFile);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMediaFile), new { id = mediaFile.Id }, mediaFile);
    }

    [HttpPut("recipe/{recipeId}/reorder")]
    public async Task<IActionResult> ReorderRecipeMedia(int recipeId, [FromBody] List<int> orderedIds)
    {
        var media = await _context.MediaFiles
            .Where(m => m.RecipeId == recipeId && m.ReviewId == null)
            .ToListAsync();

        for (int i = 0; i < orderedIds.Count; i++)
        {
            var item = media.FirstOrDefault(m => m.Id == orderedIds[i]);
            if (item != null)
            {
                item.DisplayOrder = i;
            }
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id}/set-preview")]
    public async Task<IActionResult> SetPreviewImage(int id)
    {
        var target = await _context.MediaFiles.FindAsync(id);
        if (target == null)
            return NotFound();

        var recipeMedia = await _context.MediaFiles
            .Where(m => m.RecipeId == target.RecipeId && m.ReviewId == null)
            .ToListAsync();

        foreach (var item in recipeMedia)
        {
            item.IsPreview = (item.Id == id);
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("review/{reviewId}")]
    public async Task<ActionResult<IEnumerable<MediaFile>>> GetReviewMedia(int reviewId)
    {
        var media = await _context.MediaFiles
            .Where(m => m.ReviewId == reviewId)
            .Include(m => m.UploadedByUser)
            .ToListAsync();

        return media;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMediaFile(int id)
    {
        var mediaFile = await _context.MediaFiles.FindAsync(id);
        if (mediaFile == null)
            return NotFound();

        if (mediaFile.FileName != "external-link")
        {
            var filePath = Path.Combine(_env.ContentRootPath, "uploads", mediaFile.FileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        _context.MediaFiles.Remove(mediaFile);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class AddMediaLinkRequest
{
    public int RecipeId { get; set; }
    public required string Url { get; set; }
    public string? MediaType { get; set; }
    public string? Caption { get; set; }
    public int UploadedByUserId { get; set; }
}
