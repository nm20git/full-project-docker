using System.ComponentModel.DataAnnotations;

namespace project.Models.DTO
{
    public class RaffleDTO
    {
        [Required(ErrorMessage = "GiftId is required")]
        public int GiftId { get; set; }
    }
}