using BookShoppingCartMvcUI.Models.DTOs;

namespace BookShoppingCartMvcUI.Services
{
    public interface IReportService
    {
        TopNSoldBooksVm GetTopSoldBooks(DateTime startDate, DateTime endDate, int page, int pageSize);
    }
}
