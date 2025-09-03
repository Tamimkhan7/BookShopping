using BookShopping.Models;
using BookShopping.Services;
using BookShopping.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShopping.Controllers
{
    [Authorize]
    public class UserOrderController : Controller
    {
        private readonly IUserOrderRepository _userOrderRepository;
        public UserOrderController(IUserOrderRepository userOrderRepository)
        {
            _userOrderRepository = userOrderRepository;
        }

        // GET: /UserOrder/UserOrders
        public async Task<IActionResult> UserOrders(int page = 1, int pageSize = 10)
        {
            var orders = await _userOrderRepository.UserOrders();
            int totalItems = orders.Count();

            var pagedOrders = orders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedResult = new PagedResult<Order>
            {
                Data = pagedOrders,
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize
            };

            return View(pagedResult);
        }
    }
}
