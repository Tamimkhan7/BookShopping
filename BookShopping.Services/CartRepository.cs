using BookShopping.Data;
using BookShopping.Models;
using BookShopping.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        public async Task<int> AddItem(int bookId, int qty)
        {
            string userId = GetUserId();
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User is not Logged In");

                var cart = await GetCart(userId);
                if (cart == null)
                {
                    cart = new ShoppingCart
                    {
                        UserId = userId
                    };
                    _db.ShoppingCarts.Add(cart);
                    await _db.SaveChangesAsync();
                }

                var cartItem = await _db.CartDetails
                    .FirstOrDefaultAsync(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);

                if (cartItem != null)
                {
                    cartItem.Quantity += qty;
                }
                else
                {
                    var book = _db.Books.Find(bookId);
                    cartItem = new CartDetail
                    {
                        ShoppingCartId = cart.Id,
                        BookId = bookId,
                        Quantity = qty,
                        UnitPrice = book.Price //it is a new line after update database
                    };
                    _db.CartDetails.Add(cartItem);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
            }

            return await GetCartItemCount(userId);
        }

        public async Task<int> RemoveItem(int bookId)
        {
            string userId = GetUserId();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User is not Logged In");

                var cart = await GetCart(userId);
                if (cart == null)
                    throw new Exception("Cart is empty");

                var cartItem = await _db.CartDetails
                    .FirstOrDefaultAsync(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);

                if (cartItem == null)
                    throw new Exception("Item not found in cart");

                if (cartItem.Quantity > 1)
                    cartItem.Quantity -= 1;
                else
                    _db.CartDetails.Remove(cartItem);

                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
            }
            return await GetCartItemCount(userId);
        }

        public async Task<ShoppingCart> GetUserCart()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Invalid userId");

            var shoppingCart = await _db.ShoppingCarts
                .Include(a => a.CartDetails)
                    .ThenInclude(a => a.Book)
                        .ThenInclude(b => b.Genre)
                .Where(a => a.UserId == userId)
                .FirstOrDefaultAsync();

            return shoppingCart;
        }

        public async Task<int> GetCartItemCount(string userId = "")
        {
            if (string.IsNullOrEmpty(userId))
                userId = GetUserId();

            var count = await (from cart in _db.ShoppingCarts
                               join cartDetail in _db.CartDetails
                               on cart.Id equals cartDetail.ShoppingCartId
                               where cart.UserId == userId
                               select cartDetail.Quantity).SumAsync();

            return count;
        }


        private async Task<ShoppingCart> GetCart(string userId)
        {
            return await _db.ShoppingCarts
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<bool> DoCheckOut(CheckoutModel model)
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                // logic
                //move data from cartDetail to order and order detail then we will remove cart detail

                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User is not Logged In");
                var cart = await GetCart(userId);
                if (cart is null) throw new Exception("Invalid cart");
                var cartDetails = _db.CartDetails
                    .Where(a => a.ShoppingCartId == cart.Id).ToList();
                if (cartDetails.Count == 0) throw new InvalidOperationException("Cart is empty");
                var pendingRecord = _db.OrderStatuses.FirstOrDefault(a => a.StatusName == "Pending");
                if (pendingRecord is null)
                    throw new Exception("Order status does not have pending status");

                var order = new Order
                {
                    UserId = userId,
                    CreateDate = DateTime.UtcNow,
                    Name = model.Name,
                    Email = model.Email,
                    MobileNumber = model.MobileNumber,
                    PaymentMethod = model.PaymentMethod,
                    Address = model.Address,
                    IsPaid = false,
                    OrderStatusId = pendingRecord.Id, // 1 for pending
                };

                _db.Orders.Add(order);
                _db.SaveChanges();
                foreach (var item in cartDetails)
                {
                    var orderDetail = new OrderDetail
                    {
                        BookId = item.BookId,
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice

                    };
                    _db.OrderDetails.Add(orderDetail);

                }
                _db.SaveChanges();
                //remove the cartdetails
                _db.CartDetails.RemoveRange(cartDetails);
                _db.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            return _userManager.GetUserId(principal);
        }
    }
}
