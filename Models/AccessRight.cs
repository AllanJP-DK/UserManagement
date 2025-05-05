using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Models
{
    [Table("accessrights")]
    public class AccessRight
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("description")]
        public string Description { get; set; }

        // Navigation property
        public ICollection<RoleAccessRight> RoleAccessRights { get; set; } = new List<RoleAccessRight>();
    }
}