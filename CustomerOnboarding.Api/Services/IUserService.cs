using CustomerOnboarding.Api.DTOs;
using CustomerOnboarding.Api.Models;

namespace CustomerOnboarding.Api.Services
{
    public interface IUserService
    {
        // ---------------- PATH A: New Account Creation ----------------
        Task<ICCheckResponse> CheckICNumberAsync(string icNumber);
        Task<UserResponse> StartOrContinueRegistrationAsync(RegisterUserRequest request);
        
        // Send/Verify OTP for new registration or migration
        Task<OTPResponse> SendOTPAsync(SendOTPRequest request, string? newContact = null);
        Task<OTPResponse> VerifyOTPAsync(VerifyOTPRequest request);

        Task<bool> AgreeTermsAsync(int userId);
        Task<bool> SetPINAsync(int userId, string pin, string confirmPin);
        Task<bool> SetBiometricAsync(int userId, bool enable);

        // ---------------- PATH B: Existing User Migration ----------------
        Task<MigrationResponse> InitiateMigrationAsync(string icNumber);
        Task<ChangeEmailResponse> ChangeEmailAsync(int userId, string newEmail);

        // ---------------- COMMON ----------------
        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
        Task<User?> GetUserByICAsync(string icNumber);
    }
}
