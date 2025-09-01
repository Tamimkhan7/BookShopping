using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShopping.Models
{
    //[Table("Books")]
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
        public string? Image { get; set; }
        [Required]
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public List<OrderDetail> OrderDetail { get; set; }
        public List<CartDetail> CartDetail { get; set; }
        public Stock Stock { get; set; }
        [NotMapped]
        public string? GenreName { get; set; }
        [NotMapped]
        public int Quantity { get; set; }


        public List<Review> Reviews { get; set; }

        [NotMapped]
        public double AverageRating
        {
            get
            {
                return (Reviews != null && Reviews.Count > 0) ? Reviews.Average(r => r.Rating) : 0;
            }
        }

    }
}
