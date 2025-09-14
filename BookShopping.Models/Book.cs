using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShopping.Models
{
    public class Book
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

        // ✅ Discount field
        [Range(0, 100)]
        public double DiscountPercentage { get; set; } = 0;

        // ✅ Computed Property
        [NotMapped]
        public double DiscountedPrice
        {
            get
            {
                if (DiscountPercentage > 0)
                    return Math.Round(Price - (Price * DiscountPercentage / 100), 2);
                return Price;
            }
        }

        public string? Image { get; set; }

        public int GenreId { get; set; }
        public Genre? Genre { get; set; }
        [NotMapped]
        public string? GenreName { get; set; }

        public Stock? Stock { get; set; }
        [NotMapped]
        public int Quantity { get; set; }

        public ICollection<Review>? Reviews { get; set; }

        [NotMapped]
        public decimal AverageRating
        {
            get
            {
                if (Reviews != null && Reviews.Count > 0)
                    return (decimal)Reviews.Average(r => r.Rating);
                return 0;
            }
        }
    }
}
