namespace BFF.Application.DTOs.GraphQL;

public class OrderGraphQLResponse
{
    public OrderData? Data { get; set; }
    public List<GraphQLError>? Errors { get; set; }
}

public class OrderData
{
    public OrderGraphQL? Order { get; set; }
    public List<OrderGraphQL>? Orders { get; set; }
}

public class OrderGraphQL
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public List<OrderItemGraphQL> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class OrderItemGraphQL
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Subtotal { get; set; }
}
