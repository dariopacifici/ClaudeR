# VSCode Configuration for Claude'r

This directory contains VSCode configuration files for debugging and running Claude'r.

## Files

- **launch.json** - Debug configurations (F5)
- **tasks.json** - Build, run, and deployment tasks (Ctrl+Shift+P)
- **settings.json** - Editor and project settings
- **extensions.json** - Recommended VSCode extensions

---

## 🚀 Quick Start in VSCode

### 1. Open Project
```bash
code /home/dario/Source/Claudia
```

### 2. Install Recommended Extensions
When VSCode opens, it will suggest extensions. Click "Install All".

Or manually install:
- **C# Dev Kit** (ms-dotnettools.csharp)
- **Docker** (ms-azuretools.vscode-docker)
- **REST Client** (humao.rest-client)

### 3. Debug Configurations

#### Start Debug (F5)
- Builds the project
- Launches API on localhost:5000
- Opens browser automatically
- Set breakpoints in C# code

**Available Configurations:**
- 🚀 **Debug API (localhost:5000)** - Standard debug
- 🔍 **Debug API (Attach)** - Attach to running process
- 🐳 **Debug API (Docker)** - Debug in Docker container

### 4. Run Tasks (Ctrl+Shift+P)

Type `>Task:` to see all available tasks:

#### Build & Run
- `🔨 build` - Build solution
- `🏃 run (API only)` - Run without build
- `✅ build & restore` - Clean build
- `🧹 clean` - Clean build artifacts

#### Docker
- `🐳 docker-build` - Build Docker image
- `🐳 docker-up` - Start containers (with HTTPS)
- `🐳 docker-down` - Stop containers
- `🐳 docker-logs` - View container logs

#### Deployment
- `🚀 deploy-auto (ONE-COMMAND)` - Automatic deploy to server
- `📤 deploy-manual` - Step-by-step deployment guide
- `📋 generate-certs` - Generate SSL certificates

#### Testing
- `🌐 test-local-api` - Test localhost:5000
- `🌐 test-docker-api` - Test Docker HTTPS
- `🌐 test-remote-api` - Test remote server

#### Remote Management
- `🔐 ssh-remote` - SSH to remote server
- `📋 ssh-logs-remote` - View remote logs (streaming)
- `🔄 ssh-restart-remote` - Restart remote containers
- `💾 ssh-backup-remote` - Backup remote database

#### Documentation
- `📖 open-docs` - Open QUICK_START.md and DEPLOYMENT.md
- `🎓 open-all-docs` - Open all documentation

---

## 🎯 Workflows

### Development Workflow
```
1. Press F5 to start debugging
2. Set breakpoints in C# code
3. Use browser DevTools (F12) for frontend
4. Auto-reload on save
5. Stop with Shift+F5
```

### Local Docker Test (HTTPS)
```
1. Run task: "🐳 docker-up"
2. Run task: "🌐 test-docker-api"
3. Open: https://localhost/
4. Run task: "🐳 docker-down" to stop
```

### Deploy to Production
```
1. Run task: "🚀 deploy-auto"
2. Wait 2-3 minutes for build
3. Run task: "🌐 test-remote-api"
4. Open: https://85.215.170.136:85
```

### Monitor Remote Server
```
1. Run task: "📋 ssh-logs-remote" - View logs
2. Run task: "🔄 ssh-restart-remote" - Restart containers
3. Run task: "💾 ssh-backup-remote" - Backup database
```

---

