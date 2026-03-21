namespace SocialAppBackend.Common;

public class ServiceResult<T>
{
    public T? Data { get; set; }
    public ServiceError? Error { get; set; }
    public bool Success => Error is null;
}