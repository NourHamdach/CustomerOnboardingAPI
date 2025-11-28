using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CustomerOnboarding.Api.DTOs
{
    /// <summary>
    /// Request for registering or updating user information
    /// </summary>
    public class RegisterUserRequest
    {
        /// <summary>
        /// Malaysian IC Number - Exactly 12 digits
        /// </summary>
        /// <example>990101015678</example>
        [Required]
        [StringLength(12, MinimumLength = 12)]
        public string ICNumber { get; set; } = string.Empty;

        /// <summary>
        /// Customer full name
        /// </summary>
        /// <example>John Doe</example>
        [Required]
        [StringLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Country/region phone code starting with + followed by 1-3 digits
        /// </summary>
        /// <example>+60</example>
        [Required(ErrorMessage = "Phone code is required.")]
        [RegularExpression(@"^\+\d{1,3}$", ErrorMessage = "Phone code must start with + followed by 1-3 digits (e.g., +1, +44, +60, +971).")]
        public string PhoneCode { get; set; } = string.Empty;

        /// <summary>
        /// Phone number - 4 to 15 digits only, no spaces or special characters
        /// </summary>
        /// <example>123456789</example>
        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "Phone number must be between 4 and 15 digits.")]
        [RegularExpression(@"^\d{4,15}$", ErrorMessage = "Phone number must contain only 4-15 digits without spaces or special characters.")]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Email address
        /// </summary>
        /// <example>john.doe@example.com</example>
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(150)]
        public string EmailAddress { get; set; } = string.Empty;
    }

    // Response DTO after registration
    public class UserResponse
    {
        public int UserId { get; set; }
        public string ICNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneCode { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public bool BiometricEnabled { get; set; }
        public DateTime RegistrationDate { get; set; }
    }

    /// <summary>
    /// Request for creating user PIN
    /// </summary>
    public class CreatePinRequest
    {
        /// <summary>
        /// 6-digit PIN - Numbers only (0-9)
        /// </summary>
        /// <example>123456</example>
        [Required(ErrorMessage = "PIN is required.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "PIN must be exactly 6 digits.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "PIN must contain only numbers (0-9).")]
        public string Pin { get; set; } = string.Empty;

        /// <summary>
        /// Confirm PIN - Must match the PIN
        /// </summary>
        /// <example>123456</example>
        [Required(ErrorMessage = "Confirm PIN is required.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Confirm PIN must be exactly 6 digits.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Confirm PIN must contain only numbers (0-9).")]
        public string ConfirmPin { get; set; } = string.Empty;
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
        public int? AttemptId { get; set; }
        public string? OTPCode { get; set; } // NOTE: Only for testing - Remove in production
    }
}
