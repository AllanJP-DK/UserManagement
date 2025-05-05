using UserManagement.Models;

namespace UserManagement.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetUserWithDetailsAsync(Guid id);
        Task<IEnumerable<User>> GetAllUsersWithDetailsAsync();
        Task<bool> IsUsernameUniqueAsync(string username, Guid? userId = null);
        Task<User> GetUserByUsernameAsync(string username);
        Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId);
        Task UpdateUserRolesAsync(User user, List<Guid> roleIds);
    }
}