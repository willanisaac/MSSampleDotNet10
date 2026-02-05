using System.Text.Json;
using System.Text.Json.Serialization;
using Users.Data;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001);
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddSingleton(sp =>
{
    var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "users.json");
    return new UsersRepository(filePath);
});

var app = builder.Build();

var jsonOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/graphql", async (GraphQLRequest request, UsersRepository repository) =>
{
    var queryText = request.Query?.ToLowerInvariant() ?? string.Empty;
    var operation = request.OperationName?.ToLowerInvariant();

    if (Matches(operation, queryText, "getuser"))
    {
        var id = ReadVariable<string>(request, "id", jsonOptions);
        if (string.IsNullOrWhiteSpace(id))
        {
            return Error("Missing user id", "user", jsonOptions);
        }

        var user = await repository.GetByIdAsync(id);
        return user is null
            ? Error("User not found", "user", jsonOptions)
            : Results.Json(new { data = new { user } }, jsonOptions);
    }

    if (Matches(operation, queryText, "getallusers") || Matches(operation, queryText, "users"))
    {
        var users = await repository.GetAllAsync();
        return Results.Json(new { data = new { users } }, jsonOptions);
    }

    if (Matches(operation, queryText, "createuser"))
    {
        var input = ReadVariable<CreateUserInput>(request, "input", jsonOptions);
        if (input is null || string.IsNullOrWhiteSpace(input.Name) || string.IsNullOrWhiteSpace(input.Email))
        {
            return Error("Invalid createUser input", "createUser", jsonOptions);
        }

        var created = await repository.AddAsync(input);
        return Results.Json(new { data = new { createUser = created } }, jsonOptions);
    }

    if (Matches(operation, queryText, "updateuser"))
    {
        var id = ReadVariable<string>(request, "id", jsonOptions);
        var input = ReadVariable<UpdateUserInput>(request, "input", jsonOptions);

        if (string.IsNullOrWhiteSpace(id) || input is null)
        {
            return Error("Invalid updateUser input", "updateUser", jsonOptions);
        }

        var updated = await repository.UpdateAsync(id, input);
        return updated is null
            ? Error("User not found", "updateUser", jsonOptions)
            : Results.Json(new { data = new { updateUser = updated } }, jsonOptions);
    }

    if (Matches(operation, queryText, "deleteuser"))
    {
        var id = ReadVariable<string>(request, "id", jsonOptions);
        if (string.IsNullOrWhiteSpace(id))
        {
            return Error("Invalid deleteUser input", "deleteUser", jsonOptions);
        }

        var deleted = await repository.DeleteAsync(id);
        return deleted
            ? Results.Json(new { data = new { deleteUser = true } }, jsonOptions)
            : Error("User not found", "deleteUser", jsonOptions);
    }

    return Error("Unsupported operation", null, jsonOptions);
});

app.Run();

static bool Matches(string? operationName, string queryText, string keyword)
{
    if (!string.IsNullOrWhiteSpace(operationName) && operationName.Equals(keyword, StringComparison.OrdinalIgnoreCase))
    {
        return true;
    }

    return queryText.Contains(keyword, StringComparison.OrdinalIgnoreCase);
}

static T? ReadVariable<T>(GraphQLRequest request, string name, JsonSerializerOptions options)
{
    if (request.Variables is null)
    {
        return default;
    }

    var variables = request.Variables.Value;
    if (variables.ValueKind == JsonValueKind.Object && variables.TryGetProperty(name, out var element))
    {
        try
        {
            return element.Deserialize<T>(options);
        }
        catch
        {
            return default;
        }
    }

    return default;
}

static IResult Error(string message, string? path, JsonSerializerOptions options)
{
    var error = new { message, path = path is null ? (string[]?)null : new[] { path } };
    return Results.Json(new { errors = new[] { error } }, options);
}

public class GraphQLRequest
{
    public string? Query { get; set; }
    public JsonElement? Variables { get; set; }
    public string? OperationName { get; set; }
}
