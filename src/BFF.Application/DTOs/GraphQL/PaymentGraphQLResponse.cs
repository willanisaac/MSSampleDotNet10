namespace BFF.Application.DTOs.GraphQL;

public class PaymentGraphQLResponse
{
    public PaymentData? Data { get; set; }
    public List<GraphQLError>? Errors { get; set; }
}

public class PaymentData
{
    public PaymentGraphQL? Payment { get; set; }
    public List<PaymentGraphQL>? Payments { get; set; }
}

public class PaymentGraphQL
{
    public string Id { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
