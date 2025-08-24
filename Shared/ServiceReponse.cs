namespace Shared
{
    public class ServiceResponse<T>
    {
        public T Response { get; set; }

        public string Message { get; set; }

        public ServiceResponseStatus Status { get; set; }
    }

    public enum ServiceResponseStatus
    {
        Success,
        Failure,
        Warning
    }
}
