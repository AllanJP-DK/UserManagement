using System.ComponentModel.DataAnnotations;

namespace UserManagement.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Guid? AddressId { get; set; }

        public AddressDto Address { get; set; }

        public bool Active { get; set; }

        public List<RoleDto> Roles { get; set; } = new List<RoleDto>();
    }

    public class UserCreateDto
    {
        [Required]
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Guid? AddressId { get; set; }

        public List<Guid> RoleIds { get; set; } = new List<Guid>();
    }

    public class UserUpdateDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Guid? AddressId { get; set; }

        public List<Guid> RoleIds { get; set; } = new List<Guid>();

        public bool Active { get; set; }
    }
}