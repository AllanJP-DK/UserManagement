using System.Transactions;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Repositories;

namespace UserManagement.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserRepository _userRepository;        

        public UserService(
            ApplicationDbContext context,
            IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;            
        }

        public async Task<User> CreateUserWithDetailsAsync(string username, string firstName, string lastName,
    string street, string postalCode, List<Guid> roleIds)
        {
            // Check if username is unique before starting transaction
            if (!await _userRepository.IsUsernameUniqueAsync(username))
            {
                throw new Exception("Username already exists.");
            }

            // Verify roles exist before starting transaction
            foreach (var roleId in roleIds)
            {
                if (!await _context.Roles.AnyAsync(r => r.Id == roleId))
                {
                    throw new Exception($"Role with ID {roleId} does not exist.");
                }
            }

            // Store the user ID for later retrieval
            Guid userId;

            // Use TransactionScope for distributed transaction support
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // 1. Create Address
                    var address = new Address
                    {
                        Street = street,
                        PostalCode = postalCode
                    };
                    await _context.Addresses.AddAsync(address);
                    await _context.SaveChangesAsync();

                    // 2. Create User
                    var user = new User
                    {
                        Username = username,
                        FirstName = firstName,
                        LastName = lastName,
                        AddressId = address.Id,
                        Active = true
                    };
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();

                    userId = user.Id;

                    // 3. Assign Roles
                    foreach (var roleId in roleIds)
                    {
                        await _context.UserRoles.AddAsync(new UserRole
                        {
                            UserId = user.Id,
                            RoleId = roleId
                        });
                    }
                    await _context.SaveChangesAsync();

                    // Complete the transaction
                    transactionScope.Complete();
                }
                catch (Exception)
                {
                    // Transaction will automatically be aborted if we reach here
                    throw;
                }
            }

            // After the transaction is complete, fetch and return the user with full details
            return await _context.Users
                .Include(u => u.Address)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RoleAccessRights)
                            .ThenInclude(ra => ra.AccessRight)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}