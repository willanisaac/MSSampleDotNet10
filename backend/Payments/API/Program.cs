using System.Text.Json;
using System.Text.Json.Serialization;
using Payments.Data;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5003);
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddSingleton(sp =>
{
    var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "payments.json");
    return new PaymentsRepository(filePath);
});

var app = builder.Build();

var jsonOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/graphql", async (GraphQLRequest request, PaymentsRepository repository) =>
{
    var queryText = request.Query?.ToLowerInvariant() ?? string.Empty;
    var operation = request.OperationName?.ToLowerInvariant();

    if (Matches(operation, queryText, "getpayment") && queryText.Contains("payment("))
    {
        var id = ReadVariable<string>(request, "id", jsonOptions);
        if (string.IsNullOrWhiteSpace(id))
        {
            return Error("Missing payment id", "payment", jsonOptions);
        }

        var payment = await repository.GetByIdAsync(id);
        return payment is null
            ? Error("Payment not found", "payment", jsonOptions)
            : Results.Json(new { data = new { payment } }, jsonOptions);
    }

    if (Matches(operation, queryText, "getpaymentsbyorder") || queryText.Contains("payments(orderid", StringComparison.OrdinalIgnoreCase))
    {
        var orderId = ReadVariable<string>(request, "orderId", jsonOptions);
        if (string.IsNullOrWhiteSpace(orderId))
        {
            return Error("Missing orderId", "payments", jsonOptions);
        }

        var payments = await repository.GetByOrderAsync(orderId);
        return Results.Json(new { data = new { payments } }, jsonOptions);
    }

    if (Matches(operation, queryText, "processpayment"))
    {
        var input = ReadVariable<ProcessPaymentInput>(request, "input", jsonOptions);
        if (input is null || string.IsNullOrWhiteSpace(input.OrderId) || input.Amount <= 0 || string.IsNullOrWhiteSpace(input.PaymentMethod))
        {
            return Error("Invalid processPayment input", "processPayment", jsonOptions);
        }

        var created = await repository.ProcessAsync(input);
        return Results.Json(new { data = new { processPayment = created } }, jsonOptions);
    }

    if (Matches(operation, queryText, "refundpayment"))
    {
        var id = ReadVariable<string>(request, "id", jsonOptions);
        if (string.IsNullOrWhiteSpace(id))
        {
            return Error("Invalid refundPayment input", "refundPayment", jsonOptions);
        }

        var refunded = await repository.RefundAsync(id);
        return refunded is null
            ? Error("Payment not found", "refundPayment", jsonOptions)
            : Results.Json(new { data = new { refundPayment = refunded } }, jsonOptions);
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
