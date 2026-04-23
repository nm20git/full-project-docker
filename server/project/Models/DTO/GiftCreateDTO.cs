using static project.Models.Gift;
using System.ComponentModel.DataAnnotations;

namespace project.Models.DTO
{
    public class GiftCreateDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Gift name is required")]
        [MinLength(2, ErrorMessage = "Gift name must be at least 2 characters")]
        [MaxLength(100, ErrorMessage = "Gift name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        public int Amount { get; set; }

        [Required(ErrorMessage = "SponsorId is required")]
        public int SponsorId { get; set; }

        [Required(ErrorMessage = "Price is required")]
        public int Price { get; set; }

        public IFormFile? Image { get; set; }   // חדש

        [Required(ErrorMessage = "Category is required")]
        public GiftCategory Category { get; set; }
    }
}
