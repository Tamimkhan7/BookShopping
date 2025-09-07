using BookShopping.Models;

namespace BookShopping.Services
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Book>> GetBooks(string strem, int genreId);
        Task<IEnumerable<Genre>> Genres();
        Task<Book?> GetBookByIdAsync(int id);
        Task<List<Book>> GetRelatedBooksAsync(int bookId, int genreId, int count = 5);
    }
}
