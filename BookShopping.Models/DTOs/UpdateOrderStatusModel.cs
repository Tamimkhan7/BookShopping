using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
namespace BookShopping.Models.DTOs
{
    public class UpdateOrderStatusModel
    {
        public int OrderId { get; set; }

        [Required]
        public int OrderStatusId { get; set; }

        // Use ASP.NET Core SelectListItem
        public IEnumerable<SelectListItem>? OrderStatusList { get; set; }
    }
}
