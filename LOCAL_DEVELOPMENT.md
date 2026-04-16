# 🔧 Local Development Setup

## Quick Start

### 1. Copy Template File

```powershell
# Copy the template to create your local development config
Copy-Item `
  -Path "application\backend\src\StockFlow.API\appsettings.Development.json.template" `
  -Destination "application\backend\src\StockFlow.API\appsettings.Development.json"
```

### 2. Update Your Secrets

Edit `appsettings.Development.json` with your local/development credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_DEV_SQL_CONNECTION",
    "Redis": "YOUR_DEV_REDIS_CONNECTION or leave empty for in-memory cache"
  },
  "JwtSettings": {
    "SecretKey": "YOUR_DEV_JWT_SECRET (can be anything for local dev)"
  },
  "GmailSettings": {
    "SenderEmail": "YOUR_EMAIL@gmail.com",
    "SenderPassword": "YOUR_APP_PASSWORD",
    "AdminEmails": ["YOUR_EMAIL@gmail.com"]
  },
  "PusherSettings": {
    "AppId": "YOUR_PUSHER_APP_ID",
    "Key": "YOUR_PUSHER_KEY",
    "Secret": "YOUR_PUSHER_SECRET"
  }
}
```

### 3. Run the Application

```powershell
cd application\backend\src\StockFlow.API
dotnet run
```

---

## 🔐 Security Notes

### Files Ignored by Git:
```
✓ appsettings.Development.json  # Your local secrets (git ignored)
✓ appsettings.*.json           # Any environment-specific configs
✗ appsettings.json             # Committed (no secrets)
✗ appsettings.Production.json  # Committed (no secrets)
```

### What's Safe to Commit:
- ✅ `appsettings.json` (empty secret values)
- ✅ `appsettings.Production.json` (empty secret values)
- ✅ `appsettings.Development.json.template` (template only)

### What's NEVER Committed:
- ❌ `appsettings.Development.json` (your actual secrets)
- ❌ Any file with real credentials
- ❌ `.env` files with secrets

---

## 🆚 Development vs Production

### Development (Local):
- Use `appsettings.Development.json`
- Can use the same Azure resources or local alternatives
- Secrets are in the file (gitignored)
- Run with: `dotnet run`

### Production (Azure):
- Uses `appsettings.json` + `appsettings.Production.json`
- Secrets injected from GitHub Secrets
- Deployed via GitHub Actions
- Secrets stored in Azure App Service Configuration

---

## 🔄 Alternative: User Secrets (Recommended)

Instead of `appsettings.Development.json`, you can use .NET User Secrets:

### Initialize User Secrets:
```powershell
cd application\backend\src\StockFlow.API
dotnet user-secrets init
```

### Set Secrets:
```powershell
# Connection strings
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=..."
dotnet user-secrets set "ConnectionStrings:Redis" "redis..."

# JWT
dotnet user-secrets set "JwtSettings:SecretKey" "your-secret-key"

# Gmail
dotnet user-secrets set "GmailSettings:SenderEmail" "your-email@gmail.com"
dotnet user-secrets set "GmailSettings:SenderPassword" "your-password"
dotnet user-secrets set "GmailSettings:AdminEmails:0" "admin@example.com"

# Pusher
dotnet user-secrets set "PusherSettings:AppId" "your-app-id"
dotnet user-secrets set "PusherSettings:Key" "your-key"
dotnet user-secrets set "PusherSettings:Secret" "your-secret"
```

### List All Secrets:
```powershell
dotnet user-secrets list
```

### Clear All Secrets:
```powershell
dotnet user-secrets clear
```

### Benefits:
- ✅ Secrets stored outside project directory
- ✅ Never accidentally committed
- ✅ Separate from code
- ✅ Easy to manage

---

## 🧪 Testing Without Real Services

### Use In-Memory Alternatives:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StockFlow;Trusted_Connection=True;",
    "Redis": ""  // Empty = uses in-memory cache
  },
  "GmailSettings": {
    "SenderEmail": "test@example.com",
    "SenderPassword": "not-used-in-dev"
  },
  "PusherSettings": {
    "AppId": "123",
    "Key": "test",
    "Secret": "test"
  }
}
```

---

## 🐳 Docker Development

Using Docker Compose for local services:

```yaml
# docker-compose.yml
version: '3.8'
services:
  redis:
    image: redis:alpine
    ports:
      - "6379:6379"
  
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
    ports:
      - "1433:1433"
```

Start services:
```powershell
docker-compose up -d
```

Update appsettings.Development.json:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=StockFlow;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;",
    "Redis": "localhost:6379,abortConnect=False"
  }
}
```

---

## ✅ Verification Checklist

Before running locally:

- [ ] Copied `appsettings.Development.json.template` to `appsettings.Development.json`
- [ ] Updated all secret values
- [ ] Verified `.gitignore` excludes `appsettings.Development.json`
- [ ] Can run `dotnet run` successfully
- [ ] Health check shows all services healthy

Before committing code:

- [ ] `appsettings.json` has NO secrets (empty strings)
- [ ] `appsettings.Development.json` is NOT staged for commit
- [ ] All secrets are in GitHub Secrets (for CI/CD)
- [ ] `.gitignore` is up to date

---

## 🆘 Troubleshooting

### "Configuration is missing"
→ Make sure `appsettings.Development.json` exists and has all required fields

### "SQL Connection failed"
→ Check connection string format and SQL Server is running

### "Redis connection timeout"
→ Leave Redis connection string empty to use in-memory cache

### "Gmail authentication failed"
→ Use App Password, not your regular Gmail password
→ Create at: https://myaccount.google.com/apppasswords

---

**Last Updated:** April 16, 2026
