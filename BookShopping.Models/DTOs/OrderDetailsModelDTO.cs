namespace BookShopping.Models.DTOs;

public class OrderDetailsModelDTO
{
    public string DivID { get; set; }
    public IEnumerable<OrderDetail> OrderDetails { get; set; }
}
