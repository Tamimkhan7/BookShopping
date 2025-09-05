using BookShopping.Utility;

namespace BookShopping.Models.DTOs
{
    public class BookDisplayModel
    {
        public PagedResult<Book> PagedBooks { get; set; } = new PagedResult<Book>();
        public IEnumerable<Book> Books { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
        public string STerm { get; set; } = "";
        public int GenreId { get; set; } = 0;
    }
}
