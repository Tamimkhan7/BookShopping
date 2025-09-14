using BookShopping.Data;
using BookShopping.Models;
using Microsoft.EntityFrameworkCore;

namespace BookShopping.Services
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;
        public HomeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Genre>> Genres()
        {
            return await _db.Genres.ToListAsync();
        }

        public async Task<IEnumerable<string>> GetAuthors()
        {
            return await _db.Books
                .Select(b => b.AuthorName)
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooks(string strem, int genreId, string author, string sortBy, decimal minPrice, decimal maxPrice)
        {
            strem = strem?.ToLower() ?? "";

            var booksQuery = from book in _db.Books
                             join genre in _db.Genres on book.GenreId equals genre.Id
                             join stock in _db.Stocks on book.Id equals stock.BookId into book_stock
                             from bookWithStock in book_stock.DefaultIfEmpty()
                             select new Book
                             {
                                 Id = book.Id,
                                 Image = book.Image,
                                 AuthorName = book.AuthorName,
                                 BookName = book.BookName,
                                 GenreId = book.GenreId,
                                 Price = book.Price,
                                 DiscountPercentage = book.DiscountPercentage,
                                 GenreName = genre.GenreName,
                                 Quantity = bookWithStock == null ? 0 : bookWithStock.Quantity,
                                 Reviews = book.Reviews
                             };

            if (!string.IsNullOrWhiteSpace(strem))
                booksQuery = booksQuery.Where(b => b.BookName.ToLower().Contains(strem));

            if (genreId > 0)
                booksQuery = booksQuery.Where(b => b.GenreId == genreId);

            if (!string.IsNullOrEmpty(author))
                booksQuery = booksQuery.Where(b => b.AuthorName == author);

            // DB theke data fetch
            var books = await booksQuery.ToListAsync();

            // Memory level filtering (DiscountedPrice support kore)
            if (minPrice > 0)
                books = books.Where(b => b.DiscountedPrice >= (double)minPrice).ToList();

            if (maxPrice > 0)
                books = books.Where(b => b.DiscountedPrice <= (double)maxPrice).ToList();

            // Sorting
            books = sortBy switch
            {
                "bestseller" => books.OrderByDescending(b => b.Quantity).ToList(),
                "rating" => books.OrderByDescending(b => b.AverageRating).ToList(),
                "new" => books.OrderByDescending(b => b.Id).ToList(),
                "pricelow" => books.OrderBy(b => b.DiscountedPrice).ToList(),
                "pricehigh" => books.OrderByDescending(b => b.DiscountedPrice).ToList(),
                _ => books.OrderBy(b => b.BookName).ToList()
            };

            return books;
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            var book = await _db.Books
                .Include(b => b.Genre)
                .Include(b => b.Reviews)
                .Include(b => b.Stock)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book != null && book.Stock != null)
                book.Quantity = book.Stock.Quantity;

            return book;
        }

        public async Task<List<Book>> GetRelatedBooksAsync(int bookId, int genreId, int count = 4)
        {
            var relatedBooks = await _db.Books
                .Where(b => b.GenreId == genreId && b.Id != bookId)
                .Include(b => b.Genre)
                .Include(b => b.Reviews)
                .Include(b => b.Stock)
                .ToListAsync();

            foreach (var book in relatedBooks)
            {
                book.GenreName = book.Genre?.GenreName;
                book.Quantity = book.Stock?.Quantity ?? 0;
            }

            return relatedBooks
                .OrderByDescending(b => b.AverageRating)
                .Take(count)
                .ToList();
        }
    }
}
