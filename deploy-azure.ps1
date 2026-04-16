# StockFlow Pro - Azure Deployment Script (PowerShell)
# This script sets up all Azure resources for the StockFlow Pro application

# ============================
# CONFIGURATION VARIABLES
# ============================
$RESOURCE_GROUP = "StockFlowPro-RG"
$LOCATION = "eastus"
$APP_SERVICE_PLAN = "StockFlowPro-Plan"
$WEB_APP_NAME = "stockflowpro-api"
$SQL_SERVER_NAME = "stockflowpro"
$SQL_DB_NAME = "StockFlowPro"
$SQL_ADMIN_USER = "stockflowadmin"
$SKU_APP_SERVICE = "B1"  # Basic tier - Change to F1 for free tier (limited resources)
$SKU_SQL = "Basic"       # Basic tier - $5/month

Write-Host "╔════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║  StockFlow Pro - Azure Deployment     ║" -ForegroundColor Green
Write-Host "╚════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""

# ============================
# STEP 1: LOGIN TO AZURE
# ============================
Write-Host "[1/8] Logging in to Azure..." -ForegroundColor Yellow
az login

# ============================
# STEP 2: CREATE RESOURCE GROUP
# ============================
Write-Host "[2/8] Creating Resource Group..." -ForegroundColor Yellow
az group create `
  --name $RESOURCE_GROUP `
  --location $LOCATION `
  --tags "Project=StockFlowPro" "Environment=Production"

# ============================
# STEP 3: CREATE APP SERVICE PLAN
# ============================
Write-Host "[3/8] Creating App Service Plan..." -ForegroundColor Yellow
az appservice plan create `
  --name $APP_SERVICE_PLAN `
  --resource-group $RESOURCE_GROUP `
  --location $LOCATION `
  --sku $SKU_APP_SERVICE `
  --is-linux

# ============================
# STEP 4: CREATE WEB APP
# ============================
Write-Host "[4/8] Creating Web App..." -ForegroundColor Yellow
az webapp create `
  --name $WEB_APP_NAME `
  --resource-group $RESOURCE_GROUP `
  --plan $APP_SERVICE_PLAN `
  --runtime "DOTNET:8.0"

# ============================
# STEP 5: CREATE SQL SERVER
# ============================
Write-Host "[5/8] Creating SQL Server..." -ForegroundColor Yellow
$SQL_ADMIN_PASSWORD = Read-Host "Enter SQL Admin Password (min 8 characters, requires uppercase, lowercase, number, special char)" -AsSecureString
$SQL_ADMIN_PASSWORD_PLAIN = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($SQL_ADMIN_PASSWORD))

az sql server create `
  --name $SQL_SERVER_NAME `
  --resource-group $RESOURCE_GROUP `
  --location $LOCATION `
  --admin-user $SQL_ADMIN_USER `
  --admin-password "$SQL_ADMIN_PASSWORD_PLAIN"

# ============================
# STEP 6: CONFIGURE SQL FIREWALL
# ============================
Write-Host "[6/8] Configuring SQL Server Firewall..." -ForegroundColor Yellow

# Allow Azure services
az sql server firewall-rule create `
  --resource-group $RESOURCE_GROUP `
  --server $SQL_SERVER_NAME `
  --name AllowAzureServices `
  --start-ip-address 0.0.0.0 `
  --end-ip-address 0.0.0.0

# Allow your current IP
$MY_IP = (Invoke-WebRequest -Uri "https://api.ipify.org" -UseBasicParsing).Content
az sql server firewall-rule create `
  --resource-group $RESOURCE_GROUP `
  --server $SQL_SERVER_NAME `
  --name AllowMyIP `
  --start-ip-address $MY_IP `
  --end-ip-address $MY_IP

# ============================
# STEP 7: CREATE SQL DATABASE
# ============================
Write-Host "[7/8] Creating SQL Database..." -ForegroundColor Yellow
az sql db create `
  --resource-group $RESOURCE_GROUP `
  --server $SQL_SERVER_NAME `
  --name $SQL_DB_NAME `
  --service-objective $SKU_SQL `
  --backup-storage-redundancy Local

# ============================
# STEP 8: CONFIGURE APP SETTINGS
# ============================
Write-Host "[8/8] Configuring Application Settings..." -ForegroundColor Yellow

# Build connection string
$CONNECTION_STRING = "Server=tcp:${SQL_SERVER_NAME}.database.windows.net,1433;Initial Catalog=${SQL_DB_NAME};Persist Security Info=False;User ID=${SQL_ADMIN_USER};Password=${SQL_ADMIN_PASSWORD_PLAIN};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

# Set connection strings
az webapp config connection-string set `
  --name $WEB_APP_NAME `
  --resource-group $RESOURCE_GROUP `
  --connection-string-type SQLAzure `
  --settings DefaultConnection="$CONNECTION_STRING"

# Generate JWT secret
$JWT_SECRET = -join ((48..57) + (65..90) + (97..122) | Get-Random -Count 32 | ForEach-Object {[char]$_})

# Set app settings
az webapp config appsettings set `
  --name $WEB_APP_NAME `
  --resource-group $RESOURCE_GROUP `
  --settings `
    ASPNETCORE_ENVIRONMENT="Production" `
    JwtSettings__SecretKey="$JWT_SECRET" `
    JwtSettings__Issuer="StockFlowProAPI" `
    JwtSettings__Audience="StockFlowProClient" `
    JwtSettings__ExpiryInHours="24"

# Enable HTTPS only
az webapp update `
  --name $WEB_APP_NAME `
  --resource-group $RESOURCE_GROUP `
  --https-only true

# ============================
# DEPLOYMENT COMPLETE
# ============================
Write-Host ""
Write-Host "╔════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║     Deployment Completed Successfully! ║" -ForegroundColor Green
Write-Host "╚════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Resource Details:" -ForegroundColor Yellow
Write-Host "   Resource Group: $RESOURCE_GROUP" -ForegroundColor Green
Write-Host "   Web App URL: https://${WEB_APP_NAME}.azurewebsites.net" -ForegroundColor Green
Write-Host "   SQL Server: ${SQL_SERVER_NAME}.database.windows.net" -ForegroundColor Green
Write-Host "   Database: $SQL_DB_NAME" -ForegroundColor Green
Write-Host ""
Write-Host "🔐 Next Steps:" -ForegroundColor Yellow
Write-Host "   1. Download publish profile:"
Write-Host "      az webapp deployment list-publishing-profiles --name $WEB_APP_NAME --resource-group $RESOURCE_GROUP --xml" -ForegroundColor Green
Write-Host ""
Write-Host "   2. Add publish profile to GitHub Secrets:"
Write-Host "      - Go to GitHub repository Settings → Secrets → Actions"
Write-Host "      - Create secret: AZURE_WEBAPP_PUBLISH_PROFILE"
Write-Host "      - Paste the XML content"
Write-Host ""
Write-Host "   3. Update GitHub workflow file (.github/workflows/azure-deploy.yml):"
Write-Host "      - Set AZURE_WEBAPP_NAME to: $WEB_APP_NAME"
Write-Host ""
Write-Host "   4. Configure additional settings in Azure Portal:"
Write-Host "      - Email settings (Gmail SMTP)"
Write-Host "      - Pusher settings (notifications)"
Write-Host "      - Redis connection (optional)"
Write-Host ""
Write-Host "💰 Estimated Monthly Cost: ~`$18-25" -ForegroundColor Yellow
Write-Host "   - SQL Database (Basic): ~`$5/month"
Write-Host "   - App Service (B1): ~`$13/month"
Write-Host ""
