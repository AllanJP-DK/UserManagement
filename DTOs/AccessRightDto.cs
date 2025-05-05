using System.ComponentModel.DataAnnotations;

namespace UserManagement.DTOs
{
    public class AccessRightDto
    {
        public Guid Id { get; set; }

        [Required]
        public string Description { get; set; }
    }

    public class AccessRightCreateDto
    {
        [Required]
        public string Description { get; set; }
    }
}