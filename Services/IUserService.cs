// Services/IUserService.cs
using UserManagement.Models;

namespace UserManagement.Services
{
    public interface IUserService
    {
        Task<User> CreateUserWithDetailsAsync(string username, string firstName, string lastName,
            string street, string postalCode, List<Guid> roleIds);
    }
}