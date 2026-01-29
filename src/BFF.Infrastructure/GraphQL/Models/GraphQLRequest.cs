namespace BFF.Infrastructure.GraphQL.Models;

public class GraphQLRequest
{
    public string Query { get; set; } = string.Empty;
    public object? Variables { get; set; }
    public string? OperationName { get; set; }
}
