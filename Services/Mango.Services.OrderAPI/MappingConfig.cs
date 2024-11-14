using AutoMapper;
using Mango.Services.OrderAPI.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.OrderAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                //config.CreateMap<ProductDto, Product>().ReverseMap();
                //config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
                //config.CreateMap<CartDetailsDto, CartDetails>().ReverseMap();
                //config.CreateMap<CartDto, Cart>().ReverseMap();
              
            });

            return mappingConfig;
        }
    }
}
