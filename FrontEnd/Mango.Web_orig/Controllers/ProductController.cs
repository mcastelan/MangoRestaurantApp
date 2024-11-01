using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            this._productService = productService;
        }
        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto> products = new();
            var response= await _productService.GetAllProductsAsync<ResponseDto>();
            if (response != null && response.IsSuccess)
            {
                products = JsonConvert.DeserializeObject<List<ProductDto>>(response.Result.ToString());
            }
            return View(products);
        }
        public async Task<IActionResult> ProductCreate()
        {
           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductEdit(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                List<ProductDto> products = new();
                var response = await _productService.UpdateProductAsync<ResponseDto>(model);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }

            return View(model);
        }

        public async Task<IActionResult> ProductEdit(int id)
        {
            var response = await _productService.GetProductByIdAsync<ResponseDto>(id);
            if (response != null && response.IsSuccess)
            {
                var product = JsonConvert.DeserializeObject<ProductDto>(response.Result.ToString());
                return View(product);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductCreate(ProductDto model)
        {
            if (ModelState.IsValid) {
                List<ProductDto> products = new();
                var response = await _productService.CreateProductAsync<ResponseDto>(model);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }           
            
            return View(model);
        }

      

        public async Task<IActionResult> ProductDelete(int id)
        {
            var response = await _productService.GetProductByIdAsync<ResponseDto>(id);
            if (response != null && response.IsSuccess)
            {
                var product = JsonConvert.DeserializeObject<ProductDto>(response.Result.ToString());
                return View(product);
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductDelete(ProductDto model)
        {
            //if (ModelState.IsValid)
            //{
                List<ProductDto> products = new();
                var response = await _productService.DeleteProductAsync<ResponseDto>(model.Id);
                if ( response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            //}

            return View(model);
        }
    }
}
