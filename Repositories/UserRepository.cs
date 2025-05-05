using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;

namespace UserManagement.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User> GetUserWithDetailsAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.Address)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RoleAccessRights)
                            .ThenInclude(ra => ra.AccessRight)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<User>> GetAllUsersWithDetailsAsync()
        {
            return await _context.Users
                .Include(u => u.Address)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RoleAccessRights)
                            .ThenInclude(ra => ra.AccessRight)
                .Where(u => u.Active)
                .ToListAsync();
        }

        public async Task<bool> IsUsernameUniqueAsync(string username, Guid? userId = null)
        {
            var normalizedUsername = username.ToLower();
            if (userId.HasValue)
            {
                return !await _context.Users
                    .AnyAsync(u => u.Username.ToLower() == normalizedUsername && u.Id != userId.Value && u.Active);
            }

            return !await _context.Users
                .AnyAsync(u => u.Username.ToLower() == normalizedUsername && u.Active);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var normalizedUsername = username.ToLower();
            return await _context.Users
                .Include(u => u.Address)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username.ToLower() == normalizedUsername && u.Active);
        }

        public async Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId)
        {
            var userRoles = await _context.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId && ur.Role.Active)
                .Select(ur => ur.Role)
                .ToListAsync();

            return userRoles;
        }

        public async Task UpdateUserRolesAsync(User user, List<Guid> roleIds)
        {
            // Get current user roles
            var currentUserRoles = await _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .ToListAsync();

            // Remove roles that are not in the updated list
            foreach (var userRole in currentUserRoles)
            {
                if (!roleIds.Contains(userRole.RoleId))
                {
                    _context.UserRoles.Remove(userRole);
                }
            }

            // Add new roles
            foreach (var roleId in roleIds)
            {
                if (!currentUserRoles.Any(ur => ur.RoleId == roleId))
                {
                    await _context.UserRoles.AddAsync(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = roleId
                    });
                }
            }
        }
    }
}