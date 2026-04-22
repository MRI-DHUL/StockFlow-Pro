# StockFlow Pro - Quick Start Guide

Complete guide to run the full-stack application.

## 🎯 Prerequisites

### Frontend Only (Recommended)
- Node.js 18+
- npm 11+
- Angular CLI 21+

> **✅ Backend Already Deployed**: The frontend connects to the live backend at https://stockflow-pro.up.railway.app  
> You only need to run the frontend locally!

### Backend (Optional - Already Deployed)
If you want to run backend locally:
- .NET 9 SDK
- SQL Server (LocalDB or full version)
- Redis (optional, falls back to in-memory)

## 🚀 Quick Start (2 Minutes)

### Step 1: Clone & Navigate
```bash
git clone <repository-url>
cd StockFlow-Pro/application/frontend
```

### Step 2: Install & Run Frontend

```bash
# Install dependencies (first time only)
npm install

# Start dev server
npm start
```

✅ Frontend running at: **http://localhost:4200** (or next available port)  
✅ Backend API: **https://stockflow-pro.up.railway.app** (already deployed)  
✅ Swagger UI: **https://stockflow-pro.up.railway.app/swagger**

> **That's it!** The frontend automatically connects to the deployed backend.

## 🔐 Default Login

- **Email**: admin@stockflowpro.com
- **Password**: Admin@123

## 📊 Application URLs

| Service | URL | Description |
|---------|-----|-------------|
| Frontend | http://localhost:4200 | Angular SPA |
| Backend API | http://localhost:5057/api | REST API |
| Swagger | http://localhost:5057/swagger | API Documentation |
| Hangfire | http://localhost:5057/hangfire | Background Jobs Dashboard |

## 🧪 Testing the Application

### 1. Login
- Go to http://localhost:4200
- Login with admin credentials
- You'll be redirected to the dashboard

> **Backend**: Already deployed and running at https://stockflow-pro.up.railway.app

### 2. Explore Features
- **Dashboard**: View statistics and notifications
- **Products**: Add/edit products with filtering
- **Inventory**: Manage stock levels across warehouses
- **Warehouses**: Create and manage warehouses
- **Suppliers**: Manage supplier information
- **Orders**: Track customer orders
- **Purchase Orders**: Manage supplier purchase orders
- **Stock M(Local) | http://localhost:4200 | Angular SPA (Run locally) |
| Backend API | https://stockflow-pro.up.railway.app/api | REST API (Deployed on Railway) |
| Swagger | https://stockflow-pro.up.railway.app/swagger | API Documentation |
| Health Check | https://stockflow-pro.up.railway.app/health | Backend health status
- Backend sends Pusher notifications for:
  - Low stock alerts
  - New orders
  - Stock updates
- Watch the dashboard for real-time updates

## Frontend Configuration

Edit `application/frontend/src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://stockflow-pro.up.railway.app/api/v1', // Deployed backend
  pusher: {
    appKey: '3a6db031b2381d1e78ec',
    cluster: 'ap2',
    channel: 'stockflow-notifications'
  }
};
```

> **✅ Using Deployed Backend**: The configuration already points to the Railway deployment.  
> No backend setup needed for frontend development!

### Optional: Run Backend Locally

If you want to develop backend features, edit environment.ts:

```typescript
apiUrl: 'http://localhost:5057/api/v1', // Local backend
```

Then start the backend:
```bash
cd application/backend/src/StockFlow.API
dotnet run  channel: 'stockflow-notifications'
  }
};
```

## 🔄 Database Migrations

### Create New Migration
```bash
cd application/backend/src/StockFlow.API
dotnet ef migrations add MigrationName
```

### Apply Migration
```bash
dotnet ef database update
```

### Rollback Migration
```bash
dotnet ef database update PreviousMigrationName
```

## 📦 Production Build

### Backend
```bash
cd application/backend/src/StockFlow.API
dotnet publish -c Release -o ./publish
```

### Frontend
```bash
cd application/frontend
npm run build
# Output in dist/stockflow-app/
```

## 🐛 Common Issues

### Backend Issues

**Port 5057 already in use**
```bash
# Change port in launchSettings.json or kill process
netstat -ano | findstr :5057
taskFrontend Issues

**Port 4200 already in use**
- Angular will prompt for different port
- Or manually specify: `ng serve --port 4201`

**API connection error**
- Verify backend is running: https://stockflow-pro.up.railway.app/health
- Check browser console for CORS errors
- Clear localStorage: `localStorage.clear()`
npm install
```

## 📚 Documentation

- [Backend README](application/backend/README.md)
- [Frontend README](application/frontend/README.md)
- [API Quick Start](application/backend/API_QUICK_START.md)
- [Deployment Guide](DEPLOYMENT.md)
- [Architecture Overview](application/backend/ARCHITECTURE.md)

## 🎓 Learning Resources

### Backend Stack
- [ASP.NET Core](https://learn.microsoft.com/aspnet/core)
- [Entity Framework Core](https://learn.microsoft.com/ef/core)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

### Frontend Stack
- [Angular](https://angular.dev)
- [Angular Material](https://material.angular.io)
- [RxJS](https://rxjs.dev)

## 🤝 Contributing

1. Create feature branch: `git checkout -b feature/my-feature`
2. Make changes and test
3. Commit: `git commit -m 'Add feature'`
4. Push: `git push origin feature/my-feature`
5. Create Pull Request

## 📄 License

MIT License - see LICENSE file

## 🆘 Support

For issues and questions:
1. Check the troubleshooting sections above
2. Review documentation in /docs
3. Check existing GitHub issues
4. Create new issue with details

---

**Happy Coding! 🚀**
