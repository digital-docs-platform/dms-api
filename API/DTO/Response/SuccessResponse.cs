using System.Collections;

namespace API.DTO.Response
{
    public class SuccessResponse
    {
        public string Message { get; set; }
        public bool Success => true;
        public object? Data { get; set; }
    }
}
