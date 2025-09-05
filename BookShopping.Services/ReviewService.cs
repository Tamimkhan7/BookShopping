using BookShopping.Data;
using BookShopping.Services;
using Microsoft.EntityFrameworkCore;

namespace BookShopping.Models
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddReviewAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task<double> GetAverageRatingAsync(int bookId)
        {
            var ratings = await _context.Reviews
                 .Where(r => r.BookId == bookId)
                 .Select(r => r.Rating)
                 .ToListAsync();

            return ratings.Count > 0 ? ratings.Average() : 0;
        }

        public async Task<List<Review>> GetReviewsByBookIdAsync(int bookId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.CreateAt)
                .ToListAsync();
        }

        public async Task<List<Review>> GetRecentReviewsAsync(int count = 5)
        {
            return await _context.Reviews
                .Include(r => r.Book)
                .OrderByDescending(r => r.CreateAt)
                .Take(count)
                .ToListAsync();
        }
    }
}
