using System.ComponentModel.DataAnnotations;

namespace project.Models
{
    public class Gift
    {
        public enum GiftCategory
        {
            Vacation = 1,        // חופשה
            Furniture = 2,       // ריהוט
            Clothing = 3,        // ביגוד
            Toys = 4,            // צעצועים
            Beauty = 5,          // טיפוח
            Car = 6,            //רכב   
            Other = 7           // אחר
        }

        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public int Amount { get; set; }
        public Sponsor sponsor { get; set; }
        [Required]
        public int SponsorId { get; set; }
        
        public string ImageUrl { get; set; }

        public bool IsDrawn { get; set; } = false;

        [Required]
        public int Price { get; set; }

        [Required]

        public GiftCategory Category { get; set; }
        public List<Raffle> Winners { get; set; } = new List<Raffle>();

        public List<Card> Cards { get; set; } = new List<Card>();
    }
}