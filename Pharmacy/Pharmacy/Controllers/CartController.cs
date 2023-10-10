using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Models;
using Pharmacy.ViewsModels;
using System.Security.Claims;

namespace Pharmacy.Controllers
{
    public class CartController : Controller
    {
        private readonly QlpharmacyContext _context;
        private readonly CustomerModels _customer;
        private readonly CartModels _cart;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartController(QlpharmacyContext context, CustomerModels customerModels, CartModels cartModels, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _customer = customerModels;
            _cart = cartModels;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return View();
            var CustomerInfo = _customer.GetCustomer(userId);
            var Cart = _cart.getCartByCustomerId(CustomerInfo.CustomerId);


            var cartItems = _context.CartDetails
                      .Where(cd => cd.CartId == Cart.CartId)
                        .Include(cd => cd.Product)
                      .Select(cd => new CartItemViewModels
                      {
                          CartDetailId = cd.CartDetailId,
                          ProductId = cd.ProductId,
                          ProductName = cd.Product.ProductName,
                          ProductImage = cd.Product.ProductImage,
                          CartDetailPriceCurrent = cd.CartDetailPriceCurrent,
                          CartDetailQuantity = cd.CartDetailQuantity,
                          CartDetailTemporaryPrice = cd.CartDetailTemporaryPrice
                      })
                      .ToList();

            var cartTotalPrice = cartItems.Sum(item => item.CartDetailTemporaryPrice);

            ViewBag.CartTotalPrice = cartTotalPrice;
            ViewBag.CustomerId = CustomerInfo.CustomerId;
            //count
            int countCart = _cart.TotalQuantityCartDetail(Cart.CartId);
            _httpContextAccessor.HttpContext.Session.SetInt32("counter", countCart);
            ViewBag.countCart = _httpContextAccessor.HttpContext.Session.GetInt32("counter");
            return View(cartItems);

        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(double? Price, int productId, int quantity = 1)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return RedirectToAction("Index", "Login");
            var CustomerInfo = _customer.GetCustomer(userId);
            var Cart = _cart.getCartByCustomerId(CustomerInfo.CustomerId);

            var existingCartDetail = _context.CartDetails
                 .FirstOrDefault(cd => cd.CartId == Cart.CartId && cd.ProductId == productId);

            var product = _context.Products
                .Include(p => p.Discount)
                .FirstOrDefault(p => p.ProductId == productId);
        
            if (product == null)
            {
                return NotFound();
            }
            if (existingCartDetail != null)
            {
                // Nếu sản phẩm đã tồn tại trong giỏ hàng, cập nhật số lượng và tính lại giá tạm thời
                existingCartDetail.CartDetailQuantity += quantity;
                existingCartDetail.CartDetailTemporaryPrice = existingCartDetail.CartDetailQuantity * Price;
                await _cart.UpdateCartDetailAsync(existingCartDetail);
            }
            else
            {
                // Tính tổng giá tạm thời của các sản phẩm
                double? totalTemporaryPrice = Price * quantity;

                var cartDetail = new CartDetail
                {
                    CartId = Cart.CartId,
                    ProductId = product.ProductId,
                    CartDetailQuantity = quantity,
                    CartDetailPriceCurrent = Price,
                    CartDetailTemporaryPrice = totalTemporaryPrice
                };
                await _cart.CreateCartDetail(cartDetail);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateToCart(double? Price, int productId, int quantity = 1)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return View();
            var CustomerInfo = _customer.GetCustomer(userId);
            var Cart = _cart.getCartByCustomerId(CustomerInfo.CustomerId);

            var existingCartDetail = _context.CartDetails
                 .FirstOrDefault(cd => cd.CartId == Cart.CartId && cd.ProductId == productId);
            if(quantity <= 0)
            {
				await _cart.DeleteDetailAsync(existingCartDetail.CartDetailId);
				return RedirectToAction("Index");
			}
            if (existingCartDetail != null)
            {
                // Nếu sản phẩm đã tồn tại trong giỏ hàng, cập nhật số lượng và tính lại giá tạm thời
                existingCartDetail.CartDetailQuantity = quantity;
                existingCartDetail.CartDetailTemporaryPrice = existingCartDetail.CartDetailQuantity * Price;
                await _cart.UpdateCartDetailAsync(existingCartDetail);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {

            await _cart.DeleteDetailAsync(id);
            return RedirectToAction("Index");
        }
    }
}