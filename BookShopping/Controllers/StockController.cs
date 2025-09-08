using BookShopping.Models.DTOs;
using BookShopping.Services;
using BookShopping.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShopping.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class StockController : Controller
    {
        private readonly IStockRepository _stockRepo;
        public StockController(IStockRepository stockRepo)
        {
            _stockRepo = stockRepo;
        }

        // GET: /Stock
        public async Task<IActionResult> Index(string sTerm = "", int page = 1, int pageSize = 10)
        {
            // Get StockDisplayModel list
            var stocks = await _stockRepo.GetStocks(sTerm);
            int totalItems = stocks.Count();

            var pagedStocks = stocks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedResult = new PagedResult<StockDisplayModel>
            {
                Data = pagedStocks,
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize
            };

            ViewData["SearchTerm"] = sTerm;

            return View(pagedResult);
        }

        // GET: /Stock/ManageStock?bookId=5
        public async Task<IActionResult> ManageStock(int bookId)
        {
            var existingStock = await _stockRepo.GetStockByBookId(bookId);
            var stock = new StockDTO
            {
                BookId = bookId,
                Quantity = existingStock != null ? existingStock.Quantity : 0
            };
            return View(stock);
        }

        // POST: /Stock/ManageStock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageStock(StockDTO stock)
        {
            if (!ModelState.IsValid)
                return View(stock);

            try
            {
                await _stockRepo.ManageStock(stock);
                TempData["successMessage"] = "Stock updated successfully!";

            }
            catch (Exception)
            {
                TempData["errorMessage"] = "Something went wrong!";
            }
            //return RedirectToAction(nameof(Index));
            return View(stock);
        }
    }
}
