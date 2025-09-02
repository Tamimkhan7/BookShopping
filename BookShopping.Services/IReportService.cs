using BookShoppingCartMvcUI.Models.DTOs;

namespace BookShoppingCartMvcUI.Services
{
    public interface IReportService
    {
        // TopNSoldBooksVm : this is return type, return value can be assign any type of value as line void bool or any value we can return
        TopNSoldBooksVm GetTop5SoldBooks(DateTime startDate, DateTime endDate);
    }
}
