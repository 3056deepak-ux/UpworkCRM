# Deployment Guide

## Overview

This guide covers deploying the UpworkERP framework to various cloud platforms.

## Prerequisites

- .NET 10.0 SDK
- SQL Server or equivalent
- Cloud account (AWS/Azure/DigitalOcean)
- Domain name (optional)

## Local Development Setup

### 1. Clone the Repository
```bash
git clone https://github.com/your-org/UpworkERP.git
cd UpworkERP
```

### 2. Install Dependencies
```bash
dotnet restore
```

### 3. Configure Database
Update `appsettings.json` in both API and Web projects:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=UpworkERP;Trusted_Connection=True;"
  }
}
```

### 4. Create Database (when migrations are ready)
```bash
dotnet ef database update --project src/UpworkERP.Infrastructure
```

### 5. Run the Applications
```bash
# Terminal 1 - API
dotnet run --project src/UpworkERP.API

# Terminal 2 - Web
dotnet run --project src/UpworkERP.Web
```

## Azure Deployment

### Prerequisites
- Azure account
- Azure CLI installed
- Azure SQL Database provisioned

### 1. Create Azure Resources
```bash
# Login to Azure
az login

# Create resource group
az group create --name UpworkERP-RG --location eastus

# Create App Service Plan
az appservice plan create --name UpworkERP-Plan --resource-group UpworkERP-RG --sku S1

# Create Web App for API
az webapp create --name UpworkERP-API --resource-group UpworkERP-RG --plan UpworkERP-Plan --runtime "DOTNET:10"

# Create Web App for UI
az webapp create --name UpworkERP-Web --resource-group UpworkERP-RG --plan UpworkERP-Plan --runtime "DOTNET:10"

# Create Azure SQL Server
az sql server create --name upworkerp-sql --resource-group UpworkERP-RG --location eastus --admin-user sqladmin --admin-password YourPassword123!

# Create Azure SQL Database
az sql db create --resource-group UpworkERP-RG --server upworkerp-sql --name UpworkERPDB --service-objective S0
```

### 2. Configure Connection Strings
```bash
# Set connection string for API
az webapp config connection-string set --name UpworkERP-API --resource-group UpworkERP-RG --connection-string-type SQLAzure --settings DefaultConnection="Server=tcp:upworkerp-sql.database.windows.net,1433;Database=UpworkERPDB;User ID=sqladmin;Password=YourPassword123!;Encrypt=True;TrustServerCertificate=False;"

# Set connection string for Web
az webapp config connection-string set --name UpworkERP-Web --resource-group UpworkERP-RG --connection-string-type SQLAzure --settings DefaultConnection="Server=tcp:upworkerp-sql.database.windows.net,1433;Database=UpworkERPDB;User ID=sqladmin;Password=YourPassword123!;Encrypt=True;TrustServerCertificate=False;"
```

### 3. Deploy Applications
```bash
# Publish API
cd src/UpworkERP.API
dotnet publish -c Release
cd bin/Release/net10.0/publish
zip -r api.zip *
az webapp deployment source config-zip --resource-group UpworkERP-RG --name UpworkERP-API --src api.zip

# Publish Web
cd ../../../../../UpworkERP.Web
dotnet publish -c Release
cd bin/Release/net10.0/publish
zip -r web.zip *
az webapp deployment source config-zip --resource-group UpworkERP-RG --name UpworkERP-Web --src web.zip
```

### 4. Configure Environment Variables
```bash
# API App Settings
az webapp config appsettings set --name UpworkERP-API --resource-group UpworkERP-RG --settings \
  JwtSettings__SecretKey="YourSecureSecretKeyHere123456789" \
  JwtSettings__Issuer="UpworkERP.API" \
  JwtSettings__Audience="UpworkERP.Web" \
  JwtSettings__ExpirationMinutes="60"
```

## AWS Deployment

### Prerequisites
- AWS account
- AWS CLI installed
- RDS instance for SQL Server

### 1. Create AWS Resources
```bash
# Configure AWS CLI
aws configure

# Create Elastic Beanstalk Application
aws elasticbeanstalk create-application --application-name UpworkERP

# Create Environment for API
aws elasticbeanstalk create-environment --application-name UpworkERP \
  --environment-name UpworkERP-API-Prod \
  --solution-stack-name "64bit Amazon Linux 2023 v3.0.0 running .NET 10"

# Create Environment for Web
aws elasticbeanstalk create-environment --application-name UpworkERP \
  --environment-name UpworkERP-Web-Prod \
  --solution-stack-name "64bit Amazon Linux 2023 v3.0.0 running .NET 10"
```

### 2. Deploy to Elastic Beanstalk
```bash
# Install EB CLI
pip install awsebcli

# Initialize EB
cd src/UpworkERP.API
eb init -p "dotnet-10" UpworkERP-API

# Deploy
eb create UpworkERP-API-Prod
eb deploy

# Repeat for Web
cd ../UpworkERP.Web
eb init -p "dotnet-10" UpworkERP-Web
eb create UpworkERP-Web-Prod
eb deploy
```

### 3. Configure RDS Connection
```bash
# Set environment variables
eb setenv ConnectionStrings__DefaultConnection="Server=your-rds-endpoint.amazonaws.com;Database=UpworkERP;User ID=admin;Password=yourpassword"
eb setenv JwtSettings__SecretKey="YourSecureSecretKeyHere123456789"
```

## DigitalOcean Deployment

### Prerequisites
- DigitalOcean account
- doctl CLI installed
- Managed Database (PostgreSQL/MySQL alternative)

### 1. Create App Platform Project
```bash
# Login to DigitalOcean
doctl auth init

# Create app from spec file
doctl apps create --spec app-spec.yaml
```

### 2. app-spec.yaml
```yaml
name: upworkerp
services:
  - name: api
    github:
      repo: your-org/UpworkERP
      branch: main
      deploy_on_push: true
    source_dir: /src/UpworkERP.API
    run_command: dotnet UpworkERP.API.dll
    environment_slug: dotnet
    instance_count: 1
    instance_size_slug: professional-xs
    routes:
      - path: /api
    envs:
      - key: ConnectionStrings__DefaultConnection
        value: ${db.DATABASE_URL}
      - key: JwtSettings__SecretKey
        value: YourSecureSecretKeyHere123456789
        type: SECRET
  
  - name: web
    github:
      repo: your-org/UpworkERP
      branch: main
      deploy_on_push: true
    source_dir: /src/UpworkERP.Web
    run_command: dotnet UpworkERP.Web.dll
    environment_slug: dotnet
    instance_count: 1
    instance_size_slug: professional-xs
    routes:
      - path: /
    envs:
      - key: ConnectionStrings__DefaultConnection
        value: ${db.DATABASE_URL}
      - key: ApiBaseUrl
        value: https://upworkerp-api.ondigitalocean.app

databases:
  - name: db
    engine: PG
    version: "15"
    size: db-s-1vcpu-1gb
    num_nodes: 1
```

## Docker Deployment

### 1. Create Dockerfile for API
```dockerfile
# src/UpworkERP.API/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/UpworkERP.API/UpworkERP.API.csproj", "UpworkERP.API/"]
COPY ["src/UpworkERP.Application/UpworkERP.Application.csproj", "UpworkERP.Application/"]
COPY ["src/UpworkERP.Infrastructure/UpworkERP.Infrastructure.csproj", "UpworkERP.Infrastructure/"]
COPY ["src/UpworkERP.Core/UpworkERP.Core.csproj", "UpworkERP.Core/"]
RUN dotnet restore "UpworkERP.API/UpworkERP.API.csproj"
COPY src/ .
WORKDIR "/src/UpworkERP.API"
RUN dotnet build "UpworkERP.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UpworkERP.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UpworkERP.API.dll"]
```

### 2. Docker Compose
```yaml
# docker-compose.yml
version: '3.8'

services:
  api:
    build:
      context: .
      dockerfile: src/UpworkERP.API/Dockerfile
    ports:
      - "5000:80"
    environment:
      - ConnectionStrings__DefaultConnection=Server=db;Database=UpworkERP;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True
      - JwtSettings__SecretKey=YourSecureSecretKeyHere123456789
    depends_on:
      - db
  
  web:
    build:
      context: .
      dockerfile: src/UpworkERP.Web/Dockerfile
    ports:
      - "5001:80"
    environment:
      - ConnectionStrings__DefaultConnection=Server=db;Database=UpworkERP;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True
      - ApiBaseUrl=http://api
    depends_on:
      - api
  
  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourPassword123!
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql

