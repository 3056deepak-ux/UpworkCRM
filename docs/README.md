# UpworkERP - Enterprise Resource Planning Framework

## Overview

UpworkERP is a modern, enterprise-level ERP framework built with ASP.NET Core 10.0 that follows industry best practices and SOLID principles. The framework provides a scalable, maintainable, and modular architecture designed for extensibility and long-term sustainability.

## Architecture

The application follows a **layered architecture** pattern with clear separation of concerns:

### Layer Structure

```
UpworkERP/
├── src/
│   ├── UpworkERP.Core/              # Domain Layer
│   ├── UpworkERP.Infrastructure/    # Data Access Layer
│   ├── UpworkERP.Application/       # Service Layer
│   ├── UpworkERP.API/               # API Layer
│   └── UpworkERP.Web/               # Presentation Layer
├── tests/
│   └── UpworkERP.Tests/             # Unit Tests
└── docs/                            # Documentation
```

## Core Modules

The framework includes five primary functional modules:

1. **HR Module**
   - Employee management
   - Leave request tracking
   - Payroll record management

2. **Finance Module**
   - Accounting and transactions
   - Budget management
   - Financial reporting

3. **CRM Module**
   - Customer management
   - Lead tracking and conversion
   - Sales pipeline

4. **Projects Module**
   - Project tracking
   - Task management
   - Time entry and tracking

5. **Inventory Module**
   - Product management
   - Stock movement tracking
   - Warehouse management

## Design Patterns

### Repository Pattern
Generic repository implementation for data access abstraction:
```csharp
IRepository<T> where T : class, IEntity
```

### Service Pattern
Generic service layer for business logic:
```csharp
IService<T> where T : class, IEntity
```

### Dependency Injection
All services and repositories are registered using .NET's built-in DI container.

## Enterprise Features

### Activity Logging
Tracks all user activities in the system:
- User actions (Create, Read, Update, Delete)
- Timestamps and IP addresses
- Entity-specific logging

### Audit Trail
Maintains complete audit history:
- Captures old and new values
- User identification
- Action timestamps

### Role-Based Access Control (RBAC)
User roles defined in the system:
- Admin
- Manager
- Employee
- Guest

### JWT Authentication
Secure API authentication using JWT tokens with configurable expiration.

## Technology Stack

- **.NET 10.0** - Framework
- **Entity Framework Core** - ORM
- **SQL Server** - Database
- **Swagger/OpenAPI** - API Documentation
- **xUnit** - Testing Framework
- **Moq** - Mocking Framework
- **Bootstrap 5** - UI Framework

## Getting Started

### Prerequisites
- .NET 10.0 SDK or later
- SQL Server or SQL Server Express
- Visual Studio 2022 or VS Code

### Configuration

1. Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=UpworkERP;Trusted_Connection=True;"
  }
}
```

2. Configure JWT settings:
```json
{
  "JwtSettings": {
    "SecretKey": "Your-Secret-Key-Here",
    "Issuer": "UpworkERP.API",
    "Audience": "UpworkERP.Web",
    "ExpirationMinutes": 60
  }
}
```

### Running the Application

1. **Restore packages:**
```bash
dotnet restore
```

2. **Build the solution:**
```bash
dotnet build
```

3. **Run migrations (when created):**
```bash
dotnet ef database update --project src/UpworkERP.Infrastructure
```

4. **Run the API:**
```bash
dotnet run --project src/UpworkERP.API
```

5. **Run the Web application:**
```bash
dotnet run --project src/UpworkERP.Web
```

### Running Tests

```bash
dotnet test
```

## API Documentation

When running in development mode, Swagger UI is available at the application root:
```
https://localhost:5001/
```

## Project Structure Details

### Core Layer
- **Entities**: Domain models with audit fields
- **Interfaces**: Contracts for repositories and services
- **Enums**: Type-safe enumerations for domain concepts

### Infrastructure Layer
- **Data**: DbContext and database configuration
- **Repositories**: Generic repository implementation

### Application Layer
- **Services**: Business logic implementation
- **DTOs**: Data transfer objects
- **ApiClient**: Generic HTTP client for API consumption

### API Layer
- **Controllers**: REST API endpoints
- **Middleware**: Custom middleware components
- **Configuration**: Startup configuration

### Web Layer
- **Controllers**: MVC controllers
- **Views**: Razor views with Bootstrap 5
- **wwwroot**: Static files (CSS, JS, images)

## SOLID Principles Implementation

### Single Responsibility Principle
Each class has one reason to change. Services handle business logic, repositories handle data access.

### Open/Closed Principle
Generic implementations allow extension without modification.

### Liskov Substitution Principle
All derived classes can substitute their base classes.

### Interface Segregation Principle
Small, focused interfaces (IEntity, IAuditable, IRepository).

### Dependency Inversion Principle
Depend on abstractions (interfaces), not concretions.

## Cloud Deployment

The application is designed to be cloud-ready and can be deployed to:
- **AWS** (Elastic Beanstalk, ECS, Lambda)
- **Azure** (App Service, Container Instances, Functions)
- **DigitalOcean** (App Platform, Droplets)

### Docker Support
(To be implemented)

## Future Enhancements

- Database migrations
- Docker containerization
- CI/CD pipeline configuration
- Additional module implementations
- Advanced reporting features
- Mobile application support
- Real-time notifications (SignalR)

## Contributing

This is a proprietary framework. Please refer to internal contribution guidelines.

## License

Proprietary - All rights reserved.

## Support

For support, please contact: support@upworkerp.com
