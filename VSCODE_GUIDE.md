# Claude'r - VSCode Complete Operations Guide 🎯

## 🚀 Open Project in VSCode

```bash
cd /home/dario/Source/Claudia
code .
```

---

## 🎮 All Operations at Your Fingertips

### Debug Operations (F5 Menu)

| Operation | Shortcut | Description |
|-----------|----------|-------------|
| **🚀 Debug API (localhost:5000)** | `F5` | Start debugging API on local machine |
| **🔍 Debug API (Attach)** | `F5` then select | Attach to running process |
| **🐳 Debug API (Docker)** | `F5` then select | Debug inside Docker container |

---

### Build & Run Tasks (Ctrl+Shift+P)

Type any of these commands:

#### 🔨 Build Tasks
```
🔨 build                    → Build entire solution
✅ build & restore          → Clean rebuild with restore
🧹 clean                    → Clean all build artifacts
📦 restore                  → Restore NuGet packages
```

#### 🏃 Run Tasks
```
🏃 run (API only)           → Run API without debugging (no build)
```

#### 🐳 Docker Tasks
```
🐳 docker-build             → Build Docker image (clauder:latest)
🐳 docker-up                → Start containers (API + Nginx with HTTPS)
🐳 docker-down              → Stop all containers
🐳 docker-logs              → View live logs from clauder-api
```

#### 🚀 Deployment Tasks
```
🚀 deploy-auto              → ONE-COMMAND deploy to remote server
📤 deploy-manual            → Interactive step-by-step deployment guide
📋 generate-certs           → Generate SSL certificates (PKI)
```

#### 🌐 Testing Tasks
```
🌐 test-local-api           → Test API on localhost:5000
🌐 test-docker-api          → Test API in Docker (HTTPS)
🌐 test-remote-api          → Test API on remote server (85.215.170.130:85)
📊 server-status            → Show Docker containers and logs
```

#### 🔐 Remote Management Tasks
```
🔐 ssh-remote               → SSH into remote server (85.215.170.130)
📋 ssh-logs-remote          → View remote logs (streaming, live)
🔄 ssh-restart-remote       → Restart containers on remote
💾 ssh-backup-remote        → Backup database on remote server
```

#### 📖 Documentation Tasks
```
📖 open-docs                → Open QUICK_START.md & DEPLOYMENT.md
🎓 open-all-docs            → Open all documentation files
```

---

## ⌨️ Keyboard Shortcuts

| Shortcut | Action | What It Does |
|----------|--------|-------------|
| `F5` | Debug | Start debugging with selected configuration |
| `Shift+F5` | Stop | Stop debugging |
| `Ctrl+Shift+P` | Command | Open task/command palette |
| `Ctrl+Shift+B` | Build | Run default build task |
| `Ctrl+`` | Terminal | Toggle integrated terminal |
| `Ctrl+J` | Panel | Toggle panel (terminal/problems/debug) |
| `F9` | Breakpoint | Toggle breakpoint on line |
| `F10` | Step Over | Step over code line |
| `F11` | Step Into | Step into function |
| `Shift+F11` | Step Out | Step out of function |
| `Ctrl+Shift+D` | Debug | Open debug panel |
| `Ctrl+Shift+X` | Extensions | Open extensions panel |

---

## 📋 Common Workflows

### Workflow 1: Local Development (No Docker)
```
1. Press F5
   → Select "🚀 Debug API (localhost:5000)"
   → Auto-opens browser
   
2. Set breakpoints in C# code
   → Click line number to add breakpoint
   
3. Interact with app
   → Execution stops at breakpoint
   → Inspect variables in Debug Console
   
4. Press Shift+F5 to stop
```

### Workflow 2: Docker Local Testing (HTTPS)
```
1. Ctrl+Shift+P
   → "🐳 docker-up"
   → Builds and starts containers
   
2. Ctrl+Shift+P
   → "🌐 test-docker-api"
   → Tests HTTPS connection
   
3. Open browser: https://localhost/
   → Accept self-signed cert warning
   
4. Ctrl+Shift+P
   → "🐳 docker-down"
   → Stops containers
```

### Workflow 3: Deploy to Production (One Command!)
```
1. Ctrl+Shift+P
   → "🚀 deploy-auto"
   → Generates certs
   → Uploads files to server
   → Builds Docker image
   → Starts containers
   → Verifies deployment
   
   ⏱️ Wait 2-3 minutes...
   
2. Ctrl+Shift+P
   → "🌐 test-remote-api"
   → Verifies app is running
   
3. Open: https://85.215.170.130:85
   → Accept SSL warning
   → Login with: romina/romina123
```

### Workflow 4: Monitor Remote Server
```
1. Ctrl+Shift+P
   → "📋 ssh-logs-remote"
   → View live logs (streaming)
   
2. If something breaks:
   Ctrl+Shift+P
   → "🔄 ssh-restart-remote"
   → Restart containers
   
3. Before updates:
   Ctrl+Shift+P
   → "💾 ssh-backup-remote"
   → Backup database
```

### Workflow 5: Code Review & Testing
```
1. Ctrl+Shift+P
   → "🌐 test-local-api"
   → Verify API is responding
   
2. Open REST Client file (test.http)
   → Click "Send Request"
   → Test specific endpoints
   
3. Open DevTools (F12)
   → Check network requests
   → View console errors
   → Inspect DOM
```

---

## 🎯 Daily Operations

### Morning: Start Development
```
F5 → Select "🚀 Debug API (localhost:5000)"
→ Code → Test → Repeat
Shift+F5 → Stop
```

### Update Code & Deploy
```
Ctrl+Shift+P → "🔨 build"
Ctrl+Shift+P → "🚀 deploy-auto"
Wait 2-3 min
Ctrl+Shift+P → "🌐 test-remote-api"
Done!
```

### Quick Server Check
```
Ctrl+Shift+P → "📊 server-status"
→ See containers running
→ View last 10 lines of logs
```

### Backup Before Big Changes
```
Ctrl+Shift+P → "💾 ssh-backup-remote"
→ Database backed up to:
  /opt/clauder/data/clauder.db.backup
