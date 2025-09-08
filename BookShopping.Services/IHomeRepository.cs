using BookShopping.Models;

namespace BookShopping.Services
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Book>> GetBooks(string strem, int genreId, string author, string sortBy, decimal minPrice, decimal maxPrice);
        Task<IEnumerable<Genre>> Genres();
        Task<IEnumerable<string>> GetAuthors();
        Task<Book?> GetBookByIdAsync(int id);
        Task<List<Book>> GetRelatedBooksAsync(int bookId, int genreId, int count = 4);
    }
}
