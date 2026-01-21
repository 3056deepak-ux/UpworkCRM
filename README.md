# UpworkERP - Enterprise Resource Planning Framework

This repository serves as the framework for an enterprise-level modular ERP system designed for scalability, robust coding patterns, and a professional Bootstrap 5 frontend theme.

## Features

✅ **Layered Architecture** - Core, Infrastructure, Application, API, and Web layers
✅ **SOLID Principles** - Clean, maintainable, and extensible code
✅ **Repository & Service Pattern** - Generic implementations for all entities
✅ **Entity Framework Core** - Modern ORM with LINQ support
✅ **JWT Authentication** - Secure API authentication
✅ **Activity Logging** - Track all user actions
✅ **Audit Trail** - Complete change history
✅ **RESTful API** - Well-documented with Swagger/OpenAPI
✅ **Unit Tests** - xUnit tests with Moq
✅ **Bootstrap 5** - Modern, responsive UI
✅ **Cloud Ready** - Deploy to Azure, AWS, or DigitalOcean

## Modules

- **HR** - Employee management, payroll, leave tracking
- **Finance** - Accounting, budgeting, reporting
- **CRM** - Customer and lead management
- **Projects** - Project tracking, time management
- **Inventory** - Stock levels, warehouse management

## Quick Start

```bash
# Clone the repository
git clone https://github.com/3056deepak-ux/UpworkCRM.git
cd UpworkCRM

# Restore packages
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Run the API
dotnet run --project src/UpworkERP.API

# Run the Web application
dotnet run --project src/UpworkERP.Web
```

## Documentation

- [📖 Full Documentation](./docs/README.md)
- [🏗️ Architecture Guide](./docs/ARCHITECTURE.md)
- [🚀 Deployment Guide](./docs/DEPLOYMENT.md)

## Technology Stack

- .NET 10.0
- Entity Framework Core
- SQL Server
- ASP.NET Core Web API
- ASP.NET Core MVC
- Bootstrap 5
- JWT Authentication
- Swagger/OpenAPI
- xUnit + Moq

## Project Structure

```
UpworkERP/
├── src/
│   ├── UpworkERP.Core/              # Domain entities and interfaces
│   ├── UpworkERP.Infrastructure/    # Data access and repositories
│   ├── UpworkERP.Application/       # Business logic and services
│   ├── UpworkERP.API/               # REST API endpoints
│   └── UpworkERP.Web/               # MVC web application
├── tests/
│   └── UpworkERP.Tests/             # Unit tests
└── docs/                            # Documentation
```

## License

Proprietary - All rights reserved.