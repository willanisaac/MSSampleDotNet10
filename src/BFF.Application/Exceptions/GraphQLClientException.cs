using BFF.Application.DTOs.GraphQL;

namespace BFF.Application.Exceptions;

public class GraphQLClientException : Exception
{
    public List<GraphQLError>? GraphQLErrors { get; }

    public GraphQLClientException(string message) : base(message)
    {
    }

    public GraphQLClientException(string message, List<GraphQLError> errors) : base(message)
    {
        GraphQLErrors = errors;
    }

    public GraphQLClientException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
