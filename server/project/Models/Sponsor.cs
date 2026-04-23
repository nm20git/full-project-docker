using System.ComponentModel.DataAnnotations;

namespace project.Models
{
    public class Sponsor
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public List<Gift> Gifts { get; set; } = new List<Gift>();
    }

}
