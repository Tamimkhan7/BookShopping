using BookShopping.Models;
using BookShopping.Models.DTOs;
using BookShopping.Services;
using BookShopping.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShoppingCartMvcUI.Controllers;

[Authorize(Roles = nameof(Roles.Admin))]
public class AdminOperationsController : Controller
{
    private readonly IUserOrderRepository _userOrderRepository;
    public AdminOperationsController(IUserOrderRepository userOrderRepository)
    {
        _userOrderRepository = userOrderRepository;
    }

    public async Task<IActionResult> AllOrders(int page = 1, int pageSize = 10)
    {
        var orders = await _userOrderRepository.UserOrders(true);

        var result = new PagedResult<Order>
        {
            TotalItems = orders.Count(),
            PageNumber = page,
            PageSize = pageSize,
            Data = orders.Skip((page - 1) * pageSize).Take(pageSize).ToList()
        };
        return View(result);
    }

    public async Task<IActionResult> TogglePaymentStatus(int orderId)
    {
        try
        {
            await _userOrderRepository.TogglePaymentStatus(orderId);
        }
        catch (Exception ex)
        {
            // log exception here
        }
        return RedirectToAction(nameof(AllOrders));
    }

    //orderid diye order status update korteci karon,,aita just admin korte parbe
    public async Task<IActionResult> UpdateOrderStatus(int orderId)
    {
        var order = await _userOrderRepository.GetOrderById(orderId);
        if (order == null) throw new InvalidOperationException($"Order with id:{orderId} does not found.");
        var orderstatus = await _userOrderRepository.GetOrderStatuses();
        var orderStatusList = orderstatus.Select(orderStatus =>
        {
            return new SelectListItem
            {
                Value = orderStatus.Id.ToString(),
                Text = orderStatus.StatusName,
                Selected = order.OrderStatusId == orderStatus.Id
            };
        });
        var data = new UpdateOrderStatusModel
        {
            OrderId = orderId,
            OrderStatusId = order.OrderStatusId,
            OrderStatusList = orderStatusList
        };
        return View(data);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusModel data)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var orderstatus = await _userOrderRepository.GetOrderStatuses();
                data.OrderStatusList = orderstatus.Select(orderStatus =>
                {
                    return new SelectListItem
                    {
                        Value = orderStatus.Id.ToString(),
                        Text = orderStatus.StatusName,
                        Selected = orderStatus.Id == data.OrderStatusId
                    };
                });

                return View(data);
            }
            await _userOrderRepository.ChangeOrderStatus(data);
            TempData["msg"] = "Updated successfully";
        }
        catch (Exception ex)
        {
            // catch exception here
            TempData["msg"] = "Something went wrong";
        }
        return RedirectToAction(nameof(UpdateOrderStatus), new { orderId = data.OrderId });
        //return RedirectToAction(nameof(AllOrders));
    }
    public IActionResult Dashboard()
    {
        return View();
    }

}