using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookShopping.Models
{
    public class Review
    {
        public int Id { get; set; }
        [Required]
        public int BookId { get; set; }
        [Required]
        public string UserId { get; set; }  // relaton between identity user
        [Range(1, 5)]
        public int Rating { get; set; }
        [MaxLength(500)]
        public string? Comment { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        public Book Book { get; set; }
        public IdentityUser User { get; set; }

    }
}
