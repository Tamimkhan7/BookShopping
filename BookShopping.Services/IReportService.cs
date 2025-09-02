using BookShoppingCartMvcUI.Models.DTOs;

namespace BookShoppingCartMvcUI.Services
{
    public interface IReportService
    {
        TopNSoldBooksVm GetTop5SoldBooks(DateTime startDate, DateTime endDate);
    }
}
