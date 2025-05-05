using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;

namespace UserManagement.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Role> GetRoleWithDetailsAsync(Guid id)
        {
            return await _context.Roles
                .Include(r => r.RoleAccessRights)
                    .ThenInclude(ra => ra.AccessRight)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Role>> GetAllRolesWithDetailsAsync()
        {
            return await _context.Roles
                .Include(r => r.RoleAccessRights)
                    .ThenInclude(ra => ra.AccessRight)
                .Where(r => r.Active)
                .ToListAsync();
        }

        public async Task<bool> IsRoleNameUniqueAsync(string roleName, Guid? roleId = null)
        {
            var normalizedRoleName = roleName.ToLower();
            if (roleId.HasValue)
            {
                return !await _context.Roles
                    .AnyAsync(r => r.RoleName.ToLower() == normalizedRoleName && r.Id != roleId.Value && r.Active);
            }

            return !await _context.Roles
                .AnyAsync(r => r.RoleName.ToLower() == normalizedRoleName && r.Active);
        }

        public async Task UpdateRoleAccessRightsAsync(Role role, List<Guid> accessRightIds)
        {
            // Get current role access rights
            var currentRoleAccessRights = await _context.RoleAccessRights
                .Where(ra => ra.RoleId == role.Id)
                .ToListAsync();

            // Remove access rights that are not in the updated list
            foreach (var roleAccessRight in currentRoleAccessRights)
            {
                if (!accessRightIds.Contains(roleAccessRight.AccessRightId))
                {
                    _context.RoleAccessRights.Remove(roleAccessRight);
                }
            }

            // Add new access rights
            foreach (var accessRightId in accessRightIds)
            {
                if (!currentRoleAccessRights.Any(ra => ra.AccessRightId == accessRightId))
                {
                    await _context.RoleAccessRights.AddAsync(new RoleAccessRight
                    {
                        RoleId = role.Id,
                        AccessRightId = accessRightId
                    });
                }
            }
        }
    }
}