# Claude'r - Quick Start Guide 🚀

## 📋 Projects Overview

**Claude'r - Le Ricette di Claudia** è completamente pronta per:
- ✅ Local development & debugging
- ✅ Docker deployment
- ✅ Production HTTPS on remote server

---

## 🐛 DEBUG LOCALLY

### Fastest Way (No Docker)
```bash
cd /home/dario/Source/Claudia/Clauder.API
dotnet run

# Open: http://localhost:5000
# Demo: romina/romina123
```

### With IDE Debugging (VS Code / Rider)
```bash
# In project root
code .

# Then: F5 or Run → Run and Debug
# - Set breakpoints in C# code
# - Use browser DevTools for JS (F12)
```

### Docker Local Test (with HTTPS)
```bash
cd /home/dario/Source/Claudia

# Test HTTPS locally
docker-compose up --build

# Open: https://localhost/api/recipes (ignore SSL warning)
# Logs: docker-compose logs -f clauder-api
# Stop: docker-compose down
```

---

## 🚀 DEPLOY TO PRODUCTION

### ⚡ One-Command Deploy
```bash
cd /home/dario/Source/Claudia
bash auto-deploy.sh
```

This will automatically:
1. ✅ Generate SSL certificates
2. ✅ Prepare remote server (Ionos)
3. ✅ Upload all project files
4. ✅ Build Docker image
5. ✅ Start containers
6. ✅ Verify deployment

**That's it!** App will be live at: **https://85.215.170.136:85**

### 🔧 Manual Deploy (if needed)

#### Step 1: Generate Certificates
```bash
cd /home/dario/Source/Claudia
mkdir -p certs
openssl req -x509 -newkey rsa:4096 \
  -keyout certs/clauder.key \
  -out certs/clauder.crt \
  -days 365 -nodes \
  -subj "/C=IT/ST=Italy/L=Italy/O=Clauder/CN=clauder.local"
```

#### Step 2: Prepare Server
```bash
ssh root@85.215.170.136 << 'EOF'
mkdir -p /opt/clauder/data /opt/clauder/certs
apt-get update && apt-get install -y docker.io docker-compose
systemctl start docker && systemctl enable docker
docker network create clauder-network 2>/dev/null || true
chmod 755 /opt/clauder /opt/clauder/data
EOF
```

#### Step 3: Upload Files
```bash
cd /home/dario/Source/Claudia

# Project files
scp docker-compose.yml nginx.conf Dockerfile Clauder.slnx root@85.215.170.136:/opt/clauder/
scp -r Clauder.API Clauder.Domain Clauder.Data root@85.215.170.136:/opt/clauder/

# Certificates
scp certs/* root@85.215.170.136:/opt/clauder/certs/
```

#### Step 4: Deploy
```bash
ssh root@85.215.170.136 << 'EOF'
cd /opt/clauder
docker-compose build
docker-compose up -d
sleep 10
docker-compose ps
EOF
```

#### Step 5: Verify
```bash
# Test API
curl -k https://85.215.170.136:85/api/recipes

# View logs
ssh root@85.215.170.136 'cd /opt/clauder && docker-compose logs -f'
```

---

## 📊 POST-DEPLOYMENT

### Monitor Application
```bash
# View logs (streaming)
ssh root@85.215.170.136 'cd /opt/clauder && docker-compose logs -f'

# Check containers status
ssh root@85.215.170.136 'cd /opt/clauder && docker-compose ps'

# CPU/Memory usage
ssh root@85.215.170.136 'docker stats clauder-app'
```

### Maintenance Commands
```bash
# Restart app
ssh root@85.215.170.136 'cd /opt/clauder && docker-compose restart'

# Stop app
ssh root@85.215.170.136 'cd /opt/clauder && docker-compose down'

# Backup database
ssh root@85.215.170.136 'cp /opt/clauder/data/clauder.db /opt/clauder/data/clauder.db.backup'

# Update code
# 1. Make changes locally
# 2. scp -r Clauder.API root@85.215.170.136:/opt/clauder/
# 3. ssh root@85.215.170.136 'cd /opt/clauder && docker-compose build && docker-compose up -d'
```

---

## 🌐 ACCESS APPLICATION

### Public URL
```
https://85.215.170.136:85
```

### Demo Credentials
```
Username: romina    | Password: romina123
Username: claudia   | Password: claudia123
```

### API Endpoints
```
GET    https://85.215.170.136:85/api/recipes
GET    https://85.215.170.136:85/api/ratings/leaderboard
POST   https://85.215.170.136:85/api/users/login
... (28+ endpoints available)
```

---

