﻿using Mango.Web.Models;
using Mango.Web.Models;

namespace Mango.Web.Services.IServices
{
    public interface IBaseService:IDisposable
    {
        ResponseDto responseModel { get; set; }
        Task<T> SendAsync<T>(ApiRequest apiRequest);
        
    }
}
