# 🔴 Redis Setup Guide - StockFlow Pro

## Current Issue
Your Redis connection is failing with authentication error. The credentials in your config appear to be invalid or expired.

---

## ✅ Solution 1: Free Redis Cloud (Recommended)

### Why This?
- ✅ 30MB storage free forever
- ✅ Fully managed, no maintenance
- ✅ High availability
- ✅ Perfect for small to medium apps

### Steps:

#### 1. Create Account
- Go to: https://redis.com/try-free/
- Click **"Get Started Free"**
- Sign up with email or Google

#### 2. Create Database
1. After login, click **"Create database"**
2. Choose **"Free"** tier
3. Select region closest to you (e.g., US East)
4. Click **"Activate"**

#### 3. Get Connection Details
1. Click on your database name
2. You'll see:
   ```
   Endpoint: redis-xxxxx.c123.us-east-1-3.ec2.cloud.redislabs.com
   Port: 12345
   Password: [click to reveal]
   ```

#### 4. Build Connection String
Format:
```
{endpoint}:{port},password={password},ssl=True,abortConnect=False
```

Example:
```
redis-12345.c123.us-east-1-3.ec2.cloud.redislabs.com:12345,password=Abc123XyzSecret,ssl=True,abortConnect=False
```

#### 5. Update appsettings.json
```json
{
  "ConnectionStrings": {
    "Redis": "redis-12345.c123.us-east-1-3.ec2.cloud.redislabs.com:12345,password=YourPassword,ssl=True,abortConnect=False"
  }
}
```

#### 6. Test
```powershell
dotnet run
```

Check health endpoint - Redis should be **Healthy**!

---

## ✅ Solution 2: Upstash (Alternative Free Option)

### Why This?
- ✅ 10,000 commands/day free
- ✅ 256MB storage
- ✅ Global edge caching
- ✅ Serverless Redis

### Steps:

#### 1. Create Account
- Go to: https://upstash.com/
- Click **"Get Started"**
- Sign up (can use GitHub login)

#### 2. Create Database
1. Click **"Create Database"**
2. Name: `stockflow-cache`
3. Type: **Regional** (free)
4. Region: Choose closest to you
5. Click **"Create"**

#### 3. Get Connection String
1. Click on your database
2. Scroll to **"StackExchange.Redis"** section
3. Copy the connection string

It looks like:
```
lucky-xxx-12345.upstash.io:12345,password=AaaBbbCccDdd,ssl=True
```

#### 4. Update appsettings.json
```json
{
  "ConnectionStrings": {
    "Redis": "lucky-xxx-12345.upstash.io:12345,password=AaaBbbCccDdd,ssl=True,abortConnect=False"
  }
}
```

#### 5. Test
```powershell
dotnet run
```

---

## ✅ Solution 3: Local Redis (Development Only)

### Option A: Docker (Easiest)

#### 1. Install Docker Desktop
Download from: https://www.docker.com/products/docker-desktop/

#### 2. Run Redis Container
```powershell
# Pull Redis image
docker pull redis:alpine

# Run Redis
docker run -d --name redis-dev -p 6379:6379 redis:alpine

# Verify it's running
docker ps
```

#### 3. Update appsettings.json
```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379,abortConnect=False"
  }
}
```

#### 4. Manage Container
```powershell
# Stop Redis
docker stop redis-dev

# Start Redis
docker start redis-dev

# Remove container (when done)
docker rm -f redis-dev
```

---

### Option B: WSL (Windows Subsystem for Linux)

#### 1. Open WSL Terminal
```powershell
wsl
```

#### 2. Install Redis
```bash
# Update package list
sudo apt-get update

# Install Redis
sudo apt-get install redis-server -y

# Start Redis
sudo service redis-server start

# Test connection
redis-cli ping
# Should return: PONG
```

#### 3. Auto-start Redis on WSL Startup
```bash
# Edit bashrc
echo "sudo service redis-server start" >> ~/.bashrc
```

#### 4. Update appsettings.json
```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379,abortConnect=False"
  }
}
```

---

### Option C: Native Windows (Advanced)

#### 1. Download Redis
- Go to: https://github.com/microsoftarchive/redis/releases
- Download: `Redis-x64-xxx.msi`
- Install

