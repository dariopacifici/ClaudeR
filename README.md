# Claude'r - Le Ricette di Claudia 🍝

Un'app mobile-first moderna per gestire, condividere e scoprire ricette incredibili, realizzata con **amore** e **tecnologia**.

## 📱 Features Principali

✅ **Gestione Ricette**
- Crea, visualizza, modifica ricette
- Auto-aggiunta ingredienti dal database
- Categorizzazione per nazionalità e tipo
- Upload foto e video

✅ **Sistema di Valutazione**
- Vota ricette con stelle (1-5)
- Leaderboard dinamico delle ricette più apprezzate
- Commenti e feedback

✅ **Recensioni Interattive**
- Solo Romina e Claudia possono aggiungere recensioni dopo la pubblicazione
- Foto e video nelle recensioni
- Blocchi di revisione strutturati

✅ **Design Mobile-First**
- Fully responsive (smartphone e tablet)
- Interfaccia touch-friendly
- Bottom navigation per facile accesso
- Tema colorato e accattivante

✅ **Autenticazione & Ruoli**
- Login/Register per utenti
- Ruoli: Amministratore, Recensore
- Utenti di default: Romina e Claudia

## 🛠️ Stack Tecnico

| Componente | Tecnologia |
|-----------|-----------|
| **Backend** | .NET 10 (.NET Core 10) |
| **API** | ASP.NET Core Web API |
| **Database** | SQLite con Entity Framework Core |
| **Frontend** | Bootstrap 5 + Vanilla JavaScript |
| **Localization** | Supporto per IT, EN, FR, ZH, JA |

## 🚀 Come Avviare

### Prerequisiti
- .NET 10 SDK
- Node.js (opzionale, per tools)

### Setup

```bash
# Clonare il repository
git clone <repo-url>
cd claudia

# Restore dependencies
dotnet restore

# Build
dotnet build

# Avviare l'API
cd Clauder.API
dotnet run

# L'app sarà disponibile su: http://localhost:5000
```

### Credenziali Demo

| Username | Password | Ruolo |
|----------|----------|-------|
| romina | romina123 | Admin, Recensore |
| claudia | claudia123 | Admin, Recensore |

## 📁 Struttura Progetto

```
Claudia/
├── Clauder.API/              # ASP.NET Core Web API
│   ├── Controllers/          # API endpoints
│   ├── Helpers/              # Utility functions
│   ├── Program.cs            # App configuration
│   └── appsettings.json      # Configuration
│
├── Clauder.Domain/           # Modelli di dominio
│   └── Entities/             # Domain models
│       ├── User.cs
│       ├── Recipe.cs
│       ├── Ingredient.cs
│       ├── RecipeIngredient.cs
│       ├── Rating.cs
│       ├── Review.cs
│       └── MediaFile.cs
│
├── Clauder.Data/             # Data Access Layer
│   └── ClauerDbContext.cs    # EF Core DbContext
│
└── wwwroot/                  # Frontend Assets
    ├── index.html            # SPA Main Page
    ├── css/
    │   └── custom.css        # Custom theme
    └── js/
        └── app.js            # SPA Logic

```

## 🗄️ Database Schema

### User
- Id, Username, Email, PasswordHash
- Role (Amministratore, Recensore)
- DisplayName, AvatarUrl, IsActive

### Recipe
- Id, Title, Description, Instructions
- Nationality, RecipeType
- CreatedByUserId, IsPublished
- PrepTimeMinutes, CookTimeMinutes, ServingsCount

### Ingredient
- Id, Name, Unit, Description

### RecipeIngredient (M:M)
- Id, RecipeId, IngredientId, Quantity, Unit

### Rating
- Id, RecipeId, UserId, Score (1-5), Comment

### Review
- Id, RecipeId, UserId, Title, Content
- Category, IsPublished

### MediaFile
- Id, RecipeId, ReviewId, FileName, FileUrl
- MediaType (image, video), FileSizeBytes, Caption

## 🔌 API Endpoints

### Recipes
- `GET /api/recipes` - Elenco ricette (con filtri)
- `GET /api/recipes/{id}` - Dettagli ricetta
- `POST /api/recipes` - Crea ricetta
- `PUT /api/recipes/{id}` - Modifica ricetta
- `POST /api/recipes/{id}/publish` - Pubblica ricetta

### Ingredients
- `GET /api/ingredients` - Elenco ingredienti
- `POST /api/ingredients` - Crea ingrediente
- `POST /api/ingredients/add-to-recipe` - Aggiungi a ricetta

### Ratings
- `POST /api/ratings` - Valuta ricetta
- `GET /api/ratings/recipe/{recipeId}` - Valutazioni ricetta
- `GET /api/ratings/leaderboard` - Leaderboard top 10

### Reviews
- `POST /api/reviews` - Crea recensione
- `GET /api/reviews/recipe/{recipeId}` - Recensioni ricetta
- `PUT /api/reviews/{id}` - Modifica recensione

### MediaFiles
- `POST /api/mediafiles/upload` - Upload foto/video
- `GET /api/mediafiles/recipe/{recipeId}` - Media ricetta
- `GET /api/mediafiles/review/{reviewId}` - Media recensione

### Users
- `GET /api/users` - Elenco utenti
- `POST /api/users/register` - Registrazione
- `POST /api/users/login` - Login
- `PUT /api/users/{id}` - Aggiorna profilo

## 🎨 Design Features

### Colori & Temi
- **Primary**: Rosso acceso (#FF6B6B) - Ricette, CTA
- **Secondary**: Turchese (#4ECDC4) - Accenti secondari
- **Success**: Verde (#38ADA9) - Azioni positive
- **Warning**: Giallo (#FFD93D) - Rating stars
- **Gradients**: Colori sfumati per impatto visivo

### Componenti UI
- Card con hover effects
- Bottom navigation mobile
- Modal per dettagli ricetta
- Star rating interattivo
- Leaderboard con ranking
- Filter per nazionalità e tipo
- Empty states con emoji

## 🔐 Sicurezza

- ✅ Password hashing con SHA256
- ✅ CORS configurato
- ✅ Validazione input
- ✅ Foreign key constraints
- ✅ Role-based access control (in development)

## 📱 Browser Support

- ✅ Chrome/Edge (Mobile & Desktop)
- ✅ Safari (iOS & macOS)
- ✅ Firefox (Mobile & Desktop)
- ✅ Samsung Internet

## 🎯 Roadmap

### Fase 1 (Completato ✅)
- Backend architecture
- Core API endpoints
- Frontend mobile UI
- Authentication

### Fase 2 (In Development 🚀)
- Localizzazione i18n
- Foto upload e storage
- Sharing WhatsApp/Social
- Push notifications
- Offline support

### Fase 3 (Planned 📋)
- Advanced filtering
- User recommendations
- Recipe statistics
- Community features
- Admin dashboard

## 🤝 Contribuire

1. Fork il repository
2. Crea un feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit i cambiamenti (`git commit -m 'Add AmazingFeature'`)
4. Push al branch (`git push origin feature/AmazingFeature`)
5. Apri una Pull Request

## 📄 Licenza

Questo progetto è sotto licenza MIT. Vedi il file `LICENSE` per dettagli.

## 👩‍🍳 Credits

Creato da **Claude Haiku** per **Claudia** e **Romina**.

Con amore, una ricetta alla volta. 🍝❤️

---

**Versione**: 1.0.0  
**Ultima Update**: Novembre 2026  
**Status**: 🟢 In Develop
