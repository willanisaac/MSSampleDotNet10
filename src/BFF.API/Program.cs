using BFF.API.Filters;
using BFF.API.Middleware;
using BFF.Application.Interfaces;
using BFF.Application.Mappings;
using BFF.Application.Services;
using BFF.Application.Services.Interfaces;
using BFF.Application.Validators;
using BFF.Infrastructure.Configuration;
using BFF.Infrastructure.GraphQL.Clients;
using BFF.Infrastructure.Resilience;
using FluentValidation;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// Serilog Configuration
// ========================================
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/bff-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ========================================
// Configuration
// ========================================
var graphQLSettings = builder.Configuration
    .GetSection("GraphQLServices")
    .Get<GraphQLServiceSettings>() ?? new GraphQLServiceSettings();

builder.Services.Configure<GraphQLServiceSettings>(
    builder.Configuration.GetSection("GraphQLServices"));

// ========================================
// Controllers & API
// ========================================
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "BFF API", 
        Version = "v1",
        Description = "Backend for Frontend API with Clean Architecture"
    });
});

// ========================================
// CORS
// ========================================
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ========================================
// AutoMapper
// ========================================
builder.Services.AddAutoMapper(typeof(UserMappingProfile).Assembly);

// ========================================
// FluentValidation
// ========================================
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();

// ========================================
// Application Services
// ========================================
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// ========================================
// GraphQL HTTP Clients
// ========================================
builder.Services.AddHttpClient<IUsersGraphQLClient, UsersGraphQLClient>(client =>
{
    client.BaseAddress = new Uri(graphQLSettings.UsersServiceUrl);
    client.Timeout = TimeSpan.FromSeconds(graphQLSettings.TimeoutSeconds);
})
.AddPolicyHandler(RetryPolicies.GetRetryPolicy())
.AddPolicyHandler(CircuitBreakerPolicies.GetCircuitBreakerPolicy())
.AddPolicyHandler(TimeoutPolicies.GetTimeoutPolicy());

builder.Services.AddHttpClient<IOrdersGraphQLClient, OrdersGraphQLClient>(client =>
{
    client.BaseAddress = new Uri(graphQLSettings.OrdersServiceUrl);
    client.Timeout = TimeSpan.FromSeconds(graphQLSettings.TimeoutSeconds);
})
.AddPolicyHandler(RetryPolicies.GetRetryPolicy())
.AddPolicyHandler(CircuitBreakerPolicies.GetCircuitBreakerPolicy())
.AddPolicyHandler(TimeoutPolicies.GetTimeoutPolicy());

builder.Services.AddHttpClient<IPaymentsGraphQLClient, PaymentsGraphQLClient>(client =>
{
    client.BaseAddress = new Uri(graphQLSettings.PaymentsServiceUrl);
    client.Timeout = TimeSpan.FromSeconds(graphQLSettings.TimeoutSeconds);
})
.AddPolicyHandler(RetryPolicies.GetRetryPolicy())
.AddPolicyHandler(CircuitBreakerPolicies.GetCircuitBreakerPolicy())
.AddPolicyHandler(TimeoutPolicies.GetTimeoutPolicy());

// ========================================
// Caching
// ========================================
builder.Services.AddMemoryCache();

// ========================================
// Health Checks
// ========================================
builder.Services.AddHealthChecks();

var app = builder.Build();

// ========================================
// Middleware Pipeline
// ========================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

// ========================================
// Startup
// ========================================
try
{
    Log.Information("Starting BFF API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
