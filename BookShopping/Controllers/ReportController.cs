using BookShoppingCartMvcUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookShoppingCartMvcUI.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public IActionResult Top5Sellers(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Now.AddMonths(-1); // default last 1 month
            var end = endDate ?? DateTime.Now;

            var vm = _reportService.GetTop5SoldBooks(start, end);
            return View(vm);
        }
    }
}
