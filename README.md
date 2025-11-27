# Customer Onboarding API

ASP.NET Core 8.0 REST API for customer onboarding with OTP verification.

## Features

- ✅ IC number validation and registration
- ✅ Email and mobile OTP verification
- ✅ Multi-flow OTP system (Registration, Migration, Change Email)
- ✅ PIN creation with validation
- ✅ Biometric enrollment
- ✅ Email change functionality
- ✅ User migration support

## Tech Stack

- **Framework:** ASP.NET Core 8.0
- **Database:** Microsoft SQL Server
- **ORM:** Entity Framework Core 8.0.11
- **Pattern:** Repository Pattern with Dependency Injection

## Database Schema

### Tables
- **Users** - Main user information with verification status
- **UserSecurity** - PIN storage
- **OTPAttempts** - OTP codes with flow tracking

## API Endpoints

### Registration Flow
1. `GET /api/Users/check-ic/{icNumber}` - Check IC status
2. `POST /api/Users/registration` - Register/update user
3. `POST /api/Users/send-otp` - Send OTP (auto-detects flow)
4. `POST /api/Users/verify-otp` - Verify OTP
5. `POST /api/Users/agree-terms/{userId}` - Accept terms
6. `POST /api/Users/pin/{userId}` - Set PIN
7. `POST /api/Users/biometric/{userId}` - Enable biometric

### Migration Flow
1. `POST /api/Users/migration/initiate` - Initiate migration
2. `POST /api/Users/migration/change-email` - Change email with OTP

## Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB, Express, or Full version)
- SQL Server Management Studio (optional, for database management)

## Setup Instructions

### 1. Clone the Repository
```bash
git clone https://github.com/NourHamdach/CustomerOnboardingAPI.git
cd CustomerOnboardingAPI
```

### 2. Configure Database Connection

Update `appsettings.json` with your SQL Server connection:

**For Windows Authentication (Recommended):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CustomerOnboardingDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**For SQL Server Authentication:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CustomerOnboardingDb;User Id=your_username;Password=your_password;TrustServerCertificate=True;"
  }
}
```

**For SQL Server Express:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=CustomerOnboardingDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Install Dependencies
```bash
cd CustomerOnboarding.Api
dotnet restore
```

### 4. Database Migration

**Create initial migration (if not exists):**
```bash
dotnet ef migrations add InitialCreate
```

**Apply migrations to create database:**
```bash
dotnet ef database update
```

**Verify database creation:**
```bash
# The database 'CustomerOnboardingDb' will be created automatically
# Tables: Users, UserSecurity, OTPAttempts, __EFMigrationsHistory
```

### 5. Run the Application
```bash
dotnet run
```

Or with hot reload:
```bash
dotnet watch run
```

### 6. Access the Application
- **API Base URL:** http://localhost:5000
- **Swagger UI:** http://localhost:5000/swagger
- **Health Check:** http://localhost:5000/api/Users/check-ic/123456789012

## Database Management

### View Migration History
```bash
dotnet ef migrations list
```

### Create New Migration
```bash
dotnet ef migrations add MigrationName
```

### Rollback Migration
```bash
# Rollback to previous migration
dotnet ef database update PreviousMigrationName

# Rollback all migrations
dotnet ef database update 0
```

### Remove Last Migration (before applying)
```bash
dotnet ef migrations remove
```

### Drop Database and Recreate
```bash
dotnet ef database drop
dotnet ef database update
```

### Generate SQL Script
```bash
# Generate SQL for all migrations
dotnet ef migrations script -o migration.sql

# Generate SQL for specific range
dotnet ef migrations script FromMigration ToMigration -o migration.sql
```

## Development Workflow

### Building the Project
```bash
# Debug build
dotnet build

# Release build
dotnet build -c Release
```

### Running Tests
```bash
dotnet test
```

### Publishing
```bash
# Publish for production
dotnet publish -c Release -o ./publish

# Run published app
cd publish
dotnet CustomerOnboarding.Api.dll
```

## Security Features

- OTP expiration (5 minutes)
- Email format validation
- PIN must be 6 digits (numbers only)
- Contact changes require OTP verification
- Migration requires full verification
- Server-side flow determination

## Project Structure

```
CustomerOnboarding.Api/
├── Controllers/        # API endpoints
├── Data/              # DbContext
├── DTOs/              # Request/Response models
├── Models/            # Entity models
├── Repositories/      # Data access layer
├── Services/          # Business logic
└── Utilities/         # Helper functions
```


