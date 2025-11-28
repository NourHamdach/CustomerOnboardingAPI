using System;
using System.ComponentModel.DataAnnotations;

namespace CustomerOnboarding.Api.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }   // PK

        [Required]
        [StringLength(12, MinimumLength = 12)]
        public string ICNumber { get; set; } = string.Empty;   // UNIQUE

        [Required]
        [StringLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [StringLength(5)]
        public string PhoneCode { get; set; } = string.Empty;  // e.g., +60, +1, +44

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string EmailAddress { get; set; } = string.Empty;  // UNIQUE
        public bool VerifiedEmail { get; set; }
        public bool VerifiedMobile { get; set; }
        public bool TermsAgreed { get; set; } = false;
        public bool BiometricEnabled { get; set; } = false;
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        // Navigation
        public UserSecurity Security { get; set; } = null!;
    }
}
