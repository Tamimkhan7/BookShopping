using BookShopping.Data;
using BookShopping.Models;
using BookShopping.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookShopping.Services
{
    public class UserOrderRepository : IUserOrderRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserOrderRepository(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task ChangeOrderStatus(UpdateOrderStatusModel model)
        {
            var order = await _db.Orders.FindAsync(model.OrderId);
            if (order == null)
                throw new InvalidOperationException($"Order with in id : {model.OrderId} does not found");
            order.OrderStatusId = model.OrderStatusId;
            await _db.SaveChangesAsync();

        }

        public async Task<Order>? GetOrderById(int id)
        {
            return await _db.Orders.FindAsync(id); ///find specific order by id from the database
        }

        public async Task<IEnumerable<OrderStatus>> GetOrderStatuses()
        {
            return await _db.OrderStatuses.ToListAsync();
        }

        public async Task TogglePaymentStatus(int orderId)
        {
            var order = await _db.Orders.FindAsync(orderId);
            if (order == null) throw new InvalidOperationException($"order with in id: {orderId} does not found");

            order.IsPaid = !order.IsPaid;
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> UserOrders(bool getAll = false)
        {
            var orders = _db.Orders
                   .Include(x => x.OrderStatus)
                   .Include(x => x.OrderDetail)
                   .ThenInclude(x => x.Book)
                   .ThenInclude(x => x.Genre).AsQueryable();

            if (!getAll)
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User is not Logged In");
                orders = orders.Where(x => x.UserId == userId);
                return await orders.ToListAsync();
            }
            return await orders.ToListAsync();
        }

        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            return _userManager.GetUserId(principal);
        }
    }
}
