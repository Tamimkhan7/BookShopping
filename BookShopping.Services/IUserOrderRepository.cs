using BookShopping.Models;
using BookShopping.Models.DTOs;

namespace BookShopping.Services
{
    public interface IUserOrderRepository
    {
        //show orders for admin 
        Task<IEnumerable<Order>> UserOrders(bool getAll = false);
        Task ChangeOrderStatus(UpdateOrderStatusModel model);
        Task TogglePaymentStatus(int orderId);
        Task<Order>? GetOrderById(int id);
        Task<IEnumerable<OrderStatus>> GetOrderStatuses();
    }
}