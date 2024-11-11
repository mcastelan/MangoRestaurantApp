using Mango.Services.ShoppingCartAPI.Models.Dtos;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCartAPI.Repositories
{
    public interface ICartRepository
    {
        Task<CartDto> GetCartByUserId(string userId);
        Task<CartDto> CreateUpdate(CartDto cart);

        Task<bool> RemoveFromCart(int cartDetailsId);
        Task<bool> ApplyCoupon(string userId, string couponCode);
        Task<bool> RemoveCoupon(string userId);

        Task<bool> ClearCart(string userId);
    }
}
