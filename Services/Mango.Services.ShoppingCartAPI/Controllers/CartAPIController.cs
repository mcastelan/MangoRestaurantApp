using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Messages;
using Mango.Services.ShoppingCartAPI.Models.Dtos;
using Mango.Services.ShoppingCartAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartAPIController

        : ControllerBase
    {
        protected ResponseDto _response;
        private ICartRepository _cartRepository;
        private readonly IMessageBus _messageBus;

        public CartAPIController(ICartRepository cartRepository, IMessageBus messageBus)
        {
            _cartRepository = cartRepository;
            this._messageBus = messageBus;
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

        [HttpPost("ApplyCoupon")]
        public async Task<IActionResult> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                bool isSuccess = await _cartRepository.ApplyCoupon(cartDto.CartHeader.UserId, cartDto.CartHeader.CouponCode);
                _response.IsSuccess = isSuccess;

            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }
            return Ok(_response);
        }

        [HttpPost("RemoveCoupon")]
        public async Task<IActionResult> RemoveCoupon([FromBody] string userId)
        {
            try
            {
                bool isSuccess = await _cartRepository.RemoveCoupon(userId);
                _response.IsSuccess = isSuccess;

            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }
            return Ok(_response);
        }

        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout(CheckoutHeaderDto checkoutHeader)
        {
            try
            {
               CartDto carDto = await _cartRepository.GetCartByUserId(checkoutHeader.UserId);
                if (carDto == null)
                {
                    return BadRequest();
                }
                checkoutHeader.CartDetails = carDto.CartDetails;
                _response.Result = checkoutHeader;

                await _messageBus.PublishMessage(checkoutHeader, "checkoutmessagetopic");

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
