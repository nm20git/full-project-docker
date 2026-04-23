using System.ComponentModel.DataAnnotations;

namespace project.Models.DTO
{
    public class CardDTO
    {
        [Required(ErrorMessage = "UserId is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "GiftId is required")]
        public int GiftId { get; set; }
        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }

    }
}
