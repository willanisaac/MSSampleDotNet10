using System.Text.Json;
using System.Text.Json.Serialization;
using Orders.Data;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5002);
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddSingleton(sp =>
{
    var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "orders.json");
    return new OrdersRepository(filePath);
});

var app = builder.Build();

var jsonOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/graphql", async (GraphQLRequest request, OrdersRepository repository) =>
{
    var queryText = request.Query?.ToLowerInvariant() ?? string.Empty;
    var operation = request.OperationName?.ToLowerInvariant();

    if (Matches(operation, queryText, "getorder") && queryText.Contains("order("))
    {
        var id = ReadVariable<string>(request, "id", jsonOptions);
        if (string.IsNullOrWhiteSpace(id))
        {
            return Error("Missing order id", "order", jsonOptions);
        }

        var order = await repository.GetByIdAsync(id);
        return order is null
            ? Error("Order not found", "order", jsonOptions)
            : Results.Json(new { data = new { order } }, jsonOptions);
    }

    if (Matches(operation, queryText, "getordersbyuser") || queryText.Contains("orders(userId", StringComparison.OrdinalIgnoreCase))
    {
        var userId = ReadVariable<string>(request, "userId", jsonOptions);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Error("Missing userId", "orders", jsonOptions);
        }

        var orders = await repository.GetByUserAsync(userId);
        return Results.Json(new { data = new { orders } }, jsonOptions);
    }

    if (Matches(operation, queryText, "createorder"))
    {
        var input = ReadVariable<CreateOrderInput>(request, "input", jsonOptions);
        if (input is null || string.IsNullOrWhiteSpace(input.UserId) || input.Items.Count == 0)
        {
            return Error("Invalid createOrder input", "createOrder", jsonOptions);
        }

        var created = await repository.AddAsync(input);
        return Results.Json(new { data = new { createOrder = created } }, jsonOptions);
    }

    if (Matches(operation, queryText, "cancelorder"))
    {
        var id = ReadVariable<string>(request, "id", jsonOptions);
        if (string.IsNullOrWhiteSpace(id))
        {
            return Error("Invalid cancelOrder input", "cancelOrder", jsonOptions);
        }

        var canceled = await repository.CancelAsync(id);
        return canceled is null
            ? Error("Order not found", "cancelOrder", jsonOptions)
            : Results.Json(new { data = new { cancelOrder = canceled } }, jsonOptions);
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
