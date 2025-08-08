using BookShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopping.Services
{
    public interface ICartRepository
    {
        Task<bool> AddItem(int bookId, int qty);
        Task<bool> RemoveItem(int bookId);
        Task<IEnumerable<ShoppingCart>> GetUserCart();
        Task<int> GetCartItemCount(string userId = "");
    }
}
