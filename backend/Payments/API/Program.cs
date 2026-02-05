using Payments.Data;
using Payments.API.GraphQL;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5003);
});

builder.Services.AddSingleton(sp =>
{
    var filePath = System.IO.Path.Combine(AppContext.BaseDirectory, "Data", "payments.json");
    return new PaymentsRepository(filePath);
});

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapGraphQL();

app.Run();
