using Mango.Services.ShoppingCartAPI.Models.Dto;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCartAPI.Repositories
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCoupon(string couponName);
    }
}
