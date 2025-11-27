using System;
using System.ComponentModel.DataAnnotations;

namespace CustomerOnboarding.Api.DTOs
{
    // Request DTO for registering or updating user
    public class RegisterUserRequest
    {
        [Required]
        [StringLength(12, MinimumLength = 12)]
        public string ICNumber { get; set; }

        [Required]
        [StringLength(200)]
        public string CustomerName { get; set; }

        [Required]
        [StringLength(20)]
        public string MobileNumber { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(150)]
        public string EmailAddress { get; set; }
    }

    // Response DTO after registration
    public class UserResponse
    {
        public int UserId { get; set; }
        public string ICNumber { get; set; }
        public string CustomerName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public bool BiometricEnabled { get; set; }
        public DateTime RegistrationDate { get; set; }
    }

    // DTO for PIN creation
    public class CreatePinRequest
    {
        [Required(ErrorMessage = "PIN is required.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "PIN must be exactly 6 digits.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "PIN must contain only numbers (0-9).")]
        public string Pin { get; set; }

        [Required(ErrorMessage = "Confirm PIN is required.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Confirm PIN must be exactly 6 digits.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Confirm PIN must contain only numbers (0-9).")]
        public string ConfirmPin { get; set; }
    }

    // DTO for enabling biometric
    public class EnableBiometricRequest
    {
        public bool Enable { get; set; }
    }

    // Response for IC Number check (Step 1 validation)
    public class ICCheckResponse
    {
        public string Status { get; set; } = string.Empty; // "NEW", "EXISTS", "MIGRATION"
        public string Message { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty; // "START", "LOGIN", "MIGRATE"
        public int? UserId { get; set; } // Return UserId if user exists
        public string? ObfuscatedMobile { get; set; }
        public string? ObfuscatedEmail { get; set; }
    }

    // Request for migration flow
    public class MigrationRequest
    {
        [Required]
        [StringLength(12, MinimumLength = 12)]
        public string ICNumber { get; set; } = string.Empty;
    }

    // Response for migration initiation
    public class MigrationResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public string? ObfuscatedMobile { get; set; }
        public string? ObfuscatedEmail { get; set; }
    }
}
