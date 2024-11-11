using Mango.Services.CouponAPI.Models.Dto;
using System.Threading.Tasks;

namespace Mango.Services.CouponAPI.Repositories
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCouponByCode(string couponCode);

    }
}
