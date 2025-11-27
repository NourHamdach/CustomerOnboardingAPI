using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerOnboarding.Api.Models
{
	public class UserSecurity
	{
		[Key, ForeignKey("User")]
		public int UserId { get; set; }   // PK + FK

        [Required]
        public string HashedPIN { get; set; }
        
        public DateTime PINLastUpdated { get; set; } = DateTime.UtcNow;		public int FailedAttempts { get; set; } = 0;

		// Navigation
		public User User { get; set; }
	}
}