```

---

## 🔍 Debug Tips & Tricks

### Set Conditional Breakpoint
```
1. Right-click on breakpoint (red dot)
2. Select "Edit Breakpoint"
3. Enter condition: e.g., "id > 5"
4. Press Enter
```

### Watch Variables
```
1. In Debug panel → WATCH section
2. Click "+" to add expression
3. Type: recipeId, user.Username, etc.
4. See values update as you step
```

### Debug Console
```
1. Open Debug Console (Ctrl+Shift+Y)
2. Type C# commands:
   > recipeId
   > user.Email
   > recipes.Count
3. See results in real-time
```

### Logpoint (Instead of Breakpoint)
```
1. Right-click on line number
2. Select "Add Logpoint"
3. Type message: e.g., "Recipe ID: {recipeId}"
4. Execution doesn't stop, just logs
```

---

## 🚨 Troubleshooting

### Debug won't start
```
1. Run Ctrl+Shift+B → "🔨 build" first
2. Check if port 5000 is in use:
   netstat -tlnp | grep 5000
3. Run "🧹 clean" then rebuild
4. Restart VSCode
```

### Tasks don't appear
```
1. Restart VSCode (Ctrl+Shift+P → "Reload Window")
2. Verify .vscode/tasks.json is valid JSON
3. Check file not corrupted
4. Delete .vscode folder and re-clone
```

### Docker errors
```
1. Ensure Docker is running: docker ps
2. Clear old containers: docker-compose down
3. Rebuild: 🐳 docker-build
4. Check logs: docker logs clauder-app
```

### SSH connection fails
```
1. Test basic SSH: ssh root@85.215.170.130
2. Verify password: Fl@vio2010
3. Check server is online: ping 85.215.170.130
4. Add SSH key for persistent access:
   ssh-keygen -t ed25519
   ssh-copy-id -i ~/.ssh/id_ed25519.pub root@85.215.170.130
```

---

## 📊 VSCode Settings Customization

Edit `.vscode/settings.json` to:

### Change Editor Font
```json
"editor.fontFamily": "'Courier New', monospace",
"editor.fontSize": 14
```

### Change Theme
```json
// Install theme first, then:
"workbench.colorTheme": "Dracula"
```

### Change Terminal
```json
"terminal.integrated.defaultProfile.windows": "PowerShell",
"terminal.integrated.defaultProfile.linux": "bash"
```

### Disable Format on Save
```json
"editor.formatOnSave": false
```

---

## 🎓 REST Client Extension (Built-in Testing)

Create file: `test.http`

```http
@baseUrl = http://localhost:5000/api

### Get all recipes
GET {{baseUrl}}/recipes

### Login Romina
POST {{baseUrl}}/users/login
Content-Type: application/json

{
  "username": "romina",
  "password": "romina123"
}

### Create recipe
POST {{baseUrl}}/recipes
Content-Type: application/json

{
  "title": "Pasta Carbonara",
  "description": "Classic Roman pasta",
  "nationality": "Italiana",
  "recipeType": "Primo",
  "createdByUserId": 1
}
```

Then click "Send Request" above each request!

---

## 📦 Recommended Extensions

Auto-suggested when you open project:

| Extension | Command | Purpose |
|-----------|---------|---------|
| C# Dev Kit | `ms-dotnettools.csharp` | Debug C# code |
| Docker | `ms-azuretools.vscode-docker` | Manage containers |
| REST Client | `humao.rest-client` | Test APIs |
| GitLens | `eamodio.gitlens` | Enhanced Git |
| Copilot | `GitHub.Copilot` | AI assistant |
| Prettier | `esbenp.prettier-vscode` | Format code |
| Material Icons | `PKief.material-icon-theme` | Better icons |

Install all: Click "Install All" when prompted.

---

## 🔗 Quick Links

| Resource | Location |
|----------|----------|
| Debug Config | `.vscode/launch.json` |
| Tasks Config | `.vscode/tasks.json` |
| Settings | `.vscode/settings.json` |
| Recommended Ext | `.vscode/extensions.json` |
| This Guide | `.vscode/README.md` |
| Quick Start | `QUICK_START.md` |
| Deployment | `DEPLOYMENT.md` |
| Architecture | `CLAUDE.md` |
| Project Info | `README.md` |

---

## ✅ Setup Checklist

- [ ] Opened project in VSCode: `code .`
- [ ] Installed recommended extensions (install all when prompted)
- [ ] Tested debug: Press F5 → Select config → Should open browser
- [ ] Tested task: Ctrl+Shift+P → "🔨 build"
- [ ] Tested Docker: Ctrl+Shift+P → "🐳 docker-up"
- [ ] Verified SSH: Ctrl+Shift+P → "🔐 ssh-remote"
- [ ] Read `.vscode/README.md` for detailed guide

---

## 🎉 You're Ready!

Everything is configured and ready to use:

✅ **Debug** - F5  
✅ **Build** - Ctrl+Shift+B  
✅ **Run** - Ctrl+Shift+P → "run"  
✅ **Deploy** - Ctrl+Shift+P → "deploy-auto"  
✅ **Monitor** - Ctrl+Shift+P → "ssh-logs-remote"  

**Just open VSCode and start working!**

---

**Version**: 1.0.0  
**Last Updated**: November 14, 2026  
**Status**: 🟢 Production Ready

🍝 **Happy Coding!** 🍝
