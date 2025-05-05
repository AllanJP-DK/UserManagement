using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Models
{
    [Table("users_roles")]
    public class UserRole
    {
        [Column("userid")]
        public Guid UserId { get; set; }

        [Column("roleid")]
        public Guid RoleId { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; }
    }
}