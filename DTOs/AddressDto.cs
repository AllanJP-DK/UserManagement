using System.ComponentModel.DataAnnotations;

namespace UserManagement.DTOs
{
    public class AddressDto
    {
        public Guid Id { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string PostalCode { get; set; }
    }

    public class AddressCreateDto
    {
        [Required]
        public string Street { get; set; }

        [Required]
        public string PostalCode { get; set; }
    }
}