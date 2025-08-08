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
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartRepository(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<bool> AddItem(int bookId, int qty)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                string userId = GetUserId();
                if (string.IsNullOrEmpty(userId)) return false;
                var cart = await GetCart(userId);
                if (cart is null)
                {
                    cart = new ShoppingCart
                    {
                        UserId = userId
                    };
                    _db.ShoppingCarts.Add(cart);
                }
                _db.SaveChanges();
                //cart details section
                var cartItem = _db.CartDetails.FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);
                if (cartItem is not null)
                {
                    cartItem.Quantity += qty;
                }
                else
                {
                    cartItem = new CartDetail
                    {
                        ShoppingCartId = cart.Id,
                        BookId = bookId,
                        Quantity = qty
                    };
                    _db.CartDetails.Add(cartItem);
                }
                _db.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> RemoveItem(int bookId)
            {
            //using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                string userId = GetUserId();
                if (string.IsNullOrEmpty(userId)) return false;
                var cart = await GetCart(userId);
                if (cart is null)
                {
                    return false;
                }
                //cart details section
                var cartItem = _db.CartDetails.FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);
                if (cartItem is null)
                    return false;
                else if(cartItem.Quantity > 1)
                {
                    cartItem.Quantity -= 1;
                }
                else if(cartItem.Quantity == 1)
                {
                    _db.CartDetails.Remove(cartItem);
                }
                
                _db.SaveChanges();
                //transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<IEnumerable<ShoppingCart>> GetUserCart()
        {
            var userId = GetUserId();
            if (userId == null)
                throw new Exception("Invalid userId");
            var shoppingCart = await _db.ShoppingCarts
                .Include(a => a.CartDetails)
                .ThenInclude(a => a.Book)
                .ThenInclude(a => a.Genre)
                .Where(a => a.UserId == userId).ToListAsync();
            return shoppingCart;
        }
        private async Task<ShoppingCart> GetCart(string userId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;
        }

        private async Task<int> GetCartItemCount(string userId = "")
        {
            if(!string.IsNullOrEmpty(userId))userId = GetUserId();
            var data = await (from cart in _db.ShoppingCarts
                              join CartDetail in _db.CartDetails
                              on cart.Id equals CartDetail.ShoppingCartId
                              select new { cartDetail.Id }).ToListAsync();
            return data.Count;
        }
        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            var userId = _userManager.GetUserId(principal);
            return userId;
        }
    }
}
