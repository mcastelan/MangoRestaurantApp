using Mango.Web.Models;

namespace Mango.Web.Services.IServices
{
    public interface IProductService
    {
        Task<T> GetAllProductsAsync<T>();
        Task<T> GetProductByIdAsync<T>(int id);
        Task<T> CreateProductAsync<T>(ProductDto productDTO);
        Task<T> UpdateProductAsync<T>(ProductDto productDTO);
        Task<T> DeleteProductAsync<T>(int id);

    }
}
