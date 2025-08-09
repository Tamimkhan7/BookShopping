using BookShopping.Data;
using BookShopping.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task<IEnumerable<Order>> UserOrders()
        {
            var userId = GetUserId();
            if(string.IsNullOrEmpty(userId))            
                throw new Exception("User is not Logged In");


            
            var orders = await _db.Orders
                .Include("OrderDetail")
                .Where(a => a.UserId == userId)
                .ToListAsync();

            return orders;
        }
        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            return _userManager.GetUserId(principal);
        }
    }
}
