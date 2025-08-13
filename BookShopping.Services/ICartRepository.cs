using BookShopping.Models;
using BookShopping.Models.DTOs;

namespace BookShopping.Services
{
    public interface ICartRepository
    {
        Task<int> AddItem(int bookId, int qty);
        Task<int> RemoveItem(int bookId);
        Task<ShoppingCart> GetUserCart();
        Task<int> GetCartItemCount(string userId = "");
        //Task<ShoppingCart> GetCart(string userId);
        Task<bool> DoCheckOut(CheckoutModel model);
    }
}
