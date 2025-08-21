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
        //all of genre return from here that's the function working
        public async Task<IEnumerable<Genre>> Genres()
        {
            return await _db.Genres.ToListAsync();
        }
        //strem means search name is empty and category id is 0, which i indicated 
        public async Task<IEnumerable<Book>> GetBooks(string sTrem = "", int genreId = 0)
        {
            sTrem = sTrem.ToLower();
            IEnumerable<Book> books = await (from book in _db.Books
                                             join genre in _db.Genres
                                             on book.GenreId equals genre.Id
                                             join stock in _db.Stocks
                                             on book.Id equals stock.BookId
                                             into book_stock
                                             from bookWithStock in book_stock.DefaultIfEmpty()
                                             where string.IsNullOrWhiteSpace(sTrem)
                                             || (book != null && book.BookName.ToLower().StartsWith(sTrem))
                                             select new Book
                                             {

                                                 Id = book.Id,
                                                 Image = book.Image,
                                                 AuthorName = book.AuthorName,
                                                 BookName = book.BookName,
                                                 GenreId = book.GenreId,
                                                 Price = book.Price,
                                                 GenreName = genre.GenreName,
                                                 Quantity = bookWithStock == null ? 0 : bookWithStock.Quantity

                                             }).ToListAsync();

            if (genreId > 0)
                books = books.Where(a => a.GenreId == genreId).ToList();

            return books;
        }
    }
}
