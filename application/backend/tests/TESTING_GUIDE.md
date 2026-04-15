# StockFlow Pro - Testing Guide

## 🧪 Testing Overview

This project includes comprehensive testing covering:
- **Unit Tests** - Test individual components in isolation
- **Integration Tests** - Test full API endpoints with in-memory database
- **Manual API Testing** - REST Client .http files for interactive testing

---

## 📦 Test Projects

### 1. StockFlow.UnitTests
Tests business logic, validators, and domain entities.

**Technologies:**
- xUnit - Test framework
- Moq - Mocking library
- FluentAssertions - Fluent assertion syntax

**Test Coverage:**
- ✅ CQRS Command Handlers
- ✅ CQRS Query Handlers
- ✅ FluentValidation Validators
- ✅ Domain Entities
- ✅ Business Logic

### 2. StockFlow.IntegrationTests
Tests full API endpoints with in-memory database.

**Technologies:**
- xUnit - Test framework
- WebApplicationFactory - API testing
- InMemory Database - Test database
- FluentAssertions - Assertions

**Test Coverage:**
- ✅ API Controllers (Products, Auth, etc.)
- ✅ Authentication Flow
- ✅ Database Operations
- ✅ Health Checks
- ✅ End-to-End Scenarios

---

## 🚀 Running Tests

### Run All Tests
```powershell
# From solution root
dotnet test

# With detailed output
dotnet test --verbosity detailed

# With logger
dotnet test --logger "console;verbosity=detailed"
```

### Run Unit Tests Only
```powershell
cd tests/StockFlow.UnitTests
dotnet test
```

### Run Integration Tests Only
```powershell
cd tests/StockFlow.IntegrationTests
dotnet test
```

### Run Specific Test Class
```powershell
dotnet test --filter "FullyQualifiedName~ProductsControllerTests"
```

### Run Specific Test Method
```powershell
dotnet test --filter "FullyQualifiedName~CreateProduct_WithValidData_ReturnsCreated"
```

---

## 📊 Test Coverage

### Generate Coverage Report
```powershell
# Install coverage tool
dotnet tool install --global dotnet-coverage

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate HTML report
dotnet tool install --global dotnet-reportgenerator-globaltool
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html

# Open report
start coveragereport/index.html
```

### Coverage with Coverlet
```powershell
# Install coverlet
dotnet add package coverlet.collector

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Generate report
reportgenerator -reports:"**/coverage.opencover.xml" -targetdir:"coveragereport" -reporttypes:Html
```

---

## 🧪 Unit Tests

### Example Test Structure
```csharp
[Fact]
public async Task Handle_ValidProduct_ReturnsProductDto()
{
    // Arrange
    var command = new CreateProductCommand { ... };
    
    // Act
    var result = await _handler.Handle(command, default);
    
    // Assert
    result.Should().NotBeNull();
}
```

### Running Specific Unit Tests
```powershell
# Test validators
dotnet test --filter "FullyQualifiedName~ValidatorTests"

# Test command handlers
dotnet test --filter "FullyQualifiedName~CommandHandlerTests"

# Test domain entities
dotnet test --filter "FullyQualifiedName~Domain.Entities"
```

---

## 🌐 Integration Tests

### Example Integration Test
```csharp
[Fact]
public async Task GetAllProducts_ReturnsSuccessStatusCode()
{
    // Act
    var response = await _client.GetAsync("/api/v1/products");
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
}
```

### Running Specific Integration Tests
```powershell
# Test specific controller
dotnet test --filter "FullyQualifiedName~ProductsControllerTests"

# Test authentication
dotnet test --filter "FullyQualifiedName~AuthControllerTests"

# Test health checks
dotnet test --filter "FullyQualifiedName~HealthCheckTests"
```

---

## 🔍 Manual API Testing

### Using REST Client (VS Code Extension)

1. **Install Extension**
   - Extension ID: `humao.rest-client`
   - Search "REST Client" in VS Code extensions

2. **Open Test File**
   - File: `tests/StockFlow.API.http`

