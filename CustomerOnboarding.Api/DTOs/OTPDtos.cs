using System.ComponentModel.DataAnnotations;
using CustomerOnboarding.Api.Models;

namespace CustomerOnboarding.Api.DTOs
{
    // Request for sending OTP based on UserId
 public class SendOTPRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [RegularExpression("^(EMAIL|MOBILE)$", ErrorMessage = "Must specify EMAIL or MOBILE.")]
    public string VerificationType { get; set; } // CHANNEL SELECTION
}

    // Request for verifying OTP
   public class VerifyOTPRequest
{
    [Required]
    public int AttemptId { get; set; }

    [Required]
    [StringLength(4, MinimumLength = 4)]
    public string OTPCode { get; set; }
}

    // Response for OTP sending/verifying
   public class OTPResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ObfuscatedTarget { get; set; } // for UI
    public int? AttemptId { get; set; } // <-- added
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
