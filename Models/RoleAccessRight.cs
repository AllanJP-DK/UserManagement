using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Models
{
    [Table("roles_accessrights")]
    public class RoleAccessRight
    {
        [Column("roleid")]
        public Guid RoleId { get; set; }

        [Column("accessrightid")]
        public Guid AccessRightId { get; set; }

        // Navigation properties
        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        [ForeignKey("AccessRightId")]
        public AccessRight AccessRight { get; set; }
    }
}