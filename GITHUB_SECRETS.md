# 🔐 GitHub Secrets Configuration Guide

## Required GitHub Secrets

Add these secrets to your GitHub repository:
**Settings → Secrets and variables → Actions → New repository secret**

---

## 📋 Complete List of Secrets

### 1. **AZURE_WEBAPP_PUBLISH_PROFILE**
```xml
<!-- Get this from Azure CLI or Portal -->
az webapp deployment list-publishing-profiles \
  --name stockflowpro-api \
  --resource-group StockFlow_Pro \
  --xml
```
**Paste the entire XML output**

---

### 2. **SQL_CONNECTION_STRING**
```
Server=tcp:stockflowpro.database.windows.net,1433;Initial Catalog=StockFlow Pro;Persist Security Info=False;User ID=Mridhul;Password=Vinu020403$;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

---

### 3. **REDIS_CONNECTION_STRING**
```
stockflowredis.redis.cache.windows.net:6380,password=9CU5dMTwuSPBQ207n2zUPyWy3eNHqc1kGAzCaPMPPB4=,ssl=True,abortConnect=False
```

---

### 4. **JWT_SECRET_KEY**
```
StockFlowPro-SecretKey-2026-ForJWT-Authentication-MustBeAtLeast32CharactersLong!
```
**⚠️ Generate a new one for production:**
```powershell
# PowerShell - Generate secure random key
-join ((48..57) + (65..90) + (97..122) | Get-Random -Count 64 | ForEach-Object {[char]$_})
```

---

### 5. **GMAIL_SENDER_EMAIL**
```
mridhul35@gmail.com
```

---

### 6. **GMAIL_SENDER_PASSWORD**
```
dcnh apuk emfh zpsx
```
**⚠️ This is an App Password, not your Gmail password**
**Create at:** https://myaccount.google.com/apppasswords

---

### 7. **GMAIL_ADMIN_EMAILS**
```json
["mridhul35@gmail.com","admin@stockflowpro.com"]
```
**Format:** JSON array as a single line

---

### 8. **PUSHER_APP_ID**
```
2142049
```

---

### 9. **PUSHER_KEY**
```
3a6db031b2381d1e78ec
```

---

### 10. **PUSHER_SECRET**
```
b66c053801c1ce1b4f44
```

---

## 🚀 How to Add Secrets to GitHub

### Via GitHub Web UI:

1. **Go to your repository** on GitHub
2. Click **Settings** (top navigation)
3. Click **Secrets and variables** → **Actions** (left sidebar)
4. Click **New repository secret**
5. Add each secret:
   - **Name:** Exact name from above (e.g., `SQL_CONNECTION_STRING`)
   - **Value:** The corresponding value
   - Click **Add secret**
6. Repeat for all 10 secrets

### Via GitHub CLI (Alternative):

```bash
# Install GitHub CLI first: https://cli.github.com/

# Login
gh auth login

# Add secrets (run each line)
gh secret set AZURE_WEBAPP_PUBLISH_PROFILE < publish-profile.xml
gh secret set SQL_CONNECTION_STRING -b "Server=tcp:stockflowpro..."
gh secret set REDIS_CONNECTION_STRING -b "stockflowredis.redis..."
gh secret set JWT_SECRET_KEY -b "YourNewSecretKey..."
gh secret set GMAIL_SENDER_EMAIL -b "your-email@gmail.com"
gh secret set GMAIL_SENDER_PASSWORD -b "your-app-password"
gh secret set GMAIL_ADMIN_EMAILS -b '["admin@example.com"]'
gh secret set PUSHER_APP_ID -b "your-app-id"
gh secret set PUSHER_KEY -b "your-key"
gh secret set PUSHER_SECRET -b "your-secret"
```

---

## ✅ Verification

After adding all secrets, go to:
**Settings → Secrets and variables → Actions**

You should see:
```
✓ AZURE_WEBAPP_PUBLISH_PROFILE
✓ SQL_CONNECTION_STRING
✓ REDIS_CONNECTION_STRING
✓ JWT_SECRET_KEY
✓ GMAIL_SENDER_EMAIL
✓ GMAIL_SENDER_PASSWORD
✓ GMAIL_ADMIN_EMAILS
✓ PUSHER_APP_ID
✓ PUSHER_KEY
✓ PUSHER_SECRET
```

**Total: 10 secrets**

---

## 🔒 Security Best Practices

### Do NOT:
- ❌ Commit secrets to Git
- ❌ Share secrets in Slack/Teams/Email
- ❌ Use production secrets in development
- ❌ Reuse the same JWT key across environments

### Do:
- ✅ Use different secrets for dev/staging/production
- ✅ Rotate secrets regularly (every 90 days)
- ✅ Use Azure Key Vault for production (recommended)
- ✅ Enable 2FA on GitHub account
- ✅ Limit access to repository secrets

---

## 🔄 Rotating Secrets

### JWT Secret Key:
```powershell
# Generate new key
$newKey = -join ((48..57) + (65..90) + (97..122) | Get-Random -Count 64 | ForEach-Object {[char]$_})
Write-Host $newKey

# Update GitHub secret
gh secret set JWT_SECRET_KEY -b "$newKey"
```

### SQL Database Password:
```bash
# Change in Azure Portal or CLI
az sql server update \
  --name stockflowpro \
  --resource-group StockFlow_Pro \
  --admin-password "NewSecurePassword123!"

# Update GitHub secret with new connection string
gh secret set SQL_CONNECTION_STRING -b "Server=tcp:...;Password=NewSecurePassword123!;..."
```

### Redis Key:
```bash
# Regenerate in Azure Portal or CLI
az redis regenerate-keys \
  --name stockflowredis \
  --resource-group StockFlow_Pro \
  --key-type Primary

# Get new key
az redis list-keys --name stockflowredis --resource-group StockFlow_Pro

# Update GitHub secret
gh secret set REDIS_CONNECTION_STRING -b "stockflowredis...password=NEW_KEY..."
```

---

## 📝 Current Values (For Reference)

**⚠️ DELETE THIS SECTION AFTER MIGRATION TO GITHUB SECRETS**

```yaml
# Current configuration (to be removed from code)
SQL_CONNECTION_STRING: "Server=tcp:stockflowpro.database.windows.net,1433;Initial Catalog=StockFlow Pro;User ID=Mridhul;Password=Vinu020403$;..."
REDIS_CONNECTION_STRING: "stockflowredis.redis.cache.windows.net:6380,password=9CU5dMTwuSPBQ207n2zUPyWy3eNHqc1kGAzCaPMPPB4=,..."
JWT_SECRET_KEY: "StockFlowPro-SecretKey-2026-ForJWT-Authentication-MustBeAtLeast32CharactersLong!"
GMAIL_SENDER_EMAIL: "mridhul35@gmail.com"
GMAIL_SENDER_PASSWORD: "dcnh apuk emfh zpsx"
GMAIL_ADMIN_EMAILS: ["mridhul35@gmail.com", "admin@stockflowpro.com"]
PUSHER_APP_ID: "2142049"
PUSHER_KEY: "3a6db031b2381d1e78ec"
PUSHER_SECRET: "b66c053801c1ce1b4f44"
```

---

## 🎯 Next Steps

1. ✅ Add all secrets to GitHub
2. ✅ Verify all secrets are listed
3. ✅ Push updated code (secrets removed from appsettings.json)
4. ✅ Test deployment pipeline
5. ✅ Delete this reference section

---

**Last Updated:** April 16, 2026
