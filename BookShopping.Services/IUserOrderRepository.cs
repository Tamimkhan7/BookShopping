using BookShopping.Models;

namespace BookShopping.Services
{
    public interface IUserOrderRepository
    {
        Task<IEnumerable<Order>> UserOrders();
    }
}