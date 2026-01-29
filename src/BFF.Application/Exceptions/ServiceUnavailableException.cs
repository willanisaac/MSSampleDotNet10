namespace BFF.Application.Exceptions;

public class ServiceUnavailableException : Exception
{
    public string ServiceName { get; }

    public ServiceUnavailableException(string serviceName) 
        : base($"Service '{serviceName}' is currently unavailable.")
    {
        ServiceName = serviceName;
    }

    public ServiceUnavailableException(string serviceName, Exception innerException) 
        : base($"Service '{serviceName}' is currently unavailable.", innerException)
    {
        ServiceName = serviceName;
    }
}
