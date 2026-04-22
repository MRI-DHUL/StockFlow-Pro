# 🚂 Railway Deployment Guide - StockFlow Pro Backend

## 🎯 Quick Deploy to Railway (5 minutes)

Railway offers **$5 free credit** monthly, perfect for development and small production apps.

---

## 📋 Prerequisites

- GitHub account with StockFlow-Pro repository
- Railway account (sign up at https://railway.app)

---

## 🚀 Step-by-Step Deployment

### 1. **Sign Up / Login to Railway**
- Go to: https://railway.app
- Click **"Start a New Project"**
- Choose **"Deploy from GitHub repo"**

### 2. **Connect GitHub Repository**
- Authorize Railway to access your GitHub
- Select **StockFlow-Pro** repository
- Railway will automatically detect the .NET application

### 3. **Configure Build Settings**

Railway will auto-detect the Dockerfile. No manual configuration needed!

The `railway.json` and `Dockerfile.railway` are already configured.

### 4. **Add Environment Variables**

In Railway Dashboard → Your Service → Variables tab, add these:

#### Required Variables:

```bash
# Database (Use Railway PostgreSQL or External SQL Server)
ConnectionStrings__DefaultConnection=Server=tcp:stockflowpro.database.windows.net,1433;Initial Catalog=StockFlowPro;User ID=Mridhul;Password=YOUR_PASSWORD;

# JWT Settings
JwtSettings__SecretKey=StockFlowPro-SecretKey-2026-ForJWT-Authentication-MustBeAtLeast32CharactersLong!
JwtSettings__Issuer=StockFlowProAPI
JwtSettings__Audience=StockFlowProClient
JwtSettings__ExpiryInHours=24

# Gmail Settings
GmailSettings__SmtpServer=smtp.gmail.com
GmailSettings__SmtpPort=587
GmailSettings__SenderEmail=mridhul35@gmail.com
GmailSettings__SenderName=StockFlow Pro
GmailSettings__SenderPassword=dcnh apuk emfh zpsx
GmailSettings__AdminEmails__0=mridhul35@gmail.com

# Pusher Settings (Real-time notifications)
PusherSettings__AppId=2142049
PusherSettings__Key=3a6db031b2381d1e78ec
PusherSettings__Secret=b66c053801c1ce1b4f44
PusherSettings__Cluster=ap2

# Redis (Optional - can use Railway Redis or leave empty)
ConnectionStrings__Redis=

# ASP.NET Core Settings
ASPNETCORE_ENVIRONMENT=Production
```

### 5. **Add Railway PostgreSQL (Optional)**

If you want to use Railway's free PostgreSQL instead of Azure SQL:

1. In your project, click **"+ New"**
2. Select **"Database"** → **"PostgreSQL"**
3. Railway will automatically create `DATABASE_URL` variable
4. Update your connection string to use PostgreSQL format

### 6. **Add Railway Redis (Optional)**

For caching:

1. Click **"+ New"** → **"Database"** → **"Redis"**
2. Railway auto-creates `REDIS_URL`
3. Use this format for `ConnectionStrings__Redis`:
   ```
   ${REDIS_URL}
   ```

### 7. **Deploy**

- Railway will automatically build and deploy
- Monitor the build logs in the **"Deployments"** tab
- Build time: ~3-5 minutes

### 8. **Get Your API URL**

After successful deployment:
- Go to **Settings** tab
- Click **"Generate Domain"**
- You'll get a URL like: `https://stockflow-pro-production.up.railway.app`

### 9. **Test Your API**

```bash
# Health check
curl https://your-app.up.railway.app/health

# Swagger UI
https://your-app.up.railway.app/swagger
```

---

## 🔄 Continuous Deployment

Railway automatically redeploys when you push to your main branch:

```bash
git add .
git commit -m "Update backend"
git push origin main
```

---

## 💰 Cost Estimation

**Railway Free Tier:**
- $5 free credit/month
- ~500 hours of execution time
- Perfect for development and testing

**Paid (if needed):**
- Pay only for what you use
- ~$5-20/month for small apps

---

## 🐛 Troubleshooting

### Build Failed?

Check Railway logs for specific errors. Common issues:

1. **Missing Dockerfile**: ✅ Fixed - `Dockerfile.railway` is configured
2. **Wrong .NET version**: ✅ Using .NET 8.0
3. **Missing environment variables**: Add all required variables above

### Can't Connect to Database?

1. Check `ConnectionStrings__DefaultConnection` is correct
2. Ensure database firewall allows Railway IPs
3. For Azure SQL: Add Railway's IP to firewall rules

### Redis Connection Failed?

Set `ConnectionStrings__Redis` to empty string - app will use in-memory cache as fallback.

---

## 📊 Monitoring

Railway provides:
- Real-time logs
- CPU/Memory usage graphs
- Request metrics
- Custom health checks

---

## 🔐 Security Best Practices

1. ✅ Never commit secrets to Git
2. ✅ Use Railway environment variables
3. ✅ Enable HTTPS (automatic on Railway)
4. ✅ Set `ASPNETCORE_ENVIRONMENT=Production`

---

## 📚 Next Steps

After deployment:

1. **Update Frontend** to use Railway URL:
   ```typescript
   // src/environments/environment.prod.ts
   apiUrl: 'https://your-app.up.railway.app/api/v1'
   ```

2. **Run Database Migrations**:
   Railway will run migrations automatically on startup

3. **Monitor Health**:
   Check `/health` endpoint regularly

---

## 🆚 Railway vs Azure Comparison

| Feature | Railway | Azure App Service |
|---------|---------|------------------|
| Setup Time | 5 min | 15 min |
| Free Tier | $5 credit/month | Limited free tier |
| Ease of Use | ⭐⭐⭐⭐⭐ Very Easy | ⭐⭐⭐ Moderate |
| Auto Deploy | ✅ Yes | ✅ Yes (with GitHub Actions) |
| Database | PostgreSQL/Redis included | Requires separate SQL DB |
| Cost (small app) | $0-10/month | $13-50/month |
| Best For | Dev/Small production | Enterprise/Large scale |

---

## ✅ Deployment Checklist

- [ ] Railway account created
- [ ] GitHub repo connected
- [ ] All environment variables added
- [ ] Database configured (Azure SQL or Railway PostgreSQL)
- [ ] Build successful
- [ ] Domain generated
- [ ] Health check passing
- [ ] Swagger UI accessible
- [ ] Frontend updated with new API URL

---

## 🎉 Success!

Your StockFlow Pro backend is now live on Railway! 🚀

**Access your API:**
- API: https://your-app.up.railway.app
- Swagger: https://your-app.up.railway.app/swagger
- Health: https://your-app.up.railway.app/health
- Hangfire: https://your-app.up.railway.app/hangfire

---

## 📞 Support

- Railway Docs: https://docs.railway.app
- Railway Discord: https://discord.gg/railway
- StockFlow Pro Issues: https://github.com/yourusername/StockFlow-Pro/issues
