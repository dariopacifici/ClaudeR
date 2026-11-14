using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Clauder.Data;
using Clauder.Domain.Entities;
using Clauder.API.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<ClauerDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=clauder.db"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Initialize database and seed data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ClauerDbContext>();
    dbContext.Database.EnsureCreated();

    if (!dbContext.Users.Any())
    {
        var users = new[]
        {
            new User
            {
                Username = "romina",
                Email = "romina@claudia.app",
                PasswordHash = PasswordHelper.HashPassword("romina123"),
                Role = "Amministratore,Recensore",
                DisplayName = "Romina",
                AvatarUrl = "https://via.placeholder.com/150?text=Romina",
                IsActive = true
            },
            new User
            {
                Username = "claudia",
                Email = "claudia@claudia.app",
                PasswordHash = PasswordHelper.HashPassword("claudia123"),
                Role = "Amministratore,Recensore",
                DisplayName = "Claudia",
                AvatarUrl = "https://via.placeholder.com/150?text=Claudia",
                IsActive = true
            }
        };

        dbContext.Users.AddRange(users);
        dbContext.SaveChanges();
    }
}

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "uploads");
Directory.CreateDirectory(uploadsPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Serve index.html for SPA routing
app.MapFallbackToFile("index.html");

app.Run();
