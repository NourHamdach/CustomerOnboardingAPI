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
- **Database:** PostgreSQL
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

## Setup

1. **Database:**
   ```bash
   # Update connection string in appsettings.json
   dotnet ef database update
   ```

2. **Run:**
   ```bash
   dotnet run
   ```

3. **Access:**
   - API: http://localhost:5000
   - Swagger: http://localhost:5000/swagger

## Configuration

Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=CustomerOnboardingDb;Username=postgres;Password=yourpassword"
  }
}
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

## License

MIT
