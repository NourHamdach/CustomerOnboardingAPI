namespace CustomerOnboarding.Api.Utilities
{
    public static class HashingHelper
    {
        /// <summary>
        /// Hash a PIN or password using BCrypt
        /// </summary>
        public static string HashPIN(string pin)
        {
            return BCrypt.Net.BCrypt.HashPassword(pin);
        }

        /// <summary>
        /// Verify a PIN against a stored BCrypt hash
        /// </summary>
        public static bool VerifyPIN(string pin, string hashedPIN)
        {
            return BCrypt.Net.BCrypt.Verify(pin, hashedPIN);
        }
    }
}
