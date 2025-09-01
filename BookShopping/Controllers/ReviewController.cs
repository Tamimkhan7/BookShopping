using BookShopping.Data;
using BookShopping.Models;
using BookShopping.Models.DTOs;
using BookShopping.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShopping.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly ApplicationDbContext _context;

        public ReviewController(IReviewService reviewService, ApplicationDbContext context)
        {
            _reviewService = reviewService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> AddReviewPage(int bookId)
        {
            var book = await _context.Books
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null)
                return NotFound();

            var model = new BookReviewViewModel
            {
                Book = book,
                Reviews = await _reviewService.GetReviewsByBookIdAsync(bookId),
                AverageRating = await _reviewService.GetAverageRatingAsync(bookId)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int bookId, int rating, string comment)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var review = new Review
            {
                BookId = bookId,
                UserId = userId,
                Rating = rating,
                Comment = comment
            };

            await _reviewService.AddReviewAsync(review);

            return RedirectToAction("AddReviewPage", new { bookId = bookId });
        }
    }
}
