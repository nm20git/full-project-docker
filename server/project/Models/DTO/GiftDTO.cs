using System.ComponentModel.DataAnnotations;
using static project.Models.Gift;

namespace project.Models.DTO
{
    //מודל לקריאה בלבד
    public class GiftDTO
    {
        public int Id { get; set; }

    
        public string Name { get; set; }


        public int Amount { get; set; }

       
        public int SponsorId { get; set; }

      
        public int Price { get; set; }

        public string ImageUrl { get; set; }

        public GiftCategory Category { get; set; }
        public bool IsDrawn { get; set; }
        public int PurchasesCount { get; set; }

    }
}
