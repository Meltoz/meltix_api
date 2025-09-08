namespace Shared
{
    public class ServiceResponse<T>
    {
        public T Response { get; set; }

        public string Message { get; set; }

        public ServiceResponseStatus Status { get; set; }

        public static ServiceResponse<T> Success (T response, string message= null)
        {
            return new ServiceResponse<T>
            {
                Response = response,
                Status = ServiceResponseStatus.Success,
                Message = message
            };
        }

        public static ServiceResponse<T> Failure(string message)
        {
            return new ServiceResponse<T>
            {
                Status = ServiceResponseStatus.Failure,
                Message = message
            };
        }

        public static ServiceResponse<T> NotFound(string message=null)
        {
            return new ServiceResponse<T>
            {
                Status = ServiceResponseStatus.NotFound,
                Message = message
            };
        }
    }

    public enum ServiceResponseStatus
    {
        Success,
        Failure,
        NotFound,
        Warning
    }
}
