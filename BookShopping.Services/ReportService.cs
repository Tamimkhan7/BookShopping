using BookShopping.Data;
using BookShopping.Utility;
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

        public TopNSoldBooksVm GetTopSoldBooks(DateTime startDate, DateTime endDate, int page, int pageSize)
        {
            var query = _context.OrderDetails
                .Include(od => od.Book)
                .Include(od => od.Order)
                .Where(od => od.Order.CreateDate >= startDate && od.Order.CreateDate <= endDate)
                .AsEnumerable()
                .GroupBy(od => new { od.Book.BookName, od.Book.AuthorName })
                .Select(g => new TopNSoldBookModel(
                    g.Key.BookName,
                    g.Key.AuthorName,
                    g.Sum(e => e.Quantity)
                ))
                .OrderByDescending(x => x.TotalUnitSold);

            var totalItems = query.Count();

            var books = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedResult = new PagedResult<TopNSoldBookModel>
            {
                Data = books,
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize
            };

            return new TopNSoldBooksVm(startDate, endDate, pagedResult);
        }
    }
}
