# 🚀 Frontend Deployment Guide

Deploy StockFlow Pro frontend to various hosting platforms.

## 📋 Prerequisites

- Frontend built and tested locally
- Backend already deployed at: https://stockflow-pro.up.railway.app
- Node.js 18+ and npm installed

## 🏗️ Build for Production

```bash
cd application/frontend

# Production build
npm run build

# Output will be in dist/stockflow-app/
```

Build configuration uses `environment.prod.ts` which points to the deployed backend.

## 🌐 Deployment Options

### Option 1: Vercel (Recommended) ⭐

**Why Vercel?**
- Zero config for Angular
- Automatic HTTPS
- Global CDN
- Free tier available

**Steps:**

1. **Install Vercel CLI**
```bash
npm install -g vercel
```

2. **Deploy**
```bash
cd application/frontend
vercel
```

3. **Configure** (vercel.json)
```json
{
  "version": 2,
  "builds": [
    {
      "src": "package.json",
      "use": "@vercel/static-build",
      "config": {
        "distDir": "dist/stockflow-app/browser"
      }
    }
  ],
  "routes": [
    {
      "src": "/(.*)",
      "dest": "/index.html"
    }
  ]
}
```

4. **Environment Variables**
Add in Vercel dashboard (optional, using defaults):
- `VITE_API_URL`: https://stockflow-pro.up.railway.app/api/v1

---

### Option 2: Netlify

**Steps:**

1. **Build**
```bash
npm run build
```

2. **Deploy via Netlify CLI**
```bash
npm install -g netlify-cli
netlify deploy --prod
```

3. **Configure** (netlify.toml)
```toml
[build]
  command = "npm run build"
  publish = "dist/stockflow-app/browser"

[[redirects]]
  from = "/*"
  to = "/index.html"
  status = 200
```

---

### Option 3: Firebase Hosting

**Steps:**

1. **Install Firebase CLI**
```bash
npm install -g firebase-tools
firebase login
```

2. **Initialize**
```bash
cd application/frontend
firebase init hosting
```

3. **Configure** (firebase.json)
```json
{
  "hosting": {
    "public": "dist/stockflow-app/browser",
    "ignore": ["firebase.json", "**/.*", "**/node_modules/**"],
    "rewrites": [
      {
        "source": "**",
        "destination": "/index.html"
      }
    ]
  }
}
```

4. **Deploy**
```bash
firebase deploy
```

---

### Option 4: Azure Static Web Apps

**Steps:**

1. **Install Azure CLI**
```bash
npm install -g @azure/static-web-apps-cli
```

2. **Create Static Web App**
```bash
az staticwebapp create \
  --name stockflow-pro-frontend \
  --resource-group your-resource-group \
  --location eastus2
```

3. **Deploy**
```bash
swa deploy ./dist/stockflow-app/browser \
  --deployment-token <your-token>
```

---

### Option 5: GitHub Pages

**Steps:**

1. **Install gh-pages**
```bash
npm install --save-dev angular-cli-ghpages
```

2. **Build with base href**
```bash
ng build --base-href "https://yourusername.github.io/stockflow-pro/"
```

3. **Deploy**
```bash
npx angular-cli-ghpages --dir=dist/stockflow-app/browser
```

---

### Option 6: AWS S3 + CloudFront

**Steps:**

1. **Build**
```bash
npm run build
```

2. **Create S3 Bucket**
```bash
aws s3 mb s3://stockflow-pro-frontend
```

3. **Upload**
```bash
aws s3 sync dist/stockflow-app/browser s3://stockflow-pro-frontend
```

4. **Configure S3 for static hosting**
```bash
aws s3 website s3://stockflow-pro-frontend \
  --index-document index.html \
  --error-document index.html
```

5. **Create CloudFront Distribution**
- Origin: S3 bucket
- Viewer Protocol: Redirect HTTP to HTTPS
- Default Root Object: index.html

---

### Option 7: Docker + Any Platform

**Dockerfile:**
```dockerfile
# Build stage
FROM node:18-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

# Production stage
FROM nginx:alpine
COPY --from=build /app/dist/stockflow-app/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

**nginx.conf:**
```nginx
events {
  worker_connections 1024;
}

