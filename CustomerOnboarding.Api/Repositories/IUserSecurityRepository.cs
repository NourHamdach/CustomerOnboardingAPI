using CustomerOnboarding.Api.Models;

namespace CustomerOnboarding.Api.Repositories
{
    public interface IUserSecurityRepository
    {
        Task<UserSecurity?> GetByUserIdAsync(int userId);
        Task AddAsync(UserSecurity security);
        Task SaveChangesAsync();
    }
}
