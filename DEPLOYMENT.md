# 🚀 StockFlow Pro - Deployment Guide

Complete guide to deploy StockFlow Pro backend to Azure with CI/CD pipeline.

---

## 📋 Table of Contents

1. [Prerequisites](#prerequisites)
2. [Quick Start - Automated Deployment](#quick-start---automated-deployment)
3. [Manual Azure Setup](#manual-azure-setup)
4. [GitHub Actions CI/CD Setup](#github-actions-cicd-setup)
5. [Configuration](#configuration)
6. [Docker Deployment](#docker-deployment)
7. [Free Hosting Alternatives](#free-hosting-alternatives)
8. [Troubleshooting](#troubleshooting)

---

## 🔧 Prerequisites

### Required Tools
- **Azure CLI**: [Install Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
- **.NET 8.0 SDK**: [Download .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Git**: [Download Git](https://git-scm.com/downloads)
- **GitHub Account**: For CI/CD pipeline

### Required Azure Resources
- Azure Subscription (Free trial available)
- Resource Group
- App Service Plan
- Web App
- Azure SQL Database

---

## ⚡ Quick Start - Automated Deployment

### Option 1: PowerShell (Windows)

```powershell
# 1. Clone the repository
git clone https://github.com/yourusername/StockFlow-Pro.git
cd StockFlow-Pro

# 2. Login to Azure
az login

# 3. Run deployment script
.\deploy-azure.ps1
```

### Option 2: Bash (Linux/macOS/WSL)

```bash
# 1. Clone the repository
git clone https://github.com/yourusername/StockFlow-Pro.git
cd StockFlow-Pro

# 2. Make script executable
chmod +x deploy-azure.sh

# 3. Login to Azure
az login

# 4. Run deployment script
./deploy-azure.sh
```

**The script will:**
- ✅ Create Resource Group
- ✅ Create App Service Plan
- ✅ Create Web App (.NET 8.0)
- ✅ Create Azure SQL Server
- ✅ Create SQL Database
- ✅ Configure firewall rules
- ✅ Set connection strings
- ✅ Generate JWT secret
- ✅ Enable HTTPS

**Estimated time:** 5-10 minutes

---

## 🔨 Manual Azure Setup

If you prefer manual setup or need more control:

### Step 1: Create Resource Group

```bash
az group create \
  --name StockFlowPro-RG \
  --location eastus
```

### Step 2: Create App Service Plan

```bash
# For production (Basic tier - $13/month)
az appservice plan create \
  --name StockFlowPro-Plan \
  --resource-group StockFlowPro-RG \
  --location eastus \
  --sku B1 \
  --is-linux

# For free tier (limited resources)
az appservice plan create \
  --name StockFlowPro-Plan \
  --resource-group StockFlowPro-RG \
  --location eastus \
  --sku F1 \
  --is-linux
```

### Step 3: Create Web App

```bash
az webapp create \
  --name stockflowpro-api \
  --resource-group StockFlowPro-RG \
  --plan StockFlowPro-Plan \
  --runtime "DOTNET:8.0"
```

### Step 4: Create SQL Server

```bash
az sql server create \
  --name stockflowpro \
  --resource-group StockFlowPro-RG \
  --location eastus \
  --admin-user stockflowadmin \
  --admin-password "YourSecurePassword123!"
```

### Step 5: Configure SQL Firewall

```bash
# Allow Azure services
az sql server firewall-rule create \
  --resource-group StockFlowPro-RG \
  --server stockflowpro \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0
```

### Step 6: Create SQL Database

```bash
az sql db create \
  --resource-group StockFlowPro-RG \
  --server stockflowpro \
  --name StockFlowPro \
  --service-objective Basic
```

### Step 7: Configure App Settings

```bash
# Set connection string
az webapp config connection-string set \
  --name stockflowpro-api \
  --resource-group StockFlowPro-RG \
  --connection-string-type SQLAzure \
  --settings DefaultConnection="Server=tcp:stockflowpro.database.windows.net,1433;Initial Catalog=StockFlowPro;User ID=stockflowadmin;Password=YourSecurePassword123!;Encrypt=True;"

# Set JWT settings
az webapp config appsettings set \
  --name stockflowpro-api \
  --resource-group StockFlowPro-RG \
  --settings \
    JwtSettings__SecretKey="Your-Super-Secret-JWT-Key-Min-32-Chars!" \
    JwtSettings__Issuer="StockFlowProAPI" \
    JwtSettings__Audience="StockFlowProClient"
```

---

## 🔄 GitHub Actions CI/CD Setup

### Step 1: Get Publish Profile

```bash
az webapp deployment list-publishing-profiles \
  --name stockflowpro-api \
  --resource-group StockFlowPro-RG \
  --xml > publish-profile.xml
```

### Step 2: Add GitHub Secret

1. Go to your GitHub repository
2. Navigate to **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Name: `AZURE_WEBAPP_PUBLISH_PROFILE`
5. Value: Copy and paste the content from `publish-profile.xml`
6. Click **Add secret**

### Step 3: Update Workflow File

Edit `.github/workflows/azure-deploy.yml`:

```yaml
env:
  AZURE_WEBAPP_NAME: stockflowpro-api    # ← Change this to your app name
  AZURE_WEBAPP_PACKAGE_PATH: './application/backend/src/StockFlow.API'
  DOTNET_VERSION: '8.0.x'
```

### Step 4: Push to GitHub

```bash
git add .
git commit -m "Configure Azure deployment"
git push origin main
```

**The pipeline will automatically:**
- ✅ Build the .NET application
- ✅ Run tests
- ✅ Publish artifacts
- ✅ Deploy to Azure
- ✅ Run health checks

---

## ⚙️ Configuration

### Environment Variables (Azure App Service)

Configure these in Azure Portal → App Service → Configuration:

#### Connection Strings
```plaintext
DefaultConnection (SQL Azure):
Server=tcp:stockflowpro.database.windows.net,1433;Initial Catalog=StockFlowPro;User ID=admin;Password=***;Encrypt=True;

Redis (Custom):
your-redis-connection-string (optional)
```

#### Application Settings
```plaintext
ASPNETCORE_ENVIRONMENT=Production

# JWT Settings
JwtSettings__SecretKey=Your-Secret-Key-Here
JwtSettings__Issuer=StockFlowProAPI
JwtSettings__Audience=StockFlowProClient
JwtSettings__ExpiryInHours=24

# Email Settings (Gmail)
GmailSettings__SmtpServer=smtp.gmail.com
GmailSettings__SmtpPort=587
GmailSettings__SenderEmail=your-email@gmail.com
GmailSettings__SenderPassword=your-app-password
GmailSettings__SenderName=StockFlow Pro

# Pusher Settings (Notifications)
PusherSettings__AppId=your-app-id
PusherSettings__Key=your-key
PusherSettings__Secret=your-secret
PusherSettings__Cluster=ap2

# Hangfire Settings
HangfireSettings__WorkerCount=10
```

### Database Migration

After first deployment, run migrations:

```bash
# Option 1: Using Azure Cloud Shell
az webapp ssh --name stockflowpro-api --resource-group StockFlowPro-RG
cd /home/site/wwwroot
dotnet ef database update

# Option 2: From local machine (ensure firewall allows your IP)
export ConnectionStrings__DefaultConnection="Server=tcp:stockflowpro.database.windows.net,1433;..."
dotnet ef database update --project src/StockFlow.Infrastructure --startup-project src/StockFlow.API
```

---

## 🐳 Docker Deployment

### Build Docker Image

```bash
cd application/backend
docker build -t stockflowpro-api:latest .
```

### Run Locally

```bash
docker run -d \
  -p 8080:80 \
  -e ConnectionStrings__DefaultConnection="your-connection-string" \
  -e JwtSettings__SecretKey="your-jwt-secret" \
  --name stockflowpro \
  stockflowpro-api:latest
```

### Deploy to Azure Container Instances

```bash
# Login to Azure Container Registry (create one first)
az acr create --name stockflowpro --resource-group StockFlowPro-RG --sku Basic
az acr login --name stockflowpro

# Tag and push image
docker tag stockflowpro-api:latest stockflowpro.azurecr.io/stockflowpro-api:latest
docker push stockflowpro.azurecr.io/stockflowpro-api:latest

# Deploy to ACI
az container create \
  --name stockflowpro-api \
  --resource-group StockFlowPro-RG \
  --image stockflowpro.azurecr.io/stockflowpro-api:latest \
  --cpu 1 \
  --memory 1.5 \
  --port 80 \
  --dns-name-label stockflowpro \
  --environment-variables \
    ASPNETCORE_ENVIRONMENT=Production \
  --secure-environment-variables \
    ConnectionStrings__DefaultConnection="your-connection-string"
```

---

## 🆓 Free Hosting Alternatives

### Option 1: Railway.app

1. Sign up at [Railway.app](https://railway.app)
2. Click **New Project** → **Deploy from GitHub repo**
3. Select your repository
4. Railway auto-detects .NET and builds
5. Add environment variables in project settings
6. **Cost:** Free tier (500 hours/month, then ~$5/month)

### Option 2: Render.com

1. Sign up at [Render.com](https://render.com)
2. New → **Web Service**
3. Connect GitHub repository
4. Build Command: `dotnet publish -c Release -o out`
5. Start Command: `dotnet out/StockFlow.API.dll`
6. Add environment variables
7. **Cost:** Free tier (limited resources)

### Option 3: Fly.io

```bash
# Install Fly CLI
curl -L https://fly.io/install.sh | sh

# Login
fly auth login

# Deploy
fly launch
fly deploy
```

**Cost:** Free tier (3 VMs, 160GB transfer/month)

---

## 🔍 Troubleshooting

### Issue: Application fails to start

**Solution:**
1. Check logs: `az webapp log tail --name stockflowpro-api --resource-group StockFlowPro-RG`
2. Verify connection strings are set correctly
3. Ensure SQL firewall allows Azure services

### Issue: Database migration fails

**Solution:**
```bash
# Run migrations manually
az webapp ssh --name stockflowpro-api --resource-group StockFlowPro-RG
cd /home/site/wwwroot
dotnet ef database update
```

### Issue: 502 Bad Gateway

**Solution:**
- App Service is starting (wait 2-3 minutes)
- Check Application Insights for errors
- Verify .NET 8.0 runtime is selected

### Issue: Redis connection fails

**Solution:**
- Redis is optional - app uses in-memory cache as fallback
- Comment out Redis connection string in appsettings.json
- Or set up free Redis (see [Redis Setup](#redis-setup))

---

## 💰 Cost Breakdown

### Azure Resources (Basic Tier)

| Resource | SKU | Monthly Cost |
|----------|-----|--------------|
| App Service Plan | B1 | $13 |
| Azure SQL Database | Basic | $5 |
| **Total** | | **~$18/month** |

### Free Tier Alternative

| Resource | SKU | Monthly Cost |
|----------|-----|--------------|
| App Service Plan | F1 | $0 |
| Azure SQL Database | Free tier | $0 |
| **Total** | | **$0/month** |

**Note:** Free tier has limitations (1GB RAM, 1GB storage, 60 min/day compute)

---

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/yourusername/StockFlow-Pro/issues)
- **Documentation**: [Wiki](https://github.com/yourusername/StockFlow-Pro/wiki)
- **Email**: support@stockflowpro.com

---

## ✅ Post-Deployment Checklist

- [ ] Application is accessible at `https://your-app.azurewebsites.net`
- [ ] Health check returns healthy status
- [ ] Swagger UI is accessible at `/swagger`
- [ ] Database migrations are applied
- [ ] Authentication works (register/login)
- [ ] HTTPS is enforced
- [ ] Environment variables are set
- [ ] GitHub Actions pipeline is green
- [ ] Logs are being generated
- [ ] Background jobs are running (Hangfire dashboard)

---

## 🎉 Success!

Your StockFlow Pro backend is now deployed and running on Azure! 

**Next Steps:**
1. Build and deploy the frontend application
2. Configure custom domain
3. Set up Application Insights for monitoring
4. Configure backup policies
5. Set up alerts and notifications

---

**Last Updated:** April 16, 2026
