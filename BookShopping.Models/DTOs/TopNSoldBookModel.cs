using BookShopping.Utility;

namespace BookShoppingCartMvcUI.Models.DTOs
{
    public record TopNSoldBookModel(string BookName, string AuthorName, int TotalUnitSold);

    public record TopNSoldBooksVm(
        DateTime StartDate,
        DateTime EndDate,
        PagedResult<TopNSoldBookModel> PagedBooks
    );
}
