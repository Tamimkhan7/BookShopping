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

        public IActionResult TopSellers(DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 5)
        {
            var start = startDate ?? DateTime.Now.AddMonths(-1);
            var end = endDate ?? DateTime.Now;

            var vm = _reportService.GetTopSoldBooks(start, end, page, pageSize);
            return View(vm);
        }
    }
}
