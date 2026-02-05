using Users.Data;
using Users.API.GraphQL;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001);
});

builder.Services.AddSingleton(sp =>
{
    var filePath = System.IO.Path.Combine(AppContext.BaseDirectory, "Data", "users.json");
    return new UsersRepository(filePath);
});

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapGraphQL();

app.Run();
