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

        // Return all genres
        public async Task<IEnumerable<Genre>> Genres()
        {
            return await _db.Genres.ToListAsync();
        }

        // Get books with optional search and genre filter
        public async Task<IEnumerable<Book>> GetBooks(string sTrem = "", int genreId = 0)
        {
            sTrem = sTrem.ToLower();

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

            if (!string.IsNullOrWhiteSpace(sTrem))
                booksQuery = booksQuery.Where(b => b.BookName.ToLower().StartsWith(sTrem));

            if (genreId > 0)
                booksQuery = booksQuery.Where(b => b.GenreId == genreId);

            return await booksQuery.ToListAsync();
        }

        // Get book by Id with genre, stock, reviews
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

        // Related books by genre excluding current book, sorted by AverageRating
        public Task<List<Book>> GetRelatedBooksAsync(int bookId, int genreId, int count = 5)
        {
            // Client-side sorting for AverageRating
            var relatedBooks = _db.Books
               .Where(b => b.GenreId == genreId && b.Id != bookId)
               .AsEnumerable() //all data niye jawa hocce
               .OrderByDescending(b => b.AverageRating) //sort korteci average rating onujayi
               .Take(count)
               .ToList();

            return Task.FromResult(relatedBooks); // return as Task<List<Book>>
        }
    }
}

