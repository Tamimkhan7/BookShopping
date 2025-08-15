// File: Models/DTOs/OrderDetailsModelDTO.cs

// NOTE: dhore nichhi Order & OrderDetail entity gulo BookShopping.Models namespace-e ase.
// jodi onno namespace hoy, tahole niche using line ta adjust korben.
namespace BookShopping.Models.DTOs
{
    public class OrderDetailsModelDTO
    {
        public string DivID { get; set; } = "";

        // Order-er OrderDetail list ekhane pabo
        public IEnumerable<OrderDetail> OrderDetail { get; set; } = Enumerable.Empty<OrderDetail>();
    }
}