#### 2. Start Redis Service
```powershell
# Start service
net start Redis

# Or run manually
redis-server
```

#### 3. Update appsettings.json
```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379,abortConnect=False"
  }
}
```

---

## 🧪 Testing Your Redis Connection

### Method 1: Health Check
```powershell
# Start your API
cd D:\Github\StockFlow-Pro\application\backend\src\StockFlow.API
dotnet run

# In another terminal, check health
(Invoke-WebRequest -Uri http://localhost:5057/health -UseBasicParsing).Content | ConvertFrom-Json
```

Look for:
```json
{
  "redis": {
    "status": "Healthy"  // ← Should be Healthy, not Degraded
  }
}
```

### Method 2: Redis CLI (Local Only)
```bash
# Connect to Redis
redis-cli

# Test commands
SET test "Hello Redis"
GET test
# Should return: "Hello Redis"

# Exit
exit
```

---

## 🔍 Troubleshooting

### Error: "Authentication failed"
**Cause:** Wrong password or expired credentials

**Fix:**
1. Re-check password in Redis Cloud/Upstash dashboard
2. Make sure you're using the exact password (no spaces)
3. For Redis Cloud, click "Reveal password" and copy carefully

### Error: "Connection timeout"
**Cause:** Firewall or wrong endpoint

**Fix:**
1. Check endpoint format: `hostname:port`
2. Verify SSL setting matches your provider
3. Check if your IP is allowed (Redis Cloud firewall settings)

### Error: "No connection is available"
**Cause:** Redis not running (local) or wrong host

**Fix:**
```powershell
# For Docker
docker ps  # Check if container is running
docker logs redis-dev  # Check container logs

# For WSL
wsl
sudo service redis-server status
sudo service redis-server start

# For cloud, verify endpoint is correct
```

### Still Not Working?
1. **Check connection string format carefully:**
   ```
   hostname:port,password=xxx,ssl=True,abortConnect=False
   ```

2. **Test with Redis GUI tool:**
   - Download: [RedisInsight](https://redis.com/redis-enterprise/redis-insight/)
   - Connect using your credentials
   - If GUI can't connect, credentials are wrong

3. **Enable detailed logging:**
   ```json
   "Serilog": {
     "MinimumLevel": {
       "Override": {
         "StackExchange.Redis": "Debug"
       }
     }
   }
   ```

---

## 📊 Connection String Format Reference

### Format:
```
{endpoint}:{port}[,password={password}][,ssl={true|false}][,abortConnect=False]
```

### Examples:

**Redis Cloud:**
```
redis-12345.c123.us-east-1-3.ec2.cloud.redislabs.com:12345,password=MySecret123,ssl=True,abortConnect=False
```

**Upstash:**
```
steady-lemur-12345.upstash.io:12345,password=AbCdEf123456,ssl=True,abortConnect=False
```

**Azure Redis Cache:**
```
myredis.redis.cache.windows.net:6380,password=MyKey==,ssl=True,abortConnect=False
```

**Local (no password):**
```
localhost:6379,abortConnect=False
```

**Local (with password):**
```
localhost:6379,password=mypassword,abortConnect=False
```

---

## 💡 Recommendations

### For Development:
**Use:** Local Docker Redis
- Free
- Fast
- No internet required
- Full control

### For Staging/Testing:
**Use:** Upstash Free Tier
- Free 10K commands/day
- Managed service
- Good for testing

### For Production:
**Use:** Redis Cloud or Azure Redis Cache
- High availability
- Backups
- Monitoring
- Scaling
- Support

---

## ✅ Quick Setup Commands

### Fastest Option (Docker):
```powershell
# One-time setup
docker pull redis:alpine
docker run -d --name redis-dev -p 6379:6379 redis:alpine
```

Update `appsettings.json`:
```json
"Redis": "localhost:6379,abortConnect=False"
```

Done! 🎉

---

## 📞 Need Help?

If you're still having issues:

1. **Check the error message** in terminal when running `dotnet run`
2. **Verify your connection string** matches one of the formats above
3. **Test the connection** using RedisInsight GUI tool
4. **Use local Redis** for development (easiest to troubleshoot)

---

**Last Updated:** April 16, 2026