## ⌨️ Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `F5` | Start debugging |
| `Shift+F5` | Stop debugging |
| `Ctrl+Shift+P` | Open Command Palette (tasks) |
| `Ctrl+Shift+B` | Run build task |
| `Ctrl+`` | Open Terminal |
| `Ctrl+J` | Toggle Panel |
| `F12` | Open DevTools (browser) |
| `Ctrl+,` | Open Settings |

---

## 📝 Common Tasks

### Debug Local API
```
F5 → Choose "Debug API (localhost:5000)"
```

### Run without Debugging
```
Ctrl+Shift+P → "🏃 run (API only)"
```

### Deploy to Server
```
Ctrl+Shift+P → "🚀 deploy-auto"
```

### View Remote Logs
```
Ctrl+Shift+P → "📋 ssh-logs-remote"
```

### Test API
```
Ctrl+Shift+P → "🌐 test-local-api"
```

---

## 🔧 Customization

### Edit Tasks
Edit `.vscode/tasks.json` to:
- Change ports
- Add new deployment targets
- Modify build parameters
- Add custom scripts

### Edit Launch Config
Edit `.vscode/launch.json` to:
- Change debug ports
- Add environment variables
- Modify program path
- Add additional configurations

### Edit Settings
Edit `.vscode/settings.json` to:
- Change formatter
- Adjust editor font/size
- Modify file exclusions
- Add workspace settings

---

## 📦 Recommended Extensions

These are automatically recommended when you open the project:

| Extension | Purpose |
|-----------|---------|
| **C# Dev Kit** | C# language support and debugging |
| **Docker** | Docker/Docker Compose support |
| **REST Client** | Test API endpoints (can replace Postman) |
| **GitLens** | Enhanced Git integration |
| **GitHub Copilot** | AI code completion |
| **Prettier** | Code formatting |
| **Material Icon Theme** | Better file icons |
| **Remote SSH** | SSH into remote servers |
| **Todo Highlight** | Highlight TODO comments |

---

## 🆘 Troubleshooting

### Debug won't start
```
1. Run "🔨 build" task first
2. Check if port 5000 is in use
3. Clear bin/obj folders: Run "🧹 clean"
4. Restart VSCode
```

### Docker errors
```
1. Ensure Docker is running
2. Run "🐳 docker-down" to stop existing containers
3. Run "🐳 docker-build" to rebuild
```

### SSH won't connect
```
1. Verify server IP: 85.215.170.136
2. Check password: Fl@vio2010
3. Test with: "🔐 ssh-remote"
4. Add SSH key for passwordless access
```

### Tasks don't show
```
1. Restart VSCode
2. Run Ctrl+Shift+P → "Tasks: Rerun Last Task"
3. Check if tasks.json is valid JSON
```

---

## 📚 Documentation Links

- **QUICK_START.md** - Quick reference for common commands
- **DEPLOYMENT.md** - Detailed deployment guide
- **CLAUDE.md** - Project architecture and conventions
- **README.md** - Project overview

---

## 🎓 Example: Debug a Specific Controller

```csharp
// Set breakpoint in RecipesController.cs
public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes(...)
{
    // Put cursor here and press Ctrl+Shift+D → Toggle Breakpoint
    var query = _context.Recipes...
}
```

Then:
1. Press `F5` to start debugging
2. Open browser to `http://localhost:5000`
3. Navigate to recipes page
4. Execution stops at breakpoint
5. Use Debug Console to inspect variables
6. Press `F10` to step through code

---

## 💡 Pro Tips

### Use REST Client Extension
Create `test.http` file:
```http
### Get all recipes
GET http://localhost:5000/api/recipes

### Login
POST http://localhost:5000/api/users/login
Content-Type: application/json

{
  "username": "romina",
  "password": "romina123"
}
```

Then click "Send Request" above each request.

### SSH Key Setup (passwordless)
```bash
# Generate key
ssh-keygen -t ed25519

# Copy to server
ssh-copy-id -i ~/.ssh/id_ed25519.pub root@85.215.170.136

# Test
ssh root@85.215.170.136
```

### Use VSCode Remote SSH
Install "Remote SSH" extension, then:
1. Click green icon (bottom-left)
2. "Connect to Host"
3. Choose "85.215.170.136"
4. Now you can edit files directly on server

---

## 📞 Support

If tasks or debug don't work:
1. Check `.vscode/tasks.json` and `.vscode/launch.json` syntax
2. Run `dotnet build` manually to verify project builds
3. Ensure all prerequisites are installed (dotnet, docker, etc.)
4. Review VSCode Output panel for error messages
5. Check "Problems" panel (Ctrl+Shift+M) for diagnostics

---

**Version**: 1.0.0  
**Last Updated**: November 14, 2026  
**Created for**: Claude'r Development
