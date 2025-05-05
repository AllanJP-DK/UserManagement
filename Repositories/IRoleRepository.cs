using UserManagement.Models;

namespace UserManagement.Repositories
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role> GetRoleWithDetailsAsync(Guid id);
        Task<IEnumerable<Role>> GetAllRolesWithDetailsAsync();
        Task<bool> IsRoleNameUniqueAsync(string roleName, Guid? roleId = null);
        Task UpdateRoleAccessRightsAsync(Role role, List<Guid> accessRightIds);
    }
}