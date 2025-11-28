# Customer Onboarding API

ASP.NET Core 8.0 REST API for customer onboarding with OTP verification.

## Features

- IC number validation and registration
- Email and mobile OTP verification
- Multi-flow OTP system (Registration, Migration, Change Email)
- PIN creation with validation
- Biometric enrollment
- Email change functionality
- User migration support

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
1. `POST /api/Users/migration/initiate` - Initiate migration (OTP automatically sent to mobile)
2. `POST /api/Users/verify-otp` - Verify OTP

### Change Email Flow
1. `POST /api/Users/migration/change-email` - Change email (OTP sent to new email)
2. `POST /api/Users/verify-otp` - Verify OTP

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

## API Testing Guide

### Complete Registration Flow (New User)

This is the complete step-by-step process for registering a new user with full verification.

#### Step 1: Check IC Number
**Endpoint:** `GET /api/Users/check-ic/{icNumber}`

**Example:**
```http
GET /api/Users/check-ic/990101015678
```

**Expected Response (New User):**
```json
{
  "status": "NEW",
  "message": "IC not found. Start new registration.",
  "action": "START",
  "userId": null
}
```

#### Step 2: Register User
**Endpoint:** `POST /api/Users/registration`

**Request Body:**
```json
{
  "icNumber": "990101015678",
  "customerName": "John Doe",
  "phoneCode": "+60",
  "phoneNumber": "123456789",
  "emailAddress": "john.doe@example.com"
}
```

**Expected Response:**
```json
{
  "userId": 1,
  "icNumber": "990101015678",
  "customerName": "John Doe",
  "phoneCode": "+60",
  "phoneNumber": "123456789",
  "emailAddress": "john.doe@example.com",
  "biometricEnabled": false,
  "registrationDate": "2025-11-28T08:30:00Z"
}
```

**Important:** Save the `userId` - you'll need it for all subsequent steps.

#### Step 3: Send OTP to Email
**Endpoint:** `POST /api/Users/send-otp`

**Request Body:**
```json
{
  "userId": 1,
  "verificationType": "EMAIL"
}
```

**Expected Response:**
```json
{
  "isSuccess": true,
  "message": "OTP sent successfully",
  "obfuscatedTarget": "j***@example.com",
  "attemptId": 1,
  "otpCode": "1234"
}
```

**Important:** 
- Save the `attemptId` for verification
- The `otpCode` is shown for testing only (will be removed in production)
- `verificationType` must be either "EMAIL" or "MOBILE" (case-sensitive)

#### Step 4: Verify Email OTP
**Endpoint:** `POST /api/Users/verify-otp`

**Request Body:**
```json
{
  "attemptId": 1,
  "otpCode": "1234"
}
```

**Expected Response:**
```json
{
  "isSuccess": true,
  "message": "OTP verified. Please verify your other contact.",
  "obfuscatedTarget": null,
  "attemptId": null
}
```

#### Step 5: Send OTP to Mobile
**Endpoint:** `POST /api/Users/send-otp`

**Request Body:**
```json
{
  "userId": 1,
  "verificationType": "MOBILE"
}
```

**Expected Response:**
```json
{
  "isSuccess": true,
  "message": "OTP sent successfully",
  "obfuscatedTarget": "+60***6789",
  "attemptId": 2,
  "otpCode": "5678"
}
```

#### Step 6: Verify Mobile OTP
**Endpoint:** `POST /api/Users/verify-otp`

**Request Body:**
```json
{
  "attemptId": 2,
  "otpCode": "5678"
}
```

**Expected Response:**
```json
{
  "isSuccess": true,
  "message": "Registration completed! Please set your PIN and accept terms.",
  "obfuscatedTarget": null,
  "attemptId": null
}
```

#### Step 7: Accept Terms and Conditions
**Endpoint:** `POST /api/Users/agree-terms/{userId}`

**Example:**
```http
POST /api/Users/agree-terms/1
```

**Expected Response:**
```json
{
  "userId": 1,
  "icNumber": "990101015678",
  "customerName": "John Doe",
  "phoneCode": "+60",
  "phoneNumber": "123456789",
  "emailAddress": "john.doe@example.com",
  "biometricEnabled": false,
  "registrationDate": "2025-11-28T08:30:00Z"
}
```

#### Step 8: Create PIN
**Endpoint:** `POST /api/Users/pin/{userId}`

**Request Body:**
```json
{
  "pin": "123456",
  "confirmPin": "123456"
}
```

**Expected Response:**
```json
{
  "userId": 1,
  "icNumber": "990101015678",
  "customerName": "John Doe",
  "phoneCode": "+60",
  "phoneNumber": "123456789",
  "emailAddress": "john.doe@example.com",
  "biometricEnabled": false,
  "registrationDate": "2025-11-28T08:30:00Z"
}
```

**PIN Requirements:**
- Exactly 6 digits
- Numbers only (0-9)
- No letters or special characters

#### Step 9: Enable Biometric (Optional)
**Endpoint:** `POST /api/Users/biometric/{userId}`

**Request Body:**
```json
{
  "enable": true
}
```

**Expected Response:**
```json
{
  "userId": 1,
  "icNumber": "990101015678",
  "customerName": "John Doe",
  "phoneCode": "+60",
  "phoneNumber": "123456789",
  "emailAddress": "john.doe@example.com",
  "biometricEnabled": true,
  "registrationDate": "2025-11-28T08:30:00Z"
}
```

**Registration Complete!**

---

### Migration Flow (Existing User)

For users who are already fully registered and want to migrate or verify their identity.

#### Step 1: Initiate Migration (OTP Automatically Sent)
**Endpoint:** `POST /api/Users/migration/initiate`

**Request Body:**
```json
{
  "icNumber": "990101015678"
}
```

**Expected Response:**
```json
{
  "isSuccess": true,
  "message": "Migration initiated. OTP sent to +60***6789. Please verify to continue.",
  "userId": 1,
  "obfuscatedMobile": "+60***6789",
  "obfuscatedEmail": "j***@example.com",
  "attemptId": 3,
  "otpCode": "9876"
}
```

**Important:** 
- OTP is automatically sent to the user's mobile number
- Save the `attemptId` for verification
- The `otpCode` is shown for testing only

#### Step 2: Verify OTP
**Endpoint:** `POST /api/Users/verify-otp`

**Request Body:**
```json
{
  "attemptId": 3,
  "otpCode": "9876"
}
```

**Expected Response:**
```json
{
  "isSuccess": true,
  "message": "OTP verified successfully.",
  "obfuscatedTarget": null,
  "attemptId": null
}
```

---

### Change Email Flow (Fully Verified Users)

#### Step 1: Request Email Change
**Endpoint:** `POST /api/Users/migration/change-email`

**Request Body:**
```json
{
  "userId": 1,
  "newEmailAddress": "newemail@example.com"
}
```

**Expected Response:**
```json
{
  "isSuccess": true,
  "message": "OTP sent to new email n***@example.com. Please verify to complete the change.",
  "attemptId": 4
}
```

#### Step 2: Verify New Email with OTP
**Endpoint:** `POST /api/Users/verify-otp`

**Request Body:**
```json
{
  "attemptId": 4,
  "otpCode": "4321"
}
```

**Expected Response:**
```json
{
  "isSuccess": true,
  "message": "Email changed successfully.",
  "obfuscatedTarget": null,
  "attemptId": null
}
```

**Email Changed Successfully!**

---

### Get All Users (Testing)
**Endpoint:** `GET /api/Users`

**Expected Response:**
```json
[
  {
    "userId": 1,
    "icNumber": "990101015678",
    "customerName": "John Doe",
    "phoneCode": "+60",
    "phoneNumber": "123456789",
    "emailAddress": "newemail@example.com",
    "biometricEnabled": true,
    "registrationDate": "2025-11-28T08:30:00Z"
  }
]
```

---

## Important Testing Notes

### Field Validations

| Field | Format | Example | Notes |
|-------|--------|---------|-------|
| **IC Number** | Exactly 12 digits | `990101015678` | Malaysian IC format |
| **Phone Code** | `+` followed by 1-3 digits | `+60`, `+1`, `+44` | Must start with + |
| **Phone Number** | 4-15 digits only | `123456789` | No spaces or special characters |
| **Email** | Valid email format | `user@example.com` | Standard email validation |
| **PIN** | Exactly 6 digits | `123456` | Numbers only (0-9) |
| **OTP Code** | Exactly 4 digits | `1234` | Sent via email/mobile |
| **Verification Type** | `EMAIL` or `MOBILE` | `EMAIL` | Case-sensitive |

### OTP System Behavior

1. **Automatic Flow Detection:** The system automatically determines if OTP is for:
   - **Registration:** User is not fully verified
   - **Migration:** User is fully verified (both email and mobile) - OTP automatically sent to mobile when initiating migration
   - **Change Email:** Explicitly changing email address - OTP automatically sent to new email

2. **OTP Expiration:** OTP codes expire after 5 minutes

3. **Testing Mode:** OTP codes are returned in the response (remove in production)

4. **Automatic OTP Sending:**
   - **Migration Initiate:** Automatically sends OTP to user's mobile number
   - **Change Email:** Automatically sends OTP to the new email address
   - **Registration:** Manual OTP sending via `/send-otp` endpoint

### Common Error Scenarios

| Error | Reason | Solution |
|-------|--------|----------|
| `User not found` | Invalid userId | Use the userId from registration response |
| `Invalid verification type` | Wrong VerificationType | Use "EMAIL" or "MOBILE" (case-sensitive) |
| `Incorrect OTP` | Wrong OTP code | Check the `otpCode` in the SendOTP response |
| `OTP expired` | OTP older than 5 minutes | Request a new OTP |
| `PIN must be exactly 6 digits` | Invalid PIN format | Use 6 digits only (e.g., 123456) |
| `Phone code must start with +` | Missing + in phone code | Use format: +60, +1, +44 |

### Testing with Swagger UI

1. Open Swagger UI at `http://localhost:5000/swagger`
2. Click on any endpoint to expand it
3. Click "Try it out"
4. Fill in the example values (or use custom values)
5. Click "Execute"
6. View the response below

**Swagger Tips:**
- All request examples are pre-filled with valid formats
- Field descriptions show requirements and examples
- Error messages explain exactly what's wrong
- The API description at the top lists all important format rules

---

## Security Features

- OTP expiration (5 minutes)
- Email format validation
- Phone code validation (+1-3 digits)
- Phone number validation (4-15 digits)
- PIN must be 6 digits (numbers only)
- IC number must be exactly 12 digits
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


