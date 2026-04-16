#!/bin/bash

# StockFlow Pro - Azure Deployment Script
# This script sets up all Azure resources for the StockFlow Pro application

set -e  # Exit on error

# ============================
# CONFIGURATION VARIABLES
# ============================
RESOURCE_GROUP="StockFlowPro-RG"
LOCATION="eastus"
APP_SERVICE_PLAN="StockFlowPro-Plan"
WEB_APP_NAME="stockflowpro-api"
SQL_SERVER_NAME="stockflowpro"
SQL_DB_NAME="StockFlowPro"
SQL_ADMIN_USER="stockflowadmin"
SKU_APP_SERVICE="B1"  # Basic tier - Change to F1 for free tier (limited resources)
SKU_SQL="Basic"       # Basic tier - $5/month

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}╔════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║  StockFlow Pro - Azure Deployment     ║${NC}"
echo -e "${GREEN}╔════════════════════════════════════════╗${NC}"
echo ""

# ============================
# STEP 1: LOGIN TO AZURE
# ============================
echo -e "${YELLOW}[1/8] Logging in to Azure...${NC}"
az login

# ============================
# STEP 2: CREATE RESOURCE GROUP
# ============================
echo -e "${YELLOW}[2/8] Creating Resource Group...${NC}"
az group create \
  --name $RESOURCE_GROUP \
  --location $LOCATION \
  --tags "Project=StockFlowPro" "Environment=Production"

# ============================
# STEP 3: CREATE APP SERVICE PLAN
# ============================
echo -e "${YELLOW}[3/8] Creating App Service Plan...${NC}"
az appservice plan create \
  --name $APP_SERVICE_PLAN \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --sku $SKU_APP_SERVICE \
  --is-linux

# ============================
# STEP 4: CREATE WEB APP
# ============================
echo -e "${YELLOW}[4/8] Creating Web App...${NC}"
az webapp create \
  --name $WEB_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --plan $APP_SERVICE_PLAN \
  --runtime "DOTNET|8.0"

# ============================
# STEP 5: CREATE SQL SERVER
# ============================
echo -e "${YELLOW}[5/8] Creating SQL Server...${NC}"
echo -e "${RED}Enter SQL Admin Password (min 8 characters, requires uppercase, lowercase, number, special char):${NC}"
read -s SQL_ADMIN_PASSWORD

az sql server create \
  --name $SQL_SERVER_NAME \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --admin-user $SQL_ADMIN_USER \
  --admin-password "$SQL_ADMIN_PASSWORD"

# ============================
# STEP 6: CONFIGURE SQL FIREWALL
# ============================
echo -e "${YELLOW}[6/8] Configuring SQL Server Firewall...${NC}"
# Allow Azure services
az sql server firewall-rule create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER_NAME \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

# Allow your current IP
MY_IP=$(curl -s https://api.ipify.org)
az sql server firewall-rule create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER_NAME \
  --name AllowMyIP \
  --start-ip-address $MY_IP \
  --end-ip-address $MY_IP

# ============================
# STEP 7: CREATE SQL DATABASE
# ============================
echo -e "${YELLOW}[7/8] Creating SQL Database...${NC}"
az sql db create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER_NAME \
  --name $SQL_DB_NAME \
  --service-objective $SKU_SQL \
  --backup-storage-redundancy Local

# ============================
# STEP 8: CONFIGURE APP SETTINGS
# ============================
echo -e "${YELLOW}[8/8] Configuring Application Settings...${NC}"

# Build connection string
CONNECTION_STRING="Server=tcp:${SQL_SERVER_NAME}.database.windows.net,1433;Initial Catalog=${SQL_DB_NAME};Persist Security Info=False;User ID=${SQL_ADMIN_USER};Password=${SQL_ADMIN_PASSWORD};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

# Set connection strings
az webapp config connection-string set \
  --name $WEB_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --connection-string-type SQLAzure \
  --settings DefaultConnection="$CONNECTION_STRING"

# Generate JWT secret
JWT_SECRET=$(openssl rand -base64 32)

# Set app settings
az webapp config appsettings set \
  --name $WEB_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --settings \
    ASPNETCORE_ENVIRONMENT="Production" \
    JwtSettings__SecretKey="$JWT_SECRET" \
    JwtSettings__Issuer="StockFlowProAPI" \
    JwtSettings__Audience="StockFlowProClient" \
    JwtSettings__ExpiryInHours="24"

# Enable HTTPS only
az webapp update \
  --name $WEB_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --https-only true

# ============================
# DEPLOYMENT COMPLETE
# ============================
echo ""
echo -e "${GREEN}╔════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║     Deployment Completed Successfully! ║${NC}"
echo -e "${GREEN}╚════════════════════════════════════════╝${NC}"
echo ""
echo -e "${YELLOW}📋 Resource Details:${NC}"
echo -e "   Resource Group: ${GREEN}$RESOURCE_GROUP${NC}"
echo -e "   Web App URL: ${GREEN}https://${WEB_APP_NAME}.azurewebsites.net${NC}"
echo -e "   SQL Server: ${GREEN}${SQL_SERVER_NAME}.database.windows.net${NC}"
echo -e "   Database: ${GREEN}$SQL_DB_NAME${NC}"
echo ""
echo -e "${YELLOW}🔐 Next Steps:${NC}"
echo "   1. Download publish profile:"
echo -e "      ${GREEN}az webapp deployment list-publishing-profiles --name $WEB_APP_NAME --resource-group $RESOURCE_GROUP --xml${NC}"
echo ""
echo "   2. Add publish profile to GitHub Secrets:"
echo "      - Go to GitHub repository Settings → Secrets → Actions"
echo "      - Create secret: AZURE_WEBAPP_PUBLISH_PROFILE"
echo "      - Paste the XML content"
echo ""
echo "   3. Update GitHub workflow file (.github/workflows/azure-deploy.yml):"
echo "      - Set AZURE_WEBAPP_NAME to: $WEB_APP_NAME"
echo ""
echo "   4. Configure additional settings in Azure Portal:"
echo "      - Email settings (Gmail SMTP)"
echo "      - Pusher settings (notifications)"
echo "      - Redis connection (optional)"
echo ""
echo -e "${YELLOW}💰 Estimated Monthly Cost: ~$18-25${NC}"
echo "   - SQL Database (Basic): ~$5/month"
echo "   - App Service (B1): ~$13/month"
echo ""
