using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Models
{
    [Table("auditlogs")]
    public class AuditLog
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("table_name")]
        public string TableName { get; set; }

        [Required]
        [Column("operation")]
        public string Operation { get; set; }

        [Required]
        [Column("changed_at")]
        public DateTime ChangedAt { get; set; }

        [Required]
        [Column("userid")]
        public Guid UserId { get; set; }

        // Navigation property
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}