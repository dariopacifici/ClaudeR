#!/bin/bash

# Claude'r Deploy Script
# Usage: ./deploy.sh <server_ip> <username> <password>

set -e

SERVER_IP="${1:-85.215.170.136}"
USERNAME="${2:-root}"
APP_DIR="/opt/clauder"
CERTS_DIR="$APP_DIR/certs"

echo "🚀 Claude'r Deployment Script"
echo "================================"
echo "Server: $SERVER_IP"
echo "App Directory: $APP_DIR"
echo ""

# Step 1: Generate SSL Certificates
echo "📋 Step 1: Generating SSL Certificates..."

# Create certs directory
mkdir -p certs

# Generate self-signed certificate (valid for 365 days)
openssl req -x509 -newkey rsa:4096 -keyout certs/clauder.key -out certs/clauder.crt \
  -days 365 -nodes -subj "/C=IT/ST=Italy/L=Italy/O=Clauder/CN=clauder.local"

echo "✅ SSL certificates generated:"
echo "   - certs/clauder.crt"
echo "   - certs/clauder.key"
echo ""

# Step 2: Create remote directory structure
echo "📋 Step 2: Setting up remote server..."
echo "Command to run on remote server ($SERVER_IP):"
echo ""
cat << 'EOF'
# Run these commands on the remote server:
ssh root@85.215.170.136 << 'REMOTE_SCRIPT'
#!/bin/bash

# Create app directory
mkdir -p /opt/clauder/data
mkdir -p /opt/clauder/certs

# Install Docker if not present
if ! command -v docker &> /dev/null; then
  apt-get update
  apt-get install -y docker.io docker-compose
  systemctl start docker
  systemctl enable docker
fi

# Create docker network
docker network create clauder-network || true

# Set proper permissions
chmod 755 /opt/clauder
chmod 755 /opt/clauder/data

echo "✅ Remote server ready!"
REMOTE_SCRIPT
EOF

echo ""
echo "📋 Step 3: Uploading files to remote server..."
echo ""
echo "Commands to sync files:"
cat << EOF
# Copy docker files to server:
scp docker-compose.yml root@$SERVER_IP:$APP_DIR/
scp nginx.conf root@$SERVER_IP:$APP_DIR/
scp Dockerfile root@$SERVER_IP:$APP_DIR/

# Copy source code:
scp -r Clauder.API root@$SERVER_IP:$APP_DIR/
scp -r Clauder.Domain root@$SERVER_IP:$APP_DIR/
scp -r Clauder.Data root@$SERVER_IP:$APP_DIR/
scp Clauder.slnx root@$SERVER_IP:$APP_DIR/

# Copy certificates:
scp certs/clauder.crt root@$SERVER_IP:$APP_DIR/certs/
scp certs/clauder.key root@$SERVER_IP:$APP_DIR/certs/
EOF

echo ""
echo "📋 Step 4: Deploy on remote server..."
echo ""
echo "Commands to run on remote server:"
cat << EOF
# SSH into server and run:
ssh root@$SERVER_IP << 'DEPLOY'
cd $APP_DIR

# Build Docker image
docker-compose build

# Start containers
docker-compose up -d

# Check status
docker-compose ps
docker logs clauder-app

# Test API
curl -k https://localhost/api/recipes

echo "✅ Deployment complete!"
DEPLOY
EOF

echo ""
echo "✅ All files prepared for deployment!"
echo ""
echo "📌 DEPLOYMENT STEPS SUMMARY:"
echo "1. Generate certs: ✅ Done"
echo "2. Prepare remote server: Run commands above"
echo "3. Upload files: Run commands above"
echo "4. Deploy: Run commands above"
echo ""
echo "🌐 App will be available at:"
echo "   https://85.215.170.136:85"
echo ""
echo "📝 Note: Using self-signed SSL certificate"
echo "   Browser will show security warning (expected)"