3. **Run Requests**
   - Click "Send Request" above each HTTP request
   - Results appear in side panel

### Workflow Example

```http
### 1. Login to get tokens
POST http://localhost:5057/api/v1/auth/login
Content-Type: application/json

{
  "email": "admin@stockflow.com",
  "password": "Admin@123"
}

### 2. Copy the accessToken from response

### 3. Get products with token
GET http://localhost:5057/api/v1/products
Authorization: Bearer YOUR_ACCESS_TOKEN_HERE
```

---

## 📋 Test Scenarios

### Authentication Flow
1. ✅ Register new user
2. ✅ Login with credentials
3. ✅ Get access token
4. ✅ Use token for API calls
5. ✅ Refresh expired token
6. ✅ Logout (revoke token)

### CRUD Operations
1. ✅ Create resource
2. ✅ Read single resource
3. ✅ Read all resources (paginated)
4. ✅ Update resource
5. ✅ Delete resource (soft delete)
6. ✅ Verify deletion

### Validation Testing
1. ✅ Valid data passes
2. ✅ Missing required fields fail
3. ✅ Invalid format fails
4. ✅ Out-of-range values fail
5. ✅ Duplicate unique fields fail

---

## 🎯 Best Practices

### Unit Tests
- ✅ Test one thing per test
- ✅ Use descriptive test names
- ✅ Follow Arrange-Act-Assert pattern
- ✅ Mock external dependencies
- ✅ Keep tests fast and isolated

### Integration Tests
- ✅ Use in-memory database
- ✅ Clean database between tests
- ✅ Test complete workflows
- ✅ Verify HTTP status codes
- ✅ Check response content

### Manual Testing
- ✅ Test happy paths
- ✅ Test error scenarios
- ✅ Test edge cases
- ✅ Verify authentication
- ✅ Check authorization (roles)

---

## 📈 Continuous Integration

### GitHub Actions Example
```yaml
name: Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      - name: Restore dependencies
        run: dotnet restore
      - name: Build
        run: dotnet build --no-restore
      - name: Test
        run: dotnet test --no-build --verbosity normal
```

---

## 🐛 Debugging Tests

### Debug in VS Code
1. Set breakpoint in test
2. Open Testing panel (beaker icon)
3. Right-click test → "Debug Test"

### Debug in Visual Studio
1. Set breakpoint in test
2. Right-click test in Test Explorer
3. Select "Debug"

### View Test Output
```powershell
# Detailed output
dotnet test --logger "console;verbosity=detailed"

# Output to file
dotnet test --logger "trx;LogFileName=testresults.trx"
```

---

## 📊 Test Results

### View Test Results
```powershell
# Simple summary
dotnet test

# Detailed results
dotnet test --logger "console;verbosity=detailed"

# HTML report
dotnet test --logger "html;LogFileName=testresults.html"
```

### Example Output
```
Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:    25, Skipped:     0, Total:    25, Duration: 2.5s
```

---

## 🔧 Troubleshooting

### Tests Not Discovered
```powershell
# Clear test cache
dotnet clean
dotnet build
dotnet test
```

### Port Already in Use
```powershell
# Integration tests use random ports
# No action needed - handled automatically by WebApplicationFactory
```

### Database Connection Issues
```powershell
# Integration tests use InMemory database
# No external database required
# Ensure Microsoft.EntityFrameworkCore.InMemory package is installed
```

---

## 📚 Additional Resources

- **xUnit Documentation**: https://xunit.net/
- **FluentAssertions**: https://fluentassertions.com/
- **Moq Documentation**: https://github.com/moq/moq4
- **REST Client Extension**: https://marketplace.visualstudio.com/items?itemName=humao.rest-client
- **ASP.NET Core Testing**: https://learn.microsoft.com/en-us/aspnet/core/test/

---

## ✅ Test Checklist

Before deploying:
- [ ] All unit tests pass
- [ ] All integration tests pass
- [ ] Manual testing completed
- [ ] Test coverage > 70%
- [ ] No failing tests in CI/CD
- [ ] Performance tests pass
- [ ] Security tests pass

---

**Happy Testing! 🧪**
