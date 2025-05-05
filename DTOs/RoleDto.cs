using System.ComponentModel.DataAnnotations;

namespace UserManagement.DTOs
{
    public class RoleDto
    {
        public Guid Id { get; set; }

        [Required]
        public string RoleName { get; set; }

        public bool Active { get; set; }

        public List<AccessRightDto> AccessRights { get; set; } = new List<AccessRightDto>();
    }

    public class RoleCreateDto
    {
        [Required]
        public string RoleName { get; set; }

        public List<Guid> AccessRightIds { get; set; } = new List<Guid>();
    }

    public class RoleUpdateDto
    {
        public string RoleName { get; set; }

        public bool Active { get; set; }

        public List<Guid> AccessRightIds { get; set; } = new List<Guid>();
    }
}