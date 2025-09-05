using BookShopping.Models;
using BookShopping.Models.DTOs;
using BookShopping.Services;
using BookShopping.Utility;
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

        public async Task<IActionResult> Index(string strem = "", int genreId = 0, int page = 1, int pageSize = 12)
        {
            var allBooks = await _homeRepository.GetBooks(strem, genreId);

            // Load Reviews for each book
            foreach (var book in allBooks)
            {
                book.Reviews = await _reviewService.GetReviewsByBookIdAsync(book.Id);
            }

            var pagedResult = new PagedResult<Book>
            {
                Data = allBooks.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalItems = allBooks.Count(),
                PageNumber = page,
                PageSize = pageSize
            };

            var bookModel = new BookDisplayModel
            {
                PagedBooks = pagedResult,
                Genres = await _homeRepository.Genres(),
                STerm = strem,
                GenreId = genreId
            };

            // Recent Reviews
            ViewBag.RecentReviews = await _reviewService.GetRecentReviewsAsync();

            return View(bookModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var book = await _homeRepository.GetBookByIdAsync(id);
            if (book == null) return NotFound();

            book.Reviews = await _reviewService.GetReviewsByBookIdAsync(book.Id);
            return View(book);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
