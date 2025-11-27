using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerOnboarding.Api.Models
{
    public enum OTPFlow
    {
        Registration,
        Migration,
        ChangeEmail
    }

    public class OTPAttempt
    {   
        [Key]
        public int AttemptId { get; set; }

        // Link OTP to User via UserId (FK)
        public int? UserId { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [Required]
        public OTPFlow Flow { get; set; } // Registration, Migration, or ChangeEmail

        [Required]
        [RegularExpression("^(MOBILE|EMAIL)$", ErrorMessage = "TargetType must be either 'MOBILE' or 'EMAIL'.")]
        public string TargetType { get; set; } // MOBILE or EMAIL

        [Required]
        [StringLength(200)]
        public string TargetValue { get; set; } // mobile or email address

        [Required]
        [StringLength(4, MinimumLength = 4)]
        public string OTPCode { get; set; }

        public DateTime CreationTime { get; set; }

        public bool IsVerified { get; set; } = false;

        public int AttemptCount { get; set; } = 0;

        public OTPAttempt()
        {
            CreationTime = DateTime.UtcNow;
        }
    }
}
