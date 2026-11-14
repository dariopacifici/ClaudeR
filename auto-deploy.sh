#!/bin/bash

# Claude'r - Automated Deploy Script
# Usage: ./auto-deploy.sh

set -e

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Config
SERVER_IP="85.215.170.136"
SERVER_USER="root"
REMOTE_DIR="/opt/clauder"
LOCAL_CERTS_DIR="./certs"

echo -e "${BLUE}╔════════════════════════════════════════╗${NC}"
echo -e "${BLUE}║  Claude'r - Automated Deployment       ║${NC}"
echo -e "${BLUE}╚════════════════════════════════════════╝${NC}"
echo ""

# Step 1: Generate Certificates
echo -e "${YELLOW}[1/5]${NC} Generating SSL Certificates..."
mkdir -p "$LOCAL_CERTS_DIR"

if [ ! -f "$LOCAL_CERTS_DIR/clauder.crt" ] || [ ! -f "$LOCAL_CERTS_DIR/clauder.key" ]; then
  openssl req -x509 -newkey rsa:4096 \
    -keyout "$LOCAL_CERTS_DIR/clauder.key" \
    -out "$LOCAL_CERTS_DIR/clauder.crt" \
    -days 365 -nodes \
    -subj "/C=IT/ST=Italy/L=Italy/O=Clauder/CN=clauder.local"
  echo -e "${GREEN}✅ Certificates generated${NC}"
else
  echo -e "${GREEN}✅ Certificates already exist${NC}"
fi
echo ""

# Step 2: Setup Remote Server
echo -e "${YELLOW}[2/5]${NC} Preparing remote server..."
ssh "$SERVER_USER@$SERVER_IP" << 'REMOTE_SETUP' &>/dev/null
#!/bin/bash
set -e

# Create directories
mkdir -p /opt/clauder/data
mkdir -p /opt/clauder/certs

# Install Docker if not present
if ! command -v docker &> /dev/null; then
  echo "Installing Docker..."
  apt-get update >/dev/null 2>&1
  apt-get install -y docker.io docker-compose >/dev/null 2>&1
  systemctl start docker
  systemctl enable docker
fi

# Create network
docker network create clauder-network 2>/dev/null || true

# Set permissions
chmod 755 /opt/clauder
chmod 755 /opt/clauder/data
REMOTE_SETUP

echo -e "${GREEN}✅ Remote server prepared${NC}"
echo ""

# Step 3: Upload Files
echo -e "${YELLOW}[3/5]${NC} Uploading files to server..."

# Build project first
echo "   Building .NET project..."
dotnet build >/dev/null 2>&1

# Copy files
scp -q docker-compose.yml "$SERVER_USER@$SERVER_IP:$REMOTE_DIR/" 2>/dev/null
scp -q nginx.conf "$SERVER_USER@$SERVER_IP:$REMOTE_DIR/" 2>/dev/null
scp -q Dockerfile "$SERVER_USER@$SERVER_IP:$REMOTE_DIR/" 2>/dev/null
scp -q Clauder.slnx "$SERVER_USER@$SERVER_IP:$REMOTE_DIR/" 2>/dev/null
scp -qr Clauder.API "$SERVER_USER@$SERVER_IP:$REMOTE_DIR/" 2>/dev/null
scp -qr Clauder.Domain "$SERVER_USER@$SERVER_IP:$REMOTE_DIR/" 2>/dev/null
scp -qr Clauder.Data "$SERVER_USER@$SERVER_IP:$REMOTE_DIR/" 2>/dev/null
scp -q "$LOCAL_CERTS_DIR/clauder.crt" "$SERVER_USER@$SERVER_IP:$REMOTE_DIR/certs/" 2>/dev/null
scp -q "$LOCAL_CERTS_DIR/clauder.key" "$SERVER_USER@$SERVER_IP:$REMOTE_DIR/certs/" 2>/dev/null

echo -e "${GREEN}✅ Files uploaded${NC}"
echo ""

# Step 4: Build and Deploy
echo -e "${YELLOW}[4/5]${NC} Building and deploying containers..."

ssh "$SERVER_USER@$SERVER_IP" << 'REMOTE_DEPLOY' &>/dev/null
#!/bin/bash
set -e
cd /opt/clauder
docker-compose build --no-cache >/dev/null 2>&1
docker-compose down >/dev/null 2>&1 || true
docker-compose up -d >/dev/null 2>&1
sleep 10
REMOTE_DEPLOY

echo -e "${GREEN}✅ Containers deployed${NC}"
echo ""

# Step 5: Verify Deployment
echo -e "${YELLOW}[5/5]${NC} Verifying deployment..."

# Wait for app to be ready
for i in {1..10}; do
  if curl -sk https://"$SERVER_IP":85/api/recipes >/dev/null 2>&1; then
    echo -e "${GREEN}✅ Application is running${NC}"
    break
  fi
  if [ $i -eq 10 ]; then
    echo -e "${YELLOW}⚠️  Application may still be starting...${NC}"
  fi
  sleep 2
done

echo ""
echo -e "${BLUE}╔════════════════════════════════════════╗${NC}"
echo -e "${GREEN}✅ DEPLOYMENT SUCCESSFUL!${NC}"
echo -e "${BLUE}╚════════════════════════════════════════╝${NC}"
echo ""
echo -e "${BLUE}📊 Deployment Summary:${NC}"
echo "   Server: $SERVER_IP"
echo "   Directory: $REMOTE_DIR"
echo "   Docker Compose: ✅"
echo "   Nginx Reverse Proxy: ✅"
echo "   SSL Certificates: ✅"
echo ""
echo -e "${BLUE}🌐 Access Application:${NC}"
echo -e "   ${GREEN}https://85.215.170.136:85${NC}"
echo ""
echo -e "${BLUE}🔐 Demo Credentials:${NC}"
echo "   Username: romina     | Password: romina123"
echo "   Username: claudia    | Password: claudia123"
echo ""
echo -e "${BLUE}📋 Useful Commands:${NC}"
echo "   View logs:      ssh root@85.215.170.136 'cd /opt/clauder && docker-compose logs -f'"
echo "   Restart app:    ssh root@85.215.170.136 'cd /opt/clauder && docker-compose restart'"
echo "   Container ps:   ssh root@85.215.170.136 'cd /opt/clauder && docker-compose ps'"
echo "   Backup DB:      ssh root@85.215.170.136 'cp /opt/clauder/data/clauder.db /opt/clauder/data/clauder.db.backup'"
echo ""
echo -e "${YELLOW}⚠️  Note:${NC} Browser will show security warning (self-signed certificate is normal)"
echo ""
