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

            if (minPrice > 0)
                booksQuery = booksQuery.Where(b => b.Price >= (double)minPrice);

            if (maxPrice > 0)
                booksQuery = booksQuery.Where(b => b.Price <= (double)maxPrice);

            booksQuery = sortBy switch
            {
                "bestseller" => booksQuery.OrderByDescending(b => b.Quantity),
                "rating" => booksQuery.OrderByDescending(b => b.Reviews.Any() ? b.Reviews.Average(r => (decimal)r.Rating) : 0),
                "new" => booksQuery.OrderByDescending(b => b.Id), // assuming Id ~ newest
                "pricelow" => booksQuery.OrderBy(b => b.Price),
                "pricehigh" => booksQuery.OrderByDescending(b => b.Price),
                _ => booksQuery.OrderBy(b => b.BookName)
            };

            return await booksQuery.ToListAsync();
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
