namespace BFF.Application.DTOs.GraphQL;

public class UserGraphQLResponse
{
    public UserData? Data { get; set; }
    public List<GraphQLError>? Errors { get; set; }
}

public class UserData
{
    public UserGraphQL? User { get; set; }
    public List<UserGraphQL>? Users { get; set; }
}

public class UserGraphQL
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GraphQLError
{
    public string Message { get; set; } = string.Empty;
    public List<ErrorLocation>? Locations { get; set; }
    public List<string>? Path { get; set; }
}

public class ErrorLocation
{
    public int Line { get; set; }
    public int Column { get; set; }
}
