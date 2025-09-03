namespace BookShopping.Utility
{
    public class PagedResult<T>
    {
        // Paged Data
        public List<T> Data { get; set; } = new();

        // Total Items in dataset
        public int TotalItems { get; set; }

        // Current Page Number
        public int PageNumber { get; set; }

        // Page Size
        public int PageSize { get; set; }

        // Total Pages
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}
