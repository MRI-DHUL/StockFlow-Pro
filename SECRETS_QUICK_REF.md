# 🎯 GitHub Secrets - Quick Reference Card

## Add These 10 Secrets to GitHub

**Go to:** GitHub Repository → Settings → Secrets and variables → Actions → New repository secret

---

## 📋 Secrets Checklist Table

| ✓ | Secret Name | Value to Copy | Notes |
|---|-------------|---------------|-------|
| ⬜ | `AZURE_WEBAPP_PUBLISH_PROFILE` | Run command below to get XML | Paste entire XML output |
| ⬜ | `SQL_CONNECTION_STRING` | `Server=tcp:stockflowpro.database.windows.net,1433;Initial Catalog=StockFlow Pro;Persist Security Info=False;User ID=Mridhul;Password=Vinu020403$;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;` | Full SQL connection string |
| ⬜ | `REDIS_CONNECTION_STRING` | `stockflowredis.redis.cache.windows.net:6380,password=9CU5dMTwuSPBQ207n2zUPyWy3eNHqc1kGAzCaPMPPB4=,ssl=True,abortConnect=False` | Redis with SSL |
| ⬜ | `JWT_SECRET_KEY` | `StockFlowPro-SecretKey-2026-ForJWT-Authentication-MustBeAtLeast32CharactersLong!` | Min 32 characters |
| ⬜ | `GMAIL_SENDER_EMAIL` | `mridhul35@gmail.com` | Gmail address |
| ⬜ | `GMAIL_SENDER_PASSWORD` | `dcnh apuk emfh zpsx` | App password |
| ⬜ | `GMAIL_ADMIN_EMAILS` | `["mridhul35@gmail.com","admin@stockflowpro.com"]` | ⚠️ Must be valid JSON |
| ⬜ | `PUSHER_APP_ID` | `2142049` | Numeric only |
| ⬜ | `PUSHER_KEY` | `3a6db031b2381d1e78ec` | Pusher key |
| ⬜ | `PUSHER_SECRET` | `b66c053801c1ce1b4f44` | Pusher secret |

---

## 📌 How to Add Each Secret

### For `AZURE_WEBAPP_PUBLISH_PROFILE`:
```powershell
az webapp deployment list-publishing-profiles --name stockflowpro-api --resource-group StockFlow_Pro --xml
```
Copy the **entire XML output** and paste it as the secret value.

### For All Other Secrets:
1. Click **"New repository secret"**
2. **Name:** Copy exact name from table (e.g., `SQL_CONNECTION_STRING`)
3. **Value:** Copy entire value from "Value to Copy" column
4. Click **"Add secret"**
5. Check the ✓ box in this table once added

---

## ⚠️ Important Notes

| Secret | ⚠️ Special Instructions |
|--------|------------------------|
| `GMAIL_ADMIN_EMAILS` | **MUST** be valid JSON array: `["email1","email2"]` with quotes |
| `AZURE_WEBAPP_PUBLISH_PROFILE` | Use PowerShell command to get XML, paste **entire output** |
| All others | Copy exact value from table above |

---

## ✅ Verification

After adding all 10 secrets, your GitHub Secrets page should show:

| Status | Secret Name |
|--------|-------------|
| 🟢 | AZURE_WEBAPP_PUBLISH_PROFILE |
| 🟢 | SQL_CONNECTION_STRING |
| 🟢 | REDIS_CONNECTION_STRING |
| 🟢 | JWT_SECRET_KEY |
| 🟢 | GMAIL_SENDER_EMAIL |
| 🟢 | GMAIL_SENDER_PASSWORD |
| 🟢 | GMAIL_ADMIN_EMAILS |
| 🟢 | PUSHER_APP_ID |
| 🟢 | PUSHER_KEY |
| 🟢 | PUSHER_SECRET |

**Total: 10 secrets** ✅

---

## 🚀 Deploy

Once all secrets are added:

```bash
git add .
git commit -m "Security: Move secrets to GitHub Secrets"
git push origin main
```

**GitHub Actions will automatically deploy with your secrets!** 🎉

---

**📖 Need detailed help?** See [GITHUB_SECRETS.md](GITHUB_SECRETS.md)
