using System.ComponentModel.DataAnnotations;

namespace project.Models
{
    public class Raffle
    {
        public int Id { get; set; }
        [Required]
        public int GiftId { get; set; }
        public Gift Gift { get; set; }
        public int WinnerId { get; set; }
        public User Winner { get; set; }
    }
}
