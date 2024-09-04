using FBAdsManager.Common.Paging;

namespace FBAdsManager.Common.Response.ResponseService
{
    public class ResponseService
    {
        public string? ErrorMessage { get; set; }
        public Object? Data { get; set; }
        public int StatusCode { get; set; } = 200;
        public PagingResponse? pagingResponse { get; set; }

        public ResponseService(string errorMessage, Object? data)
        {
            ErrorMessage = errorMessage;
            Data = data;
        }

        public ResponseService(string errorMessage, Object? data, int statusCode)
        {
            ErrorMessage = errorMessage;
            Data = data;
            StatusCode = statusCode;
        }

        public ResponseService(string errorMessage, Object? data,PagingResponse? paging)
        {
            ErrorMessage = errorMessage;
            Data = data;
            pagingResponse = paging;
        }
    }
}
