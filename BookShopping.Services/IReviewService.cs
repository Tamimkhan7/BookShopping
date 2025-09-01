using BookShopping.Models;

namespace BookShopping.Services
{
    public interface IReviewService
    {
        Task AddReviewAsync(Review review);
        Task<List<Review>> GetReviewsByBookIdAsync(int bookId);
        Task<double> GetAverageRatingAsync(int bookId);

        // Add this method
        Task<List<Review>> GetRecentReviewsAsync(int count = 5);
    }
}
