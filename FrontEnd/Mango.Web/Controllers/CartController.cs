using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;

        public CartController(IProductService productService, ICartService cartService, ICouponService couponService)
        {
            this._productService = productService;
            this._cartService = cartService;
            this._couponService = couponService;
        }

        public async Task<IActionResult> CartIndex()
        {

            return View(await LoadCartDtoBasedOnLoggedUser());
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {

            var userId = User.Claims.Where(User => User.Type == "sub").FirstOrDefault()?.Value;
            var token = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.RemoveFromCartAsync<ResponseDto>(cartDetailsId, token);

           
            if (response != null && response.IsSuccess)
            {
               return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

     
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDtoBasedOnLoggedUser());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            try
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                var response = await _cartService.Checkout<ResponseDto>(cartDto.CartHeader, token);

               
                    return RedirectToAction(nameof(Confirmation));
                

            }
            catch (System.Exception ex)
            {

                return View(cartDto);
            }
        }
       
        public async Task<IActionResult> Confirmation()
        {
            return View();
        }

        private async Task<CartDto> LoadCartDtoBasedOnLoggedUser()
        {
            var userId = User.Claims.Where(User => User.Type == "sub").FirstOrDefault()?.Value;
            var token = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.GetCartByUserIdAsync<ResponseDto>(userId, token);

            CartDto cartDto = new CartDto();
            if (response != null && response.IsSuccess)
            {
                cartDto = JsonConvert.DeserializeObject<CartDto>(response.Result.ToString());
            }
            if (cartDto.CartHeader != null) 
            { 
                if(!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
                {
                    var coupon = await _couponService.GetCoupon<ResponseDto>(cartDto.CartHeader.CouponCode, token);
                    if (coupon != null && coupon.IsSuccess)
                    {
                        var couponObj = JsonConvert.DeserializeObject<CouponDto>(coupon.Result.ToString());
                        cartDto.CartHeader.DiscountTotal =  couponObj.DiscountAmount;
                    }
                }
                foreach (var detail in cartDto.CartDetails)
                {
                    cartDto.CartHeader.OrderTotal += detail.Product.Price * detail.Count;
                }

                cartDto.CartHeader.OrderTotal -= cartDto.CartHeader.DiscountTotal;
            }

            return cartDto;
        }

        [HttpPost]
        [ActionName("ApplyCoupon")]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {

            var userId = User.Claims.Where(User => User.Type == "sub").FirstOrDefault()?.Value;
            var token = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.ApplyCoupon<ResponseDto>(cartDto, token);


            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        [ActionName("RemoveCoupon")]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            var userId = User.Claims.Where(User => User.Type == "sub").FirstOrDefault()?.Value;
            var token = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.RemoveCoupon<ResponseDto>(cartDto.CartHeader.UserId, token);

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
    }
}
