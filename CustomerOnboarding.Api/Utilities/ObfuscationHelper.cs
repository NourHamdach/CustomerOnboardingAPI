using System.Text;

namespace CustomerOnboarding.Api.Utilities
{
    /// <summary>
    /// Helper class for obfuscating sensitive information like phone numbers and email addresses
    /// </summary>
    public static class ObfuscationHelper
    {
        /// <summary>
        /// Obfuscates a mobile number by showing only the last 4 digits
        /// Example: +60123456789 -> ******6789
        /// </summary>
        public static string ObfuscateMobile(string mobileNumber)
        {
            if (string.IsNullOrEmpty(mobileNumber) || mobileNumber.Length < 4)
                return "******";

            var lastFour = mobileNumber.Substring(mobileNumber.Length - 4);
            return $"******{lastFour}";
        }

        /// <summary>
        /// Obfuscates an email address by masking the username and domain
        /// Example: mariam.khan@gmail.com -> ma**@****.com
        /// </summary>
        public static string ObfuscateEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
                return "**@****.***";

            var parts = email.Split('@');
            if (parts.Length != 2)
                return "**@****.***";

            var username = parts[0];
            var domain = parts[1];

            // Obfuscate username: show first 2 chars + **
            var obfuscatedUsername = username.Length <= 2 
                ? "**" 
                : $"{username.Substring(0, 2)}**";

            // Obfuscate domain: show only extension
            var domainParts = domain.Split('.');
            var extension = domainParts.Length > 0 ? domainParts[domainParts.Length - 1] : "com";
            var obfuscatedDomain = $"****.{extension}";

            return $"{obfuscatedUsername}@{obfuscatedDomain}";
        }

        /// <summary>
        /// Obfuscates an IC number by showing only first 2 and last 2 digits
        /// Example: 880215106789 -> 88********89
        /// </summary>
        public static string ObfuscateICNumber(string icNumber)
        {
            if (string.IsNullOrEmpty(icNumber) || icNumber.Length < 4)
                return "**********";

            var firstTwo = icNumber.Substring(0, 2);
            var lastTwo = icNumber.Substring(icNumber.Length - 2);
            var maskLength = icNumber.Length - 4;
            var mask = new string('*', maskLength);

            return $"{firstTwo}{mask}{lastTwo}";
        }
    }
}
