# Claude'r - Development Instructions

## Project Overview
**Claude'r - Le Ricette di Claudia** è un'applicazione mobile-first per gestire, condividere e scoprire ricette. 

**Stack**: .NET 10 Backend + Bootstrap 5 Frontend + SQLite Database

---

## 📋 Core Requirements

### 1. Recipe Management ✅
- CRUD operations for recipes
- **Auto-ingredient creation**: If ingredient doesn't exist, create it automatically
- Categorization by Nationality & Type
- Photo/Video support

### 2. User System ✅
- Authentication: Login/Register
- Two default users: **Romina** and **Claudia** (both Admin/Reviewer)
- Role-based access control

### 3. Ratings & Leaderboard ✅
- 1-5 star rating system
- Dynamic leaderboard of top-rated recipes
- Comments on ratings

### 4. Reviews & Media ✅
- Only Romina & Claudia can add post-publication reviews
- Photo/video attachments to reviews
- Review blocks transformation

### 5. Mobile-First UI ✅
- Fully responsive design
- Touch-friendly buttons (min 44px)
- Bottom navigation
- Bootstrap 5 theming

### 6. Sharing Features 🚀 (In Progress)
- WhatsApp share
- Social media share
- Copy recipe link

---

## 🏗️ Project Structure

### Backend (.NET 10)
```
Clauder.API/
  ├── Controllers/
  │   ├── RecipesController.cs
  │   ├── IngredientsController.cs
  │   ├── RatingsController.cs
  │   ├── ReviewsController.cs
  │   ├── MediaFilesController.cs
  │   └── UsersController.cs
  ├── Helpers/
  │   └── PasswordHelper.cs
  └── Program.cs

Clauder.Domain/
  └── Entities/
      ├── User.cs
      ├── Recipe.cs
      ├── Ingredient.cs
      ├── RecipeIngredient.cs
      ├── Rating.cs
      ├── Review.cs
      └── MediaFile.cs

Clauder.Data/
  └── ClauerDbContext.cs
```

### Frontend (Bootstrap 5)
```
wwwroot/
  ├── index.html          # SPA main page
  ├── css/custom.css      # Custom theme (vibrant gradients)
  └── js/app.js           # SPA logic
```

---

## 🎨 Design System

### Colors
- **Primary**: #FF6B6B (Red) - CTA, Headers
- **Secondary**: #4ECDC4 (Teal) - Accents
- **Success**: #38ADA9 (Green) - Positive actions
- **Warning**: #FFD93D (Yellow) - Rating stars
- **Background**: #F7F8FA (Light gray)

### Components
- Cards with hover effects
- Bottom navigation (mobile)
- Star rating input
- Leaderboard ranking
- Empty states with emoji

### Typography
- Bold headers (800 weight)
- Regular body text
- Rounded corners (0.8-1.2rem)

---

## 📱 Mobile UX Principles

1. **Touch Targets**: Minimum 44x44px for buttons
2. **Bottom Navigation**: For primary navigation
3. **FAB Button**: Floating Action Button for recipe creation
4. **Modals**: Full-screen on small devices
5. **Forms**: Large inputs, clear labels
6. **No Hover**: Use active states instead

---

## 🔌 API Conventions

### Request/Response Format
```json
{
  "id": 1,
  "title": "Pasta",
  "createdAt": "2026-11-14T10:30:00Z"
}
```

### Error Handling
```json
{
  "error": "Recipe not found",
  "statusCode": 404
}
```

### Pagination (Future)
```
GET /api/recipes?page=1&pageSize=10
```

---

## 🔐 Security Notes

- ✅ SHA256 password hashing
- ✅ CORS enabled for mobile
- ✅ Input validation on API
- 🚀 JWT tokens (planned)
- 🚀 Rate limiting (planned)

---

## 🚀 Development Workflow

### Starting the App
```bash
cd Clauder.API
dotnet run
# Open http://localhost:5000
```

### Database
- SQLite file: `clauder.db` (auto-created)
- Auto-migrations on startup via EF Core

### Adding a New Feature

#### Backend:
1. Add model in `Clauder.Domain/Entities/`
2. Add DbSet in `ClauerDbContext.cs`
3. Create controller in `Clauder.API/Controllers/`
4. Update `Program.cs` if needed (middleware, services)

#### Frontend:
1. Add HTML in `index.html` (new `data-page` section)
2. Add JS functions in `app.js`
3. Update bottom nav if needed
4. Test on mobile viewport

### Testing
```bash
# Build
dotnet build

# Run
dotnet run

# Test endpoints
curl http://localhost:5000/api/recipes
```

---

## 🎯 Next Steps

### Priority 1 (Core)
- [ ] Ingredient management with bulk add
- [ ] Media upload endpoint and storage
- [ ] Review functionality for Romina/Claudia

### Priority 2 (Features)
- [ ] Localization (i18n) - IT, EN, FR, ZH, JA
- [ ] WhatsApp/Social sharing
- [ ] Push notifications
- [ ] Advanced filtering

### Priority 3 (Polish)
- [ ] Error handling UI
- [ ] Loading states
- [ ] Offline support
- [ ] Performance optimization

---

## 📖 Conventions

### Naming
- **Classes**: PascalCase (Recipe, RecipeController)
- **Variables**: camelCase (recipeTitle, userId)
- **IDs**: Always include entity name (recipeId, not id)
- **Functions**: Verb + Noun (loadRecipes, createRecipe)

### Code Style
- .NET: Follow Microsoft style guide
- JavaScript: Use async/await, arrow functions
- No comments unless WHY is non-obvious

### Commit Messages
```
Brief description of change

- Bullet points of implementation details (optional)
```

---

## ❓ FAQ

**Q: How to reset the database?**  
A: Delete `clauder.db` file, run `dotnet run`

**Q: How to add a new role?**  
A: Update User.Role property and add validation in controllers

**Q: How to support a new recipe type?**  
A: Add option in HTML select, emoji mapping in JS

**Q: Where are uploaded files stored?**  
A: `/uploads` folder (implementation pending)

---

## 🔗 Resources

- [.NET 10 Docs](https://learn.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [Bootstrap 5](https://getbootstrap.com/docs/5.0/)
- [SQLite](https://www.sqlite.org/docs.html)

---

## 📞 Support

For issues or questions, check:
1. README.md for feature overview
2. Source code comments
3. API endpoint documentation

---

**Last Updated**: November 14, 2026  
**Maintainer**: Claude Haiku  
**Status**: 🟢 Active Development
