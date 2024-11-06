using Mango.Services.ShoppingCartAPI.Models.Dtos;
using Mango.Services.ShoppingCartAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartAPIController

        : ControllerBase
    {
        protected ResponseDto _response;
        private ICartRepository _cartRepository;

        public CartAPIController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
            this._response = new ResponseDto();
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<IActionResult> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = await _cartRepository.GetCartByUserId(userId);
                _response.Result = cartDto;
                _response.IsSuccess = true;
              
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false ;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
              
            }
            return Ok(_response);
        }

        [HttpPost("AddCart")]
        public async Task<object> AddCart(CartDto cartDto)
        {
            try
            {
                CartDto cartDt = await _cartRepository.CreateUpdate(cartDto);
                _response.Result = cartDt;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPost("UpdateCart")]

        public async Task<IActionResult> UpdateCart(CartDto cartDto)
        {
            try
            {
                var cart = await _cartRepository.CreateUpdate(cartDto);
                _response.Result = cart;
                _response.IsSuccess = true;

            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }
            return Ok(_response);
        }

        [HttpPost("RemoveCart")]
        public async Task<IActionResult> RemoveCart([FromBody]int cartId)
        {
            try
            {
               bool isSuccess = await _cartRepository.RemoveFromCart(cartId);
               
                _response.IsSuccess = isSuccess;

            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }
            return Ok(_response);
        }
    }
}
