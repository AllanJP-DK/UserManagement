using Microsoft.AspNetCore.Mvc;
using UserManagement.DTOs;
using UserManagement.Models;
using UserManagement.Repositories;
using UserManagement.Services;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IGenericRepository<AccessRight> _accessRightRepository;        

        public RolesController(
            IRoleRepository roleRepository,
            IGenericRepository<AccessRight> accessRightRepository)
        {
            _roleRepository = roleRepository;
            _accessRightRepository = accessRightRepository;            
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            var roles = await _roleRepository.GetAllRolesWithDetailsAsync();

            var roleDtos = roles.Select(r => new RoleDto
            {
                Id = r.Id,
                RoleName = r.RoleName,
                Active = r.Active,
                AccessRights = r.RoleAccessRights.Select(ra => new AccessRightDto
                {
                    Id = ra.AccessRight.Id,
                    Description = ra.AccessRight.Description
                }).ToList()
            }).ToList();

            return Ok(roleDtos);
        }

        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRole(Guid id)
        {
            var role = await _roleRepository.GetRoleWithDetailsAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            var roleDto = new RoleDto
            {
                Id = role.Id,
                RoleName = role.RoleName,
                Active = role.Active,
                AccessRights = role.RoleAccessRights.Select(ra => new AccessRightDto
                {
                    Id = ra.AccessRight.Id,
                    Description = ra.AccessRight.Description
                }).ToList()
            };

            return Ok(roleDto);
        }

        // POST: api/Roles
        [HttpPost]
        public async Task<ActionResult<RoleDto>> CreateRole(RoleCreateDto roleCreateDto)
        {
            // Check if role name is unique
            if (!await _roleRepository.IsRoleNameUniqueAsync(roleCreateDto.RoleName))
            {
                return BadRequest("Role name already exists.");
            }

            // Check if all access right IDs exist
            foreach (var accessRightId in roleCreateDto.AccessRightIds)
            {
                if (!await _accessRightRepository.ExistsAsync(accessRightId))
                {
                    return BadRequest($"Access right with ID {accessRightId} does not exist.");
                }
            }

            // Create new role
            var role = new Role
            {
                RoleName = roleCreateDto.RoleName,
                Active = true
            };

            await _roleRepository.AddAsync(role);
            await _roleRepository.SaveChangesAsync();

            // Add role access rights
            if (roleCreateDto.AccessRightIds.Any())
            {
                await _roleRepository.UpdateRoleAccessRightsAsync(role, roleCreateDto.AccessRightIds);
                await _roleRepository.SaveChangesAsync();
            }            

            // Return created role
            var createdRole = await _roleRepository.GetRoleWithDetailsAsync(role.Id);
            var roleDto = new RoleDto
            {
                Id = createdRole.Id,
                RoleName = createdRole.RoleName,
                Active = createdRole.Active,
                AccessRights = createdRole.RoleAccessRights.Select(ra => new AccessRightDto
                {
                    Id = ra.AccessRight.Id,
                    Description = ra.AccessRight.Description
                }).ToList()
            };

            return CreatedAtAction(nameof(GetRole), new { id = roleDto.Id }, roleDto);
        }

        // PUT: api/Roles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(Guid id, RoleUpdateDto roleUpdateDto)
        {
            var role = await _roleRepository.GetByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            // Check if role name is unique if provided
            if (roleUpdateDto.RoleName != null &&
                role.RoleName != roleUpdateDto.RoleName &&
                !await _roleRepository.IsRoleNameUniqueAsync(roleUpdateDto.RoleName, id))
            {
                return BadRequest("Role name already exists.");
            }

            // Update role properties
            if (roleUpdateDto.RoleName != null)
            {
                role.RoleName = roleUpdateDto.RoleName;
            }

            role.Active = roleUpdateDto.Active;

            _roleRepository.Update(role);

            // Update role access rights if provided
            if (roleUpdateDto.AccessRightIds != null)
            {
                // Check if all access right IDs exist
                foreach (var accessRightId in roleUpdateDto.AccessRightIds)
                {
                    if (!await _accessRightRepository.ExistsAsync(accessRightId))
                    {
                        return BadRequest($"Access right with ID {accessRightId} does not exist.");
                    }
                }

                await _roleRepository.UpdateRoleAccessRightsAsync(role, roleUpdateDto.AccessRightIds);
            }

            await _roleRepository.SaveChangesAsync();            

            return NoContent();
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            var role = await _roleRepository.GetByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            // Soft delete - just mark as inactive
            role.Active = false;
            _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync();            

            return NoContent();
        }
    }
}