namespace BFF.Infrastructure.Configuration;

public class GraphQLServiceSettings
{
    public string UsersServiceUrl { get; set; } = string.Empty;
    public string OrdersServiceUrl { get; set; } = string.Empty;
    public string PaymentsServiceUrl { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
    public int RetryCount { get; set; } = 3;
}