volumes:
  sqldata:
```

### 3. Build and Run
```bash
# Build images
docker-compose build

# Run containers
docker-compose up -d

# View logs
docker-compose logs -f

# Stop containers
docker-compose down
```

## CI/CD Setup

### GitHub Actions

Create `.github/workflows/deploy.yml`:
```yaml
name: Deploy to Azure

on:
  push:
    branches: [ main ]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v2
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: '10.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --configuration Release --no-restore
    
    - name: Test
      run: dotnet test --no-build --verbosity normal
    
    - name: Publish API
      run: dotnet publish src/UpworkERP.API/UpworkERP.API.csproj -c Release -o ./api-publish
    
    - name: Deploy API to Azure
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'UpworkERP-API'
        publish-profile: ${{ secrets.AZURE_API_PUBLISH_PROFILE }}
        package: ./api-publish
```

## SSL/TLS Configuration

### Azure
```bash
# Configure custom domain
az webapp config hostname add --webapp-name UpworkERP-API --resource-group UpworkERP-RG --hostname api.yourdomain.com

# Enable HTTPS only
az webapp update --name UpworkERP-API --resource-group UpworkERP-RG --https-only true
```

### AWS
```bash
# Request certificate in ACM
aws acm request-certificate --domain-name api.yourdomain.com --validation-method DNS

# Add certificate to load balancer
aws elb set-load-balancer-listener-ssl-certificate --load-balancer-name UpworkERP-LB --load-balancer-port 443 --ssl-certificate-id arn:aws:acm:region:account:certificate/certificate-id
```

## Monitoring Setup

### Application Insights (Azure)
```bash
# Create Application Insights
az monitor app-insights component create --app UpworkERP-Insights --location eastus --resource-group UpworkERP-RG

# Get instrumentation key
az monitor app-insights component show --app UpworkERP-Insights --resource-group UpworkERP-RG --query instrumentationKey
```

Add to appsettings.json:
```json
{
  "ApplicationInsights": {
    "InstrumentationKey": "your-key-here"
  }
}
```

## Backup and Disaster Recovery

### Azure SQL Backup
```bash
# Configure automated backups
az sql db update --resource-group UpworkERP-RG --server upworkerp-sql --name UpworkERPDB --backup-storage-redundancy Geo
```

### RDS Backup
```bash
# Enable automated backups
aws rds modify-db-instance --db-instance-identifier upworkerp-db --backup-retention-period 7 --apply-immediately
```

## Scaling

### Azure
```bash
# Scale out (add instances)
az appservice plan update --name UpworkERP-Plan --resource-group UpworkERP-RG --number-of-workers 3

# Scale up (larger instance)
az appservice plan update --name UpworkERP-Plan --resource-group UpworkERP-RG --sku P1V2
```

### AWS
```bash
# Configure auto-scaling
aws autoscaling create-auto-scaling-group --auto-scaling-group-name upworkerp-asg --min-size 2 --max-size 10 --desired-capacity 2
```

## Troubleshooting

### Check Application Logs
```bash
# Azure
az webapp log tail --name UpworkERP-API --resource-group UpworkERP-RG

# AWS
eb logs

# DigitalOcean
doctl apps logs <app-id> --tail
```

### Database Connection Issues
1. Check connection string format
2. Verify firewall rules
3. Test connectivity with SQL client
4. Check application logs for detailed error

### Performance Issues
1. Enable Application Insights/CloudWatch
2. Check database query performance
3. Review application metrics
4. Consider adding caching layer

## Security Checklist

- [ ] HTTPS enabled
- [ ] JWT secret key changed from default
- [ ] Database credentials secured
- [ ] Firewall rules configured
- [ ] CORS properly configured
- [ ] API rate limiting enabled
- [ ] Sensitive data encrypted
- [ ] Regular security updates applied
- [ ] Backup strategy in place
- [ ] Monitoring and alerts configured

## Support

For deployment issues:
- Check logs first
- Review cloud provider documentation
- Contact support team
- Consult architecture documentation
