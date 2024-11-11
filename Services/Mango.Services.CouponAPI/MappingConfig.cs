using AutoMapper;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CouponDto, Coupon>().ReverseMap();
                //config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
                //config.CreateMap<CartDetailsDto, CartDetails>().ReverseMap();
                //config.CreateMap<CartDto, Cart>().ReverseMap();
              
            });

            return mappingConfig;
        }
    }
}
