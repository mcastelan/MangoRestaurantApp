using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Mango.Web.Models
{
    public class ResponseDto
    {
        public bool IsSuccess { get; set; } = true;

        public object Result { get; set; }

        public string DisplayMessage { get; set; } = "";
        public List<String> Errors { get; set; }

        

    }
}