## 🔍 Troubleshooting

### App won't start
```bash
ssh root@85.215.170.136
cd /opt/clauder
docker-compose logs clauder-app
# Check for errors in output
```

### Port already in use
```bash
# Check what's using port 85
ssh root@85.215.170.136 'netstat -tlnp | grep 85'
# Or modify docker-compose.yml to use different port
```

### Database errors
```bash
ssh root@85.215.170.136
# Ensure directory exists and permissions are correct
ls -la /opt/clauder/data/
chmod 755 /opt/clauder/data
# Restart containers
cd /opt/clauder && docker-compose restart
```

### Certificate errors
```bash
ssh root@85.215.170.136
# Verify certs exist
ls -la /opt/clauder/certs/
# Should show: clauder.crt and clauder.key
```

### Nginx not proxying
```bash
ssh root@85.215.170.136
# Check nginx logs
docker logs clauder-nginx
# Verify api is accessible
docker exec clauder-app curl http://localhost:5000/api/recipes
```

---

## 📦 Project Structure

```
Claudia/
├── Clauder.API/                # Backend API
│   ├── Controllers/            # 6 API Controllers
│   ├── Helpers/                # Password hashing
│   ├── Program.cs              # App configuration
│   └── wwwroot/                # Frontend files
│       ├── index.html
│       ├── css/custom.css
│       └── js/app.js
├── Clauder.Domain/             # Domain models
├── Clauder.Data/               # EF Core DbContext
├── Dockerfile                  # Docker build config
├── docker-compose.yml          # Multi-container setup
├── nginx.conf                  # Reverse proxy config
├── auto-deploy.sh              # One-command deploy
├── DEPLOYMENT.md               # Full documentation
└── README.md                   # Project overview
```

---

## 📝 Important Notes

### SSL Certificates
- Generated with OpenSSL (self-signed)
- Valid for 365 days
- Browser will show security warning (normal & expected)
- Click "Advanced" → "Continue to site" to proceed

### Database
- SQLite (clauder.db)
- Auto-created on first run
- Persisted in Docker volume
- Backed up regularly

### CORS
- Enabled for mobile access
- Configured in Program.cs
- Safe for development

### Docker Network
- `clauder-network` (bridge)
- API and Nginx communicate through network
- Isolated from host system

---

## ✅ Deployment Checklist

- [ ] Generated SSL certificates
- [ ] SSH access to remote server verified
- [ ] Docker installed on remote server
- [ ] `/opt/clauder/` directory prepared
- [ ] All project files uploaded
- [ ] `docker-compose build` successful
- [ ] `docker-compose up -d` running
- [ ] `docker-compose ps` shows all containers
- [ ] API responds: `curl -k https://85.215.170.136:85/api/recipes`
- [ ] Browser can access app without connection errors

---

## 🎯 Next Steps

### Immediate (Priority 1)
1. **Test locally**: `dotnet run` and verify UI works
2. **Deploy**: `bash auto-deploy.sh`
3. **Monitor**: `ssh root@85.215.170.136 'cd /opt/clauder && docker-compose logs -f'`

### Short Term (Priority 2)
1. Implement media upload feature
2. Add recipe sharing (WhatsApp/Social)
3. Test on mobile devices

### Future (Priority 3)
1. Localization (i18n)
2. User recommendations
3. Admin dashboard
4. Performance optimization

---

## 🆘 Need Help?

### For Local Development
- Check `CLAUDE.md` for project structure
- Read `README.md` for API documentation
- View `API.http` for endpoint examples

### For Deployment Issues
- Check `DEPLOYMENT.md` for detailed troubleshooting
- Run: `docker-compose logs` for error messages
- SSH to remote: `ssh root@85.215.170.136`

### Server Access
```
Host: 85.215.170.136
User: root
Password: Fl@vio2010
```

---

## 📊 Quick Status

| Component | Status | Location |
|-----------|--------|----------|
| Backend API | ✅ Ready | Clauder.API/ |
| Frontend | ✅ Ready | Clauder.API/wwwroot/ |
| Database | ✅ Ready | SQLite (auto-create) |
| Docker | ✅ Ready | Dockerfile + compose |
| SSL/HTTPS | ✅ Ready | Nginx + self-signed certs |
| Deployment | ✅ Ready | auto-deploy.sh |
| Documentation | ✅ Ready | DEPLOYMENT.md |

---

**Version**: 1.0.0 Production Ready  
**Last Updated**: November 14, 2026  
**Created**: Claude Haiku 4.5  
**For**: Claudia & Romina 👨‍🍳👩‍🍳

🍝 **Buon appetito!** 🍝
