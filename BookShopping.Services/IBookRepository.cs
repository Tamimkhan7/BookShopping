using BookShopping.Models;

namespace BookShopping.Services
{
    public interface IBookRepository
    {
        Task AddBook(Book book);
        //Task use kora hoice basically async ar jonno
        Task DeleteBook(Book book);
        //return Book list, if not found return null 
        Task<Book?> GetBookById(int id);
        Task<IEnumerable<Book>> GetBooks();
        Task UpdateBook(Book book);
    }
}
