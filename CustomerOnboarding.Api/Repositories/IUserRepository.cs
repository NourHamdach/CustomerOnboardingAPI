using CustomerOnboarding.Api.Models;

namespace CustomerOnboarding.Api.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByICNumberAsync(string icNumber);
        Task<User?> GetByIdAsync(int userId);
        Task AddAsync(User user);
        Task<IEnumerable<User>> GetAllAsync();
        Task SaveChangesAsync();
    }
}
