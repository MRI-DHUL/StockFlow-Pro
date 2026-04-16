# 🚀 Quick Deployment Reference

## Azure Deployment (5 Minutes)

### Prerequisites
```powershell
# Check if Azure CLI is installed
az --version

# Login to Azure
az login
```

### Deploy Everything
```powershell
# Run automated deployment script
.\deploy-azure.ps1
```

---

## Manual Steps After Deployment

### 1. Get Publish Profile
```bash
az webapp deployment list-publishing-profiles \
  --name stockflowpro-api \
  --resource-group StockFlowPro-RG \
  --xml
```

### 2. Add to GitHub Secrets
1. Copy the XML output
2. Go to GitHub repo → Settings → Secrets → Actions
3. New secret: `AZURE_WEBAPP_PUBLISH_PROFILE`
4. Paste XML

### 3. Push Code
```bash
git add .
git commit -m "Deploy to Azure"
git push origin main
```

### 4. Watch Deployment
- GitHub Actions will automatically build and deploy
- Check: https://github.com/yourusername/repo/actions

---

## Verify Deployment

```bash
# Test health endpoint
curl https://stockflowpro-api.azurewebsites.net/health

# Test Swagger
curl https://stockflowpro-api.azurewebsites.net/swagger
```

---

## Common Commands

### View Logs
```bash
az webapp log tail --name stockflowpro-api --resource-group StockFlowPro-RG
```

### Restart App
```bash
az webapp restart --name stockflowpro-api --resource-group StockFlowPro-RG
```

### SSH into App
```bash
az webapp ssh --name stockflowpro-api --resource-group StockFlowPro-RG
```

### Update App Settings
```bash
az webapp config appsettings set \
  --name stockflowpro-api \
  --resource-group StockFlowPro-RG \
  --settings KEY=VALUE
```

---

## Cost

- **Development**: Free tier ($0/month)
- **Production**: Basic tier (~$18/month)
- **Enterprise**: Standard tier (~$100/month)

---

## Support

Full guide: [DEPLOYMENT.md](DEPLOYMENT.md)