http {
  include /etc/nginx/mime.types;
  default_type application/octet-stream;

  server {
    listen 80;
    server_name localhost;
    root /usr/share/nginx/html;
    index index.html;

    location / {
      try_files $uri $uri/ /index.html;
    }

    gzip on;
    gzip_types text/plain text/css application/json application/javascript text/xml application/xml application/xml+rss text/javascript;
  }
}
```

**Build & Deploy:**
```bash
docker build -t stockflow-frontend .
docker run -p 80:80 stockflow-frontend
```

---

## ✅ Post-Deployment Checklist

1. **Test the deployed URL**
   - Login functionality
   - API connectivity
   - Real-time notifications

2. **Verify backend connection**
   ```bash
   curl https://your-deployed-frontend.com/api/health
   ```

3. **Check browser console**
   - No CORS errors
   - No 404s for assets

4. **Test on different devices**
   - Desktop browser
   - Mobile browser
   - Tablet

5. **Performance audit**
   ```bash
   npm install -g lighthouse
   lighthouse https://your-deployed-frontend.com
   ```

## 🔧 Environment Variables

All platforms support environment variables. Set these if needed:

```bash
API_URL=https://stockflow-pro.up.railway.app/api/v1
PUSHER_KEY=3a6db031b2381d1e78ec
PUSHER_CLUSTER=ap2
```

## 🌍 Custom Domain

After deployment, configure your custom domain:

1. Add CNAME record: `www.yourdomain.com` → `your-deployment.vercel.app`
2. Add A record: `yourdomain.com` → Platform IP
3. Enable HTTPS (most platforms auto-configure Let's Encrypt)

## 📊 Recommended: Vercel Deployment

**Quick Deploy:**
```bash
cd application/frontend
npm install -g vercel
vercel --prod
```

**Result:**
- ✅ Deployed in seconds
- ✅ Automatic HTTPS
- ✅ Global CDN
- ✅ Automatic deployments from Git
- ✅ Preview deployments for PRs

**Live URL Example:**
`https://stockflow-pro-frontend.vercel.app`

## 🎯 Performance Optimization

### Enable Gzip/Brotli
Most platforms enable this by default. For nginx:
```nginx
gzip on;
gzip_comp_level 6;
gzip_types text/plain text/css application/json application/javascript;
```

### Enable Caching
```nginx
location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf)$ {
  expires 1y;
  add_header Cache-Control "public, immutable";
}
```

### CDN Configuration
- Enable CDN on your platform
- Configure cache rules
- Purge cache after deployments

## 🔒 Security Headers

Add to nginx.conf or platform config:
```nginx
add_header X-Frame-Options "SAMEORIGIN" always;
add_header X-Content-Type-Options "nosniff" always;
add_header X-XSS-Protection "1; mode=block" always;
add_header Referrer-Policy "strict-origin-when-cross-origin" always;
```

## 📈 Monitoring

### Setup Analytics
- Google Analytics
- Vercel Analytics
- Sentry for error tracking

### Health Check Endpoint
Monitor: `https://your-frontend.com`

Should return 200 status code.

## 🚀 CI/CD Pipeline

**GitHub Actions** (`.github/workflows/deploy.yml`):
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
      - uses: actions/setup-node@v3
        with:
          node-version: '18'
      - working-directory: ./application/frontend
        run: |
          npm ci
          npm run build
      - uses: amondnet/vercel-action@v20
        with:
          vercel-token: ${{ secrets.VERCEL_TOKEN }}
          vercel-org-id: ${{ secrets.VERCEL_ORG_ID }}
          vercel-project-id: ${{ secrets.VERCEL_PROJECT_ID }}
          working-directory: ./application/frontend
```

---

## 🎉 Success!

Your frontend is now deployed and connected to the backend at:
`https://stockflow-pro.up.railway.app`

**Test it:**
1. Visit your deployed URL
2. Login with: admin@stockflowpro.com / Admin@123
3. Explore all features!

---

**Need Help?** Check the main [GETTING_STARTED.md](../GETTING_STARTED.md) guide.
