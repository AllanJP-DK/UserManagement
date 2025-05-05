using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Models
{
    [Table("adresses")]
    public class Address
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("street")]
        public string Street { get; set; }

        [Required]
        [Column("postalcode")]
        public string PostalCode { get; set; }

        // Navigation property
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}