﻿using AutoMapper;
using Mango.Services.CouponAPI.DbContexts;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Mango.Services.CouponAPI.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public CouponRepository(ApplicationDbContext db, IMapper mapper)
        {
            this._db = db;
            this._mapper = mapper;
        }
        public async Task<CouponDto> GetCouponByCode(string couponCode)
        {
           var couponFromDb = await _db.Coupons.FirstOrDefaultAsync(u => u.CouponCode == couponCode);
            return _mapper.Map<CouponDto>(couponFromDb);
        }
    }
}
