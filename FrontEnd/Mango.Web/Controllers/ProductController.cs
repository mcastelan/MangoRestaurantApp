using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

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
            var accesstoken = await HttpContext.GetTokenAsync("access_token");
            var response= await _productService.GetAllProductsAsync<ResponseDto>(accesstoken);
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
                var accesstoken = await HttpContext.GetTokenAsync("access_token");
                List<ProductDto> products = new();
                var response = await _productService.UpdateProductAsync<ResponseDto>(model,accesstoken);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }

            return View(model);
        }

        public async Task<IActionResult> ProductEdit(int id)
        {
            var accesstoken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.GetProductByIdAsync<ResponseDto>(id, accesstoken);
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
                var accesstoken = await HttpContext.GetTokenAsync("access_token");
                List<ProductDto> products = new();
                var response = await _productService.CreateProductAsync<ResponseDto>(model, accesstoken);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }           
            
            return View(model);
        }


        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> ProductDelete(int id)
        //{
        //    var accesstoken = await HttpContext.GetTokenAsync("access_token");
        //    var response = await _productService.GetProductByIdAsync<ResponseDto>(id, accesstoken);
        //    if (response != null && response.IsSuccess)
        //    {
        //        var product = JsonConvert.DeserializeObject<ProductDto>(response.Result.ToString());
        //        return View(product);
        //    }
        //    return NotFound();
        //}
        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //[ValidateAntiForgeryToken]

        //public async Task<IActionResult> ProductDelete(ProductDto model)
        //{
        //    //if (ModelState.IsValid)
        //    //{
        //        List<ProductDto> products = new();
        //    var accesstoken = await HttpContext.GetTokenAsync("access_token");
        //    var response = await _productService.DeleteProductAsync<ResponseDto>(model.ProductId,accesstoken);
        //        if ( response.IsSuccess)
        //        {
        //            return RedirectToAction(nameof(ProductIndex));
        //        }
        //    //}

        //    return View(model);
        //}

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ProductDelete(int id)
        {
            var accesstoken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.GetProductByIdAsync<ResponseDto>(id, accesstoken);

            if (response != null && response.IsSuccess)
            {
                ProductDto model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductDelete(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var response = await _productService.DeleteProductAsync<ResponseDto>(model.ProductId, accessToken);
                if (response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }
            return View(model);
        }
    }
}
