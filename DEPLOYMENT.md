# Claude'r - Debug & Deployment Guide

## 🐛 DEBUG - Local Development

### Option 1: Direct Run (No Docker)
```bash
# Navigate to API project
cd Clauder.API

# Run in development mode
dotnet run

# App will be available at:
# http://localhost:5000
```

### Option 2: Debug with VS Code / Rider
```bash
# Open project in IDE
code .  # or open in Rider

# F5 or click "Run and Debug"
# Set breakpoints as needed
# Backend: Clauder.API
# Frontend: browser console (F12)
```

### Option 3: Docker Debug Locally
```bash
# Build image
docker build -t clauder:debug .

# Run container with volume mounts
docker run -it --rm \
  -p 5000:5000 \
  -v $(pwd)/Clauder.API:/app/src \
  -e ASPNETCORE_ENVIRONMENT=Development \
  clauder:debug
```

### Option 4: Debug with Docker Compose (Local HTTPS Test)
```bash
# Start containers
docker-compose up --build

# View logs
docker-compose logs -f clauder-api
docker-compose logs -f clauder-nginx

# Test endpoints
curl -k https://localhost/api/recipes

# Stop containers
docker-compose down
```

---

## 🚀 DEPLOYMENT - Remote Server (Ionos)

### Server Details
```
Host: 85.215.170.136
Username: root
Password: Fl@vio2010
OS: Debian
Port: 85 (HTTP), 443 (HTTPS)
```

### One-Command Deploy

```bash
# From project root directory:
bash deploy.sh
```

This will:
1. ✅ Generate SSL certificates locally
2. 📝 Show commands to prepare remote server
3. 📤 Show commands to upload files
4. 🚀 Show commands to start containers

### Manual Deployment Steps

#### Step 1: Generate SSL Certificates
```bash
# On local machine
mkdir -p certs
openssl req -x509 -newkey rsa:4096 \
  -keyout certs/clauder.key \
  -out certs/clauder.crt \
  -days 365 -nodes \
  -subj "/C=IT/ST=Italy/L=Italy/O=Clauder/CN=clauder.local"
```

#### Step 2: Prepare Remote Server
```bash
# SSH into remote server
ssh root@85.215.170.136

# Create directories
mkdir -p /opt/clauder/data
mkdir -p /opt/clauder/certs

# Install Docker (if not present)
apt-get update
apt-get install -y docker.io docker-compose
systemctl start docker
systemctl enable docker

# Create Docker network
docker network create clauder-network || true

# Exit SSH
exit
```

#### Step 3: Upload Project Files
```bash
# From local machine, copy everything to remote:
scp docker-compose.yml root@85.215.170.136:/opt/clauder/
scp nginx.conf root@85.215.170.136:/opt/clauder/
scp Dockerfile root@85.215.170.136:/opt/clauder/
scp Clauder.slnx root@85.215.170.136:/opt/clauder/
scp -r Clauder.API root@85.215.170.136:/opt/clauder/
scp -r Clauder.Domain root@85.215.170.136:/opt/clauder/
scp -r Clauder.Data root@85.215.170.136:/opt/clauder/
scp certs/clauder.crt root@85.215.170.136:/opt/clauder/certs/
scp certs/clauder.key root@85.215.170.136:/opt/clauder/certs/
```

#### Step 4: Deploy Containers
```bash
# SSH into remote server
ssh root@85.215.170.136

# Navigate to app directory
cd /opt/clauder

# Build Docker image
docker-compose build

# Start containers in background
docker-compose up -d

# Check container status
docker-compose ps

# View logs
docker logs clauder-app
docker logs clauder-nginx

# Test API endpoint
curl -k https://localhost/api/recipes

# Exit SSH
exit
```

#### Step 5: Access Application
```
🌐 Open in browser:
   https://85.215.170.136:85

⚠️ Browser Warning:
   - Self-signed certificate will show security warning
   - This is expected and normal
   - Click "Advanced" → "Continue to site"
```

---

## 📊 Monitoring & Maintenance

### Check Container Status
```bash
ssh root@85.215.170.136 << 'EOF'
cd /opt/clauder
docker-compose ps
EOF
```

### View Live Logs
```bash
ssh root@85.215.170.136 << 'EOF'
cd /opt/clauder
docker-compose logs -f
EOF
```

### Restart Application
```bash
ssh root@85.215.170.136 << 'EOF'
cd /opt/clauder
docker-compose restart
EOF
```

### Stop Application
```bash
ssh root@85.215.170.136 << 'EOF'
cd /opt/clauder
docker-compose down
EOF
```

### Database Backup
```bash
ssh root@85.215.170.136 << 'EOF'
cd /opt/clauder
docker-compose exec clauder-api cp /app/data/clauder.db /app/data/clauder.db.backup
EOF
```

### Update Application
```bash
# From local machine:

# 1. Make code changes
# 2. Commit to git
# 3. Upload updated files
scp -r Clauder.API root@85.215.170.136:/opt/clauder/

# 4. Rebuild and restart
ssh root@85.215.170.136 << 'EOF'
cd /opt/clauder
docker-compose build --no-cache
docker-compose up -d
EOF
```

---

## 🔧 Troubleshooting

### Container won't start
```bash
ssh root@85.215.170.136
cd /opt/clauder
docker-compose logs clauder-app
```

### Database connection error
```bash
ssh root@85.215.170.136
# Ensure /opt/clauder/data directory exists and is writable
ls -la /opt/clauder/data/
chmod 755 /opt/clauder/data
```

### Certificate error
```bash
ssh root@85.215.170.136
# Regenerate certificates locally and re-upload
cd /opt/clauder/certs
ls -la
# Should see: clauder.crt and clauder.key
```

### Port 85 already in use
```bash
ssh root@85.215.170.136
# Find what's using port 85
lsof -i :85
# Modify docker-compose.yml to use different port
```

---

## 📋 Checklist for Deployment

- [ ] SSL certificates generated (`certs/clauder.crt`, `certs/clauder.key`)
- [ ] Remote server has Docker installed
- [ ] `/opt/clauder/` directory created on remote
- [ ] All project files uploaded to remote
- [ ] Certificates uploaded to `/opt/clauder/certs/`
- [ ] `docker-compose build` successful
- [ ] `docker-compose up -d` running
- [ ] `docker-compose ps` shows all containers running
- [ ] `curl -k https://85.215.170.136:85/api/recipes` responds
- [ ] Browser can access https://85.215.170.136:85

---

## 🌐 Final Access

Once deployed successfully:

**App URL**: `https://85.215.170.136:85`  
**API Base**: `https://85.215.170.136:85/api`  
**Demo Users**:
- Username: `romina` | Password: `romina123`
- Username: `claudia` | Password: `claudia123`

---

## 📞 Support

For issues:
1. Check logs: `docker-compose logs`
2. Verify network: `docker network ls`
3. Test connectivity: `curl -k https://85.215.170.136:85`
4. Review nginx config: Check `nginx.conf`

---

**Version**: 1.0.0  
**Last Updated**: November 14, 2026  
**Status**: 🟢 Ready for Deployment
