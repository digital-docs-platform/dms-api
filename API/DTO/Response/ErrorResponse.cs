using System.Collections;

namespace API.DTO.Response
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public bool Success => false;
        public object? Data { get; set; }
    }
}
