using Microsoft.AspNetCore.Mvc;
using UserManagement.DTOs;
using UserManagement.Models;
using UserManagement.Repositories;
using UserManagement.Services;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;        
        private readonly IUserService _userService;

        public UsersController(
            IUserRepository userRepository,
            IRoleRepository roleRepository,            
            IUserService userService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;            
            _userService = userService;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userRepository.GetAllUsersWithDetailsAsync();

            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                FirstName = u.FirstName,
                LastName = u.LastName,
                AddressId = u.AddressId,
                Address = u.Address != null ? new AddressDto
                {
                    Id = u.Address.Id,
                    Street = u.Address.Street,
                    PostalCode = u.Address.PostalCode
                } : null,
                Active = u.Active,
                Roles = u.UserRoles.Select(ur => new RoleDto
                {
                    Id = ur.Role.Id,
                    RoleName = ur.Role.RoleName,
                    Active = ur.Role.Active,
                    AccessRights = ur.Role.RoleAccessRights.Select(ra => new AccessRightDto
                    {
                        Id = ra.AccessRight.Id,
                        Description = ra.AccessRight.Description
                    }).ToList()
                }).ToList()
            }).ToList();

            return Ok(userDtos);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(Guid id)
        {
            var user = await _userRepository.GetUserWithDetailsAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AddressId = user.AddressId,
                Address = user.Address != null ? new AddressDto
                {
                    Id = user.Address.Id,
                    Street = user.Address.Street,
                    PostalCode = user.Address.PostalCode
                } : null,
                Active = user.Active,
                Roles = user.UserRoles.Select(ur => new RoleDto
                {
                    Id = ur.Role.Id,
                    RoleName = ur.Role.RoleName,
                    Active = ur.Role.Active,
                    AccessRights = ur.Role.RoleAccessRights.Select(ra => new AccessRightDto
                    {
                        Id = ra.AccessRight.Id,
                        Description = ra.AccessRight.Description
                    }).ToList()
                }).ToList()
            };

            return Ok(userDto);
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(UserCreateDto userCreateDto)
        {
            // Check if username is unique
            if (!await _userRepository.IsUsernameUniqueAsync(userCreateDto.Username))
            {
                return BadRequest("Username already exists.");
            }

            // Check if all role IDs exist
            foreach (var roleId in userCreateDto.RoleIds)
            {
                if (!await _roleRepository.ExistsAsync(roleId))
                {
                    return BadRequest($"Role with ID {roleId} does not exist.");
                }
            }

            // Create new user
            var user = new User
            {
                Username = userCreateDto.Username,
                FirstName = userCreateDto.FirstName,
                LastName = userCreateDto.LastName,
                AddressId = userCreateDto.AddressId,
                Active = true
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Add user roles
            if (userCreateDto.RoleIds.Any())
            {
                await _userRepository.UpdateUserRolesAsync(user, userCreateDto.RoleIds);
                await _userRepository.SaveChangesAsync();
            }            

            // Return created user
            var createdUser = await _userRepository.GetUserWithDetailsAsync(user.Id);
            var userDto = new UserDto
            {
                Id = createdUser.Id,
                Username = createdUser.Username,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                AddressId = createdUser.AddressId,
                Address = createdUser.Address != null ? new AddressDto
                {
                    Id = createdUser.Address.Id,
                    Street = createdUser.Address.Street,
                    PostalCode = createdUser.Address.PostalCode
                } : null,
                Active = createdUser.Active,
                Roles = createdUser.UserRoles.Select(ur => new RoleDto
                {
                    Id = ur.Role.Id,
                    RoleName = ur.Role.RoleName,
                    Active = ur.Role.Active
                }).ToList()
            };

            return CreatedAtAction(nameof(GetUser), new { id = userDto.Id }, userDto);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, UserUpdateDto userUpdateDto)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Update user properties
            user.FirstName = userUpdateDto.FirstName ?? user.FirstName;
            user.LastName = userUpdateDto.LastName ?? user.LastName;
            user.AddressId = userUpdateDto.AddressId ?? user.AddressId;
            user.Active = userUpdateDto.Active;

            _userRepository.Update(user);

            // Update user roles if provided
            if (userUpdateDto.RoleIds != null && userUpdateDto.RoleIds.Any())
            {
                // Check if all role IDs exist
                foreach (var roleId in userUpdateDto.RoleIds)
                {
                    if (!await _roleRepository.ExistsAsync(roleId))
                    {
                        return BadRequest($"Role with ID {roleId} does not exist.");
                    }
                }

                await _userRepository.UpdateUserRolesAsync(user, userUpdateDto.RoleIds);
            }

            await _userRepository.SaveChangesAsync();            

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Soft delete - just mark as inactive
            user.Active = false;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();            

            return NoContent();
        }

        // POST: api/Users/CreateWithDetails
        [HttpPost("CreateWithDetails")]
        public async Task<ActionResult<UserDto>> CreateUserWithDetails(UserCreateFullDto dto)
        {
            try
            {
                var user = await _userService.CreateUserWithDetailsAsync(
                    dto.Username,
                    dto.FirstName,
                    dto.LastName,
                    dto.Street,
                    dto.PostalCode,
                    dto.RoleIds);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    AddressId = user.AddressId,
                    Address = user.Address != null ? new AddressDto
                    {
                        Id = user.Address.Id,
                        Street = user.Address.Street,
                        PostalCode = user.Address.PostalCode
                    } : null,
                    Active = user.Active,
                    Roles = user.UserRoles.Select(ur => new RoleDto
                    {
                        Id = ur.Role.Id,
                        RoleName = ur.Role.RoleName,
                        Active = ur.Role.Active,
                        AccessRights = ur.Role.RoleAccessRights.Select(ra => new AccessRightDto
                        {
                            Id = ra.AccessRight.Id,
                            Description = ra.AccessRight.Description
                        }).ToList()
                    }).ToList()
                };

                return CreatedAtAction(nameof(GetUser), new { id = userDto.Id }, userDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}