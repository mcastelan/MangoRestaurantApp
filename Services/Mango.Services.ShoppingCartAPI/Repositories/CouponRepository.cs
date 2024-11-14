using AutoMapper;
using Mango.Services.ShoppingCartAPI.DbContexts;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Mango.Services.ShoppingCartAPI.Models.Dtos;

namespace Mango.Services.ShoppingCartAPI.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly HttpClient httpClient;
       
        private readonly IMapper _mapper;

        public CouponRepository( IMapper mapper, HttpClient client)
        {
            
            this._mapper = mapper;
            this.httpClient = client;
        }
        public async Task<CouponDto> GetCoupon(string couponName)
        {
           
                var response = await httpClient.GetAsync($"api/coupon/{couponName}");
              var apiContent = await response.Content.ReadAsStringAsync();
              var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                if (resp.IsSuccess)
                {
                    return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(resp.Result));
                    
                }
                return new CouponDto();
            
        }
    }
}
