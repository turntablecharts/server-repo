namespace Presentation.DTO
{
    public class ResponseDto<T> 
    {
        public T Data {get; set;}
        public int StatusCode { get; set; }

        public string ResponseMessage {get; set;}
    }
}