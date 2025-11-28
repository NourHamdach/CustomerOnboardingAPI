using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CustomerOnboarding.Api.Models;

namespace CustomerOnboarding.Api.DTOs
{
    /// <summary>
    /// Request for sending OTP to user's email or mobile
    /// </summary>
 public class SendOTPRequest
{
    /// <summary>
    /// User ID obtained from registration
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Verification channel - Must be either 'EMAIL' or 'MOBILE' (case-sensitive)
    /// </summary>
    /// <example>EMAIL</example>
    [Required]
    [RegularExpression("^(EMAIL|MOBILE)$", ErrorMessage = "Must specify EMAIL or MOBILE.")]
    public string VerificationType { get; set; } = string.Empty;
}

    /// <summary>
    /// Request for verifying OTP code
    /// </summary>
   public class VerifyOTPRequest
{
    /// <summary>
    /// Attempt ID received from SendOTP response
    /// </summary>
    [Required]
    public int AttemptId { get; set; }

    /// <summary>
    /// 4-digit OTP code sent to user's email or mobile
    /// </summary>
    /// <example>1234</example>
    [Required]
    [StringLength(4, MinimumLength = 4)]
    public string OTPCode { get; set; } = string.Empty;
}

    // Response for OTP sending/verifying
   public class OTPResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ObfuscatedTarget { get; set; } // for UI
    public int? AttemptId { get; set; } // <-- added
    public string? OTPCode { get; set; } // NOTE: Only for testing - Remove in production
}

    // Request for changing email during migration
    public class ChangeEmailRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string NewEmailAddress { get; set; } = string.Empty;
    }

    // Response for change email
    public class ChangeEmailResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? AttemptId { get; set; }
    }
}
