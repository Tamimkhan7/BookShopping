using BookShopping.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BookShopping.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepository;
        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<IActionResult> AddItem(int bookId, int qty = 1, int redirect=0)
        {
            var cartCount = await _cartRepository.AddItem(bookId, qty);
            if (redirect == 0) return Ok(cartCount);
            return RedirectToAction("GetUserCart");
        }

        public async Task<IActionResult> RemoveItem(int bookId)
        {
            var cartCount = await _cartRepository.RemoveItem(bookId);
            return RedirectToAction("GetUserCart");
        }

        public async Task<IActionResult> GetUserCart()
        {
            var cart = await _cartRepository.GetUserCart();
            return View(cart);
        }

        public async Task<IActionResult> GetTotalItemInCart()
        {
            int cartItem = await _cartRepository.GetCartItemCount();
            return Ok(cartItem);
        }
        public async Task<IActionResult> Checkout()
        { 
            bool isCheckOut = await _cartRepository.DoCheckOut();
            if (!isCheckOut) throw new Exception("Something happen in server side");
            return RedirectToAction("Index", "Home");
        }
    }
}
