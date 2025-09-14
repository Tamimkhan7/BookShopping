using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BookShopping.Models.DTOs
{
    public class BookDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string? BookName { get; set; }

        [Required]
        [MaxLength(40)]
        public string? AuthorName { get; set; }

        [Required]
        public double Price { get; set; }

        // ✅ Discount property added
        [Range(0, 100)]
        public double DiscountPercentage { get; set; } = 0;

        public string? Image { get; set; }

        [Required]
        public int GenreId { get; set; }

        public IFormFile? ImageFile { get; set; }

        // Use Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        public IEnumerable<SelectListItem>? GenreList { get; set; }
    }
}
