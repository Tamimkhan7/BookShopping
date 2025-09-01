using BookShopping.Models;
using BookShopping.Models.DTOs;
using BookShopping.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookShopping.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;
        private readonly IReviewService _reviewService;

        public HomeController(
            ILogger<HomeController> logger,
            IHomeRepository homeRepository,
            IReviewService reviewService)
        {
            _logger = logger;
            _homeRepository = homeRepository;
            _reviewService = reviewService;
        }

        public async Task<IActionResult> Index(string strem = "", int genreId = 0)
        {
            var books = await _homeRepository.GetBooks(strem, genreId);
            var genres = await _homeRepository.Genres();

            // Load Reviews for each book, AverageRating calculated automatically
            foreach (var book in books)
            {
                book.Reviews = await _reviewService.GetReviewsByBookIdAsync(book.Id);
            }

            // Recent Reviews
            var recentReviews = await _reviewService.GetRecentReviewsAsync();
            ViewBag.RecentReviews = recentReviews;

            var bookModel = new BookDisplayModel
            {
                Books = books,
                Genres = genres,
                STerm = strem,
                GenreId = genreId
            };

            return View(bookModel);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
