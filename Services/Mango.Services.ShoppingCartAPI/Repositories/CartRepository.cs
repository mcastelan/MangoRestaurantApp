using AutoMapper;
using Mango.Services.ShoppingCartAPI.DbContexts;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public CartRepository(ApplicationDbContext db, IMapper mapper) {
            this._db = db;
            this._mapper = mapper;
        }
        public async Task<bool> ClearCart(string userId)
        {
            try
            {
                var cartHeaderFromDb = await _db.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId);
                if (cartHeaderFromDb == null)
                {
                    _db.CartDetails.RemoveRange(_db.CartDetails.Where(u => u.CartHeaderId == cartHeaderFromDb.CartHeaderId));
                    _db.CartHeaders.Remove(cartHeaderFromDb);
                    await _db.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {

                return false;
            }
            
        }

        public async Task<CartDto> CreateUpdate(CartDto cartDto)
        {
            try
            {
                Cart cart = _mapper.Map<Cart>(cartDto);
                //check if product exists in the database
                var prodInDb = await _db.Products
                    .FirstOrDefaultAsync(p => p.ProductId == cartDto.CartDetails.FirstOrDefault().ProductId);
                if (prodInDb == null)
                {
                    _db.Products.Add(cart.CartDetails.FirstOrDefault().Product);
                    await _db.SaveChangesAsync();
                }
                //check if header is null
                var cartHeaderFromDb = await _db.CartHeaders.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserId == cart.CartHeader.UserId);

                if (cartHeaderFromDb == null)
                {

                    //Create header and Details
                    _db.CartHeaders.Add(cart.CartHeader);
                    await _db.SaveChangesAsync();

                    cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.CartHeaderId;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    _db.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                    await _db.SaveChangesAsync();
                }
                else
                {
                    //if header is not null
                    //check if details has the same product
                    var cartDetailsFromDb = await _db.CartDetails.AsNoTracking()
                        .FirstOrDefaultAsync(u => u.ProductId == cart.CartDetails.FirstOrDefault().ProductId
                        && u.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if (cartDetailsFromDb == null)
                    {
                        cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        cart.CartDetails.FirstOrDefault().Product = null;
                        _db.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        //if it does then update the count
                        cart.CartDetails.FirstOrDefault().Product = null;
                        cart.CartDetails.FirstOrDefault().Count += cartDetailsFromDb.Count;
                        _db.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                        await _db.SaveChangesAsync();
                    }
                }


                return _mapper.Map<CartDto>(cart);
            }
            catch (Exception ex)
            {

                return null;
            }
           
        }

        public async Task<CartDto> GetCartByUserId(string userId)
        {
            try
            {
                var cart = new Cart
                {
                    CartHeader = await _db.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId)
                };
                cart.CartDetails = _db.CartDetails.Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId).Include(u => u.Product).ToList();

                return _mapper.Map<CartDto>(cart);
            }
            catch (Exception ex)
            {

                return null;
            }
           

        }

        public async Task<bool> RemoveFromCart(int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = await _db.CartDetails.FirstOrDefaultAsync(u => u.CartDetailsId == cartDetailsId);
                int totalCountOfCartItems = _db.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                _db.CartDetails.Remove(cartDetails);
                if (totalCountOfCartItems == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
                    _db.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
          
        }
    }
}
