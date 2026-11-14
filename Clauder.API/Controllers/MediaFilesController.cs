using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Clauder.Data;
using Clauder.Domain.Entities;

namespace Clauder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaFilesController : ControllerBase
{
    private readonly ClauerDbContext _context;
    private readonly IWebHostEnvironment _env;

    public MediaFilesController(ClauerDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [HttpPost("upload")]
    public async Task<ActionResult<MediaFile>> UploadMedia([FromForm] IFormFile file, [FromForm] int recipeId, [FromForm] int? reviewId, [FromForm] int uploadedByUserId, [FromForm] string? caption)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required");

        var recipe = await _context.Recipes.FindAsync(recipeId);
        if (recipe == null)
            return NotFound("Recipe not found");

        if (reviewId.HasValue)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
                return NotFound("Review not found");
        }

        var mediaType = file.ContentType?.Contains("image") == true ? "image" : "video";
        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
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
            .Include(m => m.UploadedByUser)
            .ToListAsync();

        return media;
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

        var filePath = Path.Combine(_env.ContentRootPath, "uploads", mediaFile.FileName);
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }

        _context.MediaFiles.Remove(mediaFile);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
