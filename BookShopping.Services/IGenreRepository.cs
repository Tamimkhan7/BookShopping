using BookShopping.Models;

namespace BookShopping.Services
{
    public interface IGenreRepository
    {
        //ai function gula basically crud operation korteci, jate ami janra add delete and update korte pari, optionally getGenreById and getGenres diye genre gula dekhte pari
        Task AddGenre(Genre genre);
        Task UpdateGenre(Genre genre);
        Task<Genre?> GetGenreById(int id);
        Task DeleteGenre(Genre genre);
        Task<IEnumerable<Genre>> GetGenres();
    }
}
