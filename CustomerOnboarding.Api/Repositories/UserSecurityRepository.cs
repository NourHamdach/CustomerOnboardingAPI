using CustomerOnboarding.Api.Data;
using CustomerOnboarding.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerOnboarding.Api.Repositories
{
    public class UserSecurityRepository : IUserSecurityRepository
    {
        private readonly ApplicationDbContext _context;
        public UserSecurityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserSecurity?> GetByUserIdAsync(int userId)
        {
            return await _context.UserSecurity.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task AddAsync(UserSecurity security)
        {
            await _context.UserSecurity.AddAsync(security);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
