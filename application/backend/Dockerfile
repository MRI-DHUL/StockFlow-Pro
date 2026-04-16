# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["application/backend/StockFlow.sln", "application/backend/"]
COPY ["application/backend/src/StockFlow.API/StockFlow.API.csproj", "application/backend/src/StockFlow.API/"]
COPY ["application/backend/src/StockFlow.Application/StockFlow.Application.csproj", "application/backend/src/StockFlow.Application/"]
COPY ["application/backend/src/StockFlow.Domain/StockFlow.Domain.csproj", "application/backend/src/StockFlow.Domain/"]
COPY ["application/backend/src/StockFlow.Infrastructure/StockFlow.Infrastructure.csproj", "application/backend/src/StockFlow.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "application/backend/src/StockFlow.API/StockFlow.API.csproj"

# Copy everything else
COPY application/backend/ application/backend/

# Build the application
WORKDIR "/src/application/backend/src/StockFlow.API"
RUN dotnet build "StockFlow.API.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "StockFlow.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published files
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/Logs

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=30s --retries=3 \
  CMD curl -f http://localhost/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "StockFlow.API.dll"]
