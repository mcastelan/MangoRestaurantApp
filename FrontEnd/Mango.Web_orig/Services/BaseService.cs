using Mango.Web.Models;
using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Newtonsoft.Json;
using System.Text;
using static Mango.Web.SD;

namespace Mango.Web.Services
{
    public class BaseService : IBaseService
    {
        public IHttpClientFactory httpClient;

        public ResponseDto responseModel { get ; set; }
        public BaseService(IHttpClientFactory httpClientFactory)
        {
            this.httpClient = httpClientFactory;
            this.responseModel = new ResponseDto();
        }


        public async Task<T> SendAsync<T>(ApiRequest apiRequest)
        {
            try
            {
                var client = httpClient.CreateClient("MangoAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url);
                client.DefaultRequestHeaders.Clear();
                message.Method = apiRequest.ApiType switch
                {
                    ApiType.POST => HttpMethod.Post,
                    ApiType.PUT => HttpMethod.Put,
                    ApiType.DELETE => HttpMethod.Delete,
                    _ => HttpMethod.Get,
                };
                if (apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), 
                        Encoding.UTF8, "application/json");
                }
                HttpResponseMessage apiResponse =null;
                apiResponse = await client.SendAsync(message);
                var apiContent = await apiResponse.Content.ReadAsStringAsync();
                var apiResponseDto = JsonConvert.DeserializeObject<T>(apiContent);

                return apiResponseDto;


            }
            catch (Exception ex)
            {

                var dto = new ResponseDto
                {
                    DisplayMessage = "Error",
                    Errors = new List<string> { Convert.ToString(ex.Message) },
                    IsSuccess = false
                };
                var res = JsonConvert.SerializeObject(dto);
                var apiResponseDto = JsonConvert.DeserializeObject<T>(res);
                return apiResponseDto;
            }
        }
        public void Dispose()
        {
            GC.SuppressFinalize(true);
        }
    }
}
