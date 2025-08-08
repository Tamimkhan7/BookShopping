using BookShopping.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookShopping.Services
{
    public interface ICartRepository
    {
        Task<int> AddItem(int bookId, int qty);
        Task<int> RemoveItem(int bookId);
        Task<ShoppingCart> GetUserCart();
        Task<int> GetCartItemCount(string userId = "");
    }
}
