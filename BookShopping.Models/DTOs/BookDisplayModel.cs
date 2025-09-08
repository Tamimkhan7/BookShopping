using BookShopping.Utility;

namespace BookShopping.Models.DTOs
{
    public class BookDisplayModel
    {
        public PagedResult<Book> PagedBooks { get; set; }
        public IEnumerable<Book> Books { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
        public IEnumerable<string> Authors { get; set; }
        public string STerm { get; set; } = "";
        public int GenreId { get; set; } = 0;
        public string SelectedAuthor { get; set; } = "";
        public string SortBy { get; set; } = "";
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 0;
    }
}
