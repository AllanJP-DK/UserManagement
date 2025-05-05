using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Models
{
    [Table("roles")]
    public class Role
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("rolename")]
        public string RoleName { get; set; }

        [Column("active")]
        public bool Active { get; set; } = true;

        // Navigation properties
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RoleAccessRight> RoleAccessRights { get; set; } = new List<RoleAccessRight>();
    }
}