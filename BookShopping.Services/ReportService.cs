using BookShopping.Data;
using BookShoppingCartMvcUI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BookShoppingCartMvcUI.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public TopNSoldBooksVm GetTop5SoldBooks(DateTime startDate, DateTime endDate)
        {
            // LINQ query by Top 5 Seller find
            var topBooks = _context.OrderDetails
             .Include(od => od.Book)
             .Include(od => od.Order)
             .Where(od => od.Order.CreateDate >= startDate && od.Order.CreateDate <= endDate)
             .AsEnumerable()  // <-- now from the calculation memory added
             .GroupBy(od => new { od.Book.BookName, od.Book.AuthorName })
             .Select(g => new TopNSoldBookModel(
                 g.Key.BookName,
                 g.Key.AuthorName,
                 g.Sum(e => e.Quantity)
             ))
             .OrderByDescending(x => x.TotalUnitSold)
             .Take(5)
             .ToList();

            return new TopNSoldBooksVm(startDate, endDate, topBooks);
        }
    }
}
