using CustomerOnboarding.Api.Data;
using CustomerOnboarding.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerOnboarding.Api.Repositories
{
    public class OTPRepository : IOTPRepository
    {
        private readonly ApplicationDbContext _context;
        public OTPRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OTPAttempt?> GetLatestByTargetAsync(string target)
        {
            return await _context.OTPAttempts
                .Where(o => o.TargetValue == target && !o.IsVerified)
                .OrderByDescending(o => o.CreationTime)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(OTPAttempt otp)
        {
            await _context.OTPAttempts.AddAsync(otp);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<OTPAttempt?> GetByIdAsync(int attemptId)
{
    return await _context.OTPAttempts.FirstOrDefaultAsync(o => o.AttemptId == attemptId);
}

    }
}
