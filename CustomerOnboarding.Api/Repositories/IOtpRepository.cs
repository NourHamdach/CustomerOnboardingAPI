using CustomerOnboarding.Api.Models;

namespace CustomerOnboarding.Api.Repositories
{
    public interface IOTPRepository
    {
        Task<OTPAttempt?> GetLatestByTargetAsync(string target);
        Task<OTPAttempt?> GetByIdAsync(int attemptId); // <-- added
        Task AddAsync(OTPAttempt otp);
        Task SaveChangesAsync();
    }
}
