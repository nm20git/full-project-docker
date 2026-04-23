using System.ComponentModel.DataAnnotations;

namespace project.Models
{
    public enum UserRole
    {
        User,
        Admin
    }
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = "";

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = "";

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = "";

        [Phone]
        public string? Phone { get; set; }
        
        //סיסמה מוצפנת
        [Required]
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        
        //קידומת לסיסמה, למנוע כפילות
        [Required]
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

        public UserRole Role { get; set; } = UserRole.User;

        public List<Card> Cards { get; set; } = new List<Card>();
    }
}
