namespace BookShopping.Models.DTOs
{
    public class BookReviewViewModel
    {
        public Book Book { get; set; }
        public List<Review> Reviews { get; set; } = new List<Review>();
        //public Genre Genre { get; set; }
        public double AverageRating { get; set; }
    }
}
