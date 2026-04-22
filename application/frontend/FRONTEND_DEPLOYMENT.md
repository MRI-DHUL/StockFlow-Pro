# 🚀 StockFlow Pro Frontend - Deployment Guide

Complete guide for deploying the StockFlow Pro Angular frontend to production.

---

## 📋 Table of Contents
1. [Pre-Deployment Checklist](#pre-deployment-checklist)
2. [Build for Production](#build-for-production)
3. [Deployment Platforms](#deployment-platforms)
   - [Vercel (Recommended)](#vercel-recommended)
   - [Netlify](#netlify)
   - [Azure Static Web Apps](#azure-static-web-apps)
   - [Firebase Hosting](#firebase-hosting)
4. [Environment Configuration](#environment-configuration)
5. [Post-Deployment Steps](#post-deployment-steps)
6. [Troubleshooting](#troubleshooting)

---

## ✅ Pre-Deployment Checklist

### 1. **Verify Backend Connection**
Ensure the backend is deployed and accessible:
```
Backend URL: https://stockflow-pro.up.railway.app
API Endpoint: https://stockflow-pro.up.railway.app/api/v1
Health Check: https://stockflow-pro.up.railway.app/health
Swagger: https://stockflow-pro.up.railway.app/swagger
```

### 2. **Update Production Environment**
File: `src/environments/environment.prod.ts`
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://stockflow-pro.up.railway.app/api/v1',
  pusher: {
    key: '3a6db031b2381d1e78ec',
    cluster: 'ap2',
    encrypted: true,
    channel: 'stockflow-notifications'
  }
};
```

### 3. **Test Locally**
```bash
# Navigate to frontend folder
cd d:/Github/StockFlow-Pro/application/frontend

# Install dependencies
npm install

# Build for production
npm run build

# Preview production build
npx http-server dist/stockflow-app/browser -p 8080
```

### 4. **Check Build Output**
- ✅ No TypeScript errors
- ✅ No linting warnings
- ✅ Bundle size reasonable (<5MB)
- ✅ All assets included

---

## 🏗️ Build for Production

### Standard Build
```bash
npm run build
```

### Build with Configuration
```bash
# With specific configuration
ng build --configuration production

# With base href (if hosting in subdirectory)
ng build --configuration production --base-href=/stockflow/

# With optimization
ng build --configuration production --optimization --build-optimizer
```

### Build Output
```
dist/stockflow-app/
└── browser/
    ├── index.html
    ├── main.*.js
    ├── polyfills.*.js
    ├── styles.*.css
    └── assets/
```

---

## 🌐 Deployment Platforms

## Vercel (Recommended)

### Why Vercel?
- ✅ Zero configuration for Angular
- ✅ Automatic HTTPS
- ✅ Global CDN
- ✅ Environment variables support
- ✅ Free tier available

### Deployment Steps

#### Option 1: GitHub Integration (Recommended)
1. **Push to GitHub**
   ```bash
   git add .
   git commit -m "Frontend ready for deployment"
   git push origin main
   ```

2. **Connect to Vercel**
   - Visit [vercel.com](https://vercel.com)
   - Sign in with GitHub
   - Click "Import Project"
   - Select your repository

3. **Configure Build Settings**
   ```
   Framework Preset: Angular
   Build Command: npm run build
   Output Directory: dist/stockflow-app/browser
   Install Command: npm install
   Root Directory: application/frontend
   ```

4. **Set Environment Variables**
   - Go to Project Settings → Environment Variables
   - Add (if needed for build-time configuration):
     ```
     NODE_ENV=production
     ```

5. **Deploy**
   - Click "Deploy"
   - Wait 2-3 minutes
   - Access your app at `https://your-project.vercel.app`

#### Option 2: Vercel CLI
```bash
# Install Vercel CLI
npm install -g vercel

# Login
vercel login

# Navigate to frontend directory
cd application/frontend

# Deploy
vercel

# Production deployment
vercel --prod
```

### Custom Domain (Vercel)
1. Go to Project Settings → Domains
2. Add your custom domain
3. Configure DNS:
   ```
   Type: CNAME
   Name: www (or @)
   Value: cname.vercel-dns.com
   ```

---

## Netlify

### Deployment Steps

#### Option 1: Netlify CLI
```bash
# Install Netlify CLI
npm install -g netlify-cli

# Login
netlify login

# Navigate to frontend directory
cd application/frontend

# Build
npm run build

# Deploy
netlify deploy

# Production deployment
netlify deploy --prod
```

#### Option 2: GitHub Integration
1. Push to GitHub
2. Visit [netlify.com](https://netlify.com)
3. Click "New site from Git"
4. Select repository
5. Configure:
   ```
   Base directory: application/frontend
   Build command: npm run build
   Publish directory: dist/stockflow-app/browser
   ```

### netlify.toml Configuration
Create `application/frontend/netlify.toml`:
```toml
[build]
  command = "npm run build"
  publish = "dist/stockflow-app/browser"

[[redirects]]
  from = "/*"
  to = "/index.html"
  status = 200

[build.environment]
  NODE_VERSION = "20"
```

---

## Azure Static Web Apps

### Prerequisites
- Azure account
- Azure CLI installed

### Deployment Steps
```bash
# Login to Azure
az login

# Create resource group
az group create --name stockflow-rg --location eastus

# Create static web app
az staticwebapp create \
  --name stockflow-frontend \
  --resource-group stockflow-rg \
  --source https://github.com/yourusername/StockFlow-Pro \
  --location eastus2 \
  --branch main \
  --app-location "application/frontend" \
  --output-location "dist/stockflow-app/browser" \
  --build-command "npm run build"
```

### Azure Static Web Apps Configuration
Create `application/frontend/staticwebapp.config.json`:
```json
{
  "routes": [
    {
      "route": "/*",
      "serve": "/index.html",
      "statusCode": 200
    }
  ],
  "navigationFallback": {
    "rewrite": "/index.html",
    "exclude": ["/assets/*", "/*.{css,scss,js,png,gif,ico,jpg,svg}"]
  },
  "globalHeaders": {
    "content-security-policy": "default-src 'self' https:; style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; font-src 'self' https://fonts.gstatic.com"
  }
}
```

---

## Firebase Hosting

### Prerequisites
```bash
npm install -g firebase-tools
```

### Setup
```bash
# Login
firebase login

# Initialize
cd application/frontend
firebase init hosting
```

Configuration:
```
? What do you want to use as your public directory? dist/stockflow-app/browser
? Configure as a single-page app (rewrite all urls to /index.html)? Yes
? Set up automatic builds and deploys with GitHub? No
```

### Deploy
```bash
# Build
npm run build

# Deploy
firebase deploy --only hosting
```

### firebase.json
```json
{
  "hosting": {
    "public": "dist/stockflow-app/browser",
    "ignore": [
      "firebase.json",
      "**/.*",
      "**/node_modules/**"
    ],
    "rewrites": [
      {
        "source": "**",
        "destination": "/index.html"
      }
    ],
    "headers": [
      {
        "source": "**/*.@(js|css)",
        "headers": [
          {
            "key": "Cache-Control",
            "value": "max-age=31536000"
          }
        ]
      }
    ]
  }
}
```

---

## 🔧 Environment Configuration

### Multiple Environments

#### Development (environment.ts)
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api/v1',
  pusher: {
    key: '3a6db031b2381d1e78ec',
    cluster: 'ap2',
    encrypted: true,
    channel: 'stockflow-notifications'
  }
};
```

#### Staging (environment.staging.ts)
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://stockflow-staging.up.railway.app/api/v1',
  pusher: {
    key: '3a6db031b2381d1e78ec',
    cluster: 'ap2',
    encrypted: true,
    channel: 'stockflow-staging-notifications'
  }
};
```

#### Production (environment.prod.ts)
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://stockflow-pro.up.railway.app/api/v1',
  pusher: {
    key: '3a6db031b2381d1e78ec',
    cluster: 'ap2',
    encrypted: true,
    channel: 'stockflow-notifications'
  }
};
```

### Build for Specific Environment
```bash
# Development
ng build

# Staging
ng build --configuration staging

# Production
ng build --configuration production
```

---

## 🔐 Security Configuration

### CORS Setup
Ensure backend CORS allows your frontend domain:

**Backend Configuration** (ASP.NET Core):
```csharp
// In Program.cs or Startup.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200",
            "https://your-app.vercel.app",
            "https://stockflow.yourdomai.com"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

app.UseCors("AllowFrontend");
```

### Content Security Policy
Add to `index.html`:
```html
<meta http-equiv="Content-Security-Policy" 
      content="default-src 'self' https:; 
               style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; 
               font-src 'self' https://fonts.gstatic.com;
               img-src 'self' data: https:;
               script-src 'self' 'unsafe-inline' 'unsafe-eval';">
```

---

## ✅ Post-Deployment Steps

### 1. **Verify Deployment**
- [ ] Homepage loads correctly
- [ ] Login functionality works
- [ ] API calls successful
- [ ] Real-time notifications working
- [ ] All routes accessible
- [ ] Icons displaying correctly
- [ ] Styles applied properly

### 2. **Test Core Features**
```
✓ Authentication (Login/Logout)
✓ Dashboard loads with data
✓ Product list and CRUD operations
✓ Inventory management
✓ Warehouse management
✓ Order creation and tracking
✓ Real-time notifications
```

### 3. **Performance Check**
- Run Lighthouse audit
- Check bundle sizes
- Verify CDN caching
- Test load times from different locations

### 4. **Setup Monitoring**
- Configure error tracking (e.g., Sentry)
- Setup analytics (e.g., Google Analytics)
- Monitor API response times
- Track user behavior

---

## 🐛 Troubleshooting

### Issue: Blank Page on Deployment
**Solution**: Check browser console for errors. Common causes:
- Base href not set correctly
- API URL not updated in environment
- Missing index.html in output directory

### Issue: 404 on Refresh
**Solution**: Configure URL rewriting to return index.html for all routes

**Vercel**: Automatic (uses `vercel.json`)  
**Netlify**: Add `_redirects` file:
```
/*  /index.html  200
```

### Issue: API Calls Failing
**Causes**:
1. CORS not configured on backend
2. Wrong API URL in environment
3. Backend not deployed

**Solution**: Check Network tab in DevTools, verify CORS headers

### Issue: Icons Not Showing
**Solution**: Verify CDN links in `index.html`:
```html
<link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
<link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" rel="stylesheet">
```

### Issue: Styles Not Applied
**Solutions**:
1. Clear browser cache
2. Check `styles.scss` is imported in `angular.json`
3. Verify `styleUrls` in components
4. Check for CSS build errors

---

## 📊 Performance Optimization

### 1. **Enable Gzip Compression**
Most platforms enable this by default. Verify in Response Headers:
```
Content-Encoding: gzip
```

### 2. **Enable Caching**
Configure cache headers for static assets:
```
Cache-Control: max-age=31536000, immutable
```

### 3. **Lazy Loading**
Already implemented via route-level code splitting in Angular

### 4. **Bundle Analysis**
```bash
# Generate bundle stats
ng build --stats-json

# Analyze with webpack-bundle-analyzer
npx webpack-bundle-analyzer dist/stockflow-app/browser/stats.json
```

---

## 🔄 Continuous Deployment

### GitHub Actions Workflow
Create `.github/workflows/deploy.yml`:
```yaml
name: Deploy Frontend

on:
  push:
    branches: [main]
    paths:
      - 'application/frontend/**'

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '20'
          
      - name: Install dependencies
        working-directory: application/frontend
        run: npm ci
        
      - name: Build
        working-directory: application/frontend
        run: npm run build
        
      - name: Deploy to Vercel
        uses: amondnet/vercel-action@v25
        with:
          vercel-token: ${{ secrets.VERCEL_TOKEN }}
          vercel-org-id: ${{ secrets.VERCEL_ORG_ID }}
          vercel-project-id: ${{ secrets.VERCEL_PROJECT_ID }}
          working-directory: application/frontend
```

---

## 📝 Deployment Checklist

- [ ] Backend deployed and accessible
- [ ] Environment variables configured
- [ ] Production build tested locally
- [ ] CORS configured on backend
- [ ] Domain/subdomain configured (if custom)
- [ ] SSL certificate active (HTTPS)
- [ ] URL rewriting configured for SPA
- [ ] All routes tested
- [ ] API integration verified
- [ ] Real-time features tested
- [ ] Performance audit completed
- [ ] Error monitoring setup
- [ ] Analytics configured

---

## 🎯 Quick Commands Reference

```bash
# Development
npm start                          # Start dev server
npm run build                      # Build for production
npm run test                       # Run tests
npm run lint                       # Lint code

# Deployment
vercel                             # Deploy to Vercel
netlify deploy                     # Deploy to Netlify
firebase deploy --only hosting     # Deploy to Firebase

# Verification
npx http-server dist/stockflow-app/browser  # Preview build locally
```

---

## 📚 Additional Resources

- [Angular Deployment Guide](https://angular.io/guide/deployment)
- [Vercel Documentation](https://vercel.com/docs)
- [Netlify Documentation](https://docs.netlify.com)
- [Azure Static Web Apps Docs](https://docs.microsoft.com/azure/static-web-apps)
- [Firebase Hosting Docs](https://firebase.google.com/docs/hosting)

---

## 🆘 Support

For deployment issues:
1. Check the troubleshooting section above
2. Review platform-specific documentation
3. Check browser console and network tab
4. Verify backend connectivity
5. Review build logs for errors

---

**Deployment Status**: ✅ Ready  
**Last Updated**: April 22, 2026  
**Version**: 1.0  
**Recommended Platform**: Vercel
