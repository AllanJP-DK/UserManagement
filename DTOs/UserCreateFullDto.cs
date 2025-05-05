// Add a new DTO
// DTOs/UserCreateFullDto.cs
using System.ComponentModel.DataAnnotations;

namespace UserManagement.DTOs
{
    public class UserCreateFullDto
    {
        [Required]
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string PostalCode { get; set; }

        public List<Guid> RoleIds { get; set; } = new List<Guid>();
    }
}