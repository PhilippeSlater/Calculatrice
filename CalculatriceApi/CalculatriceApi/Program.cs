using CalculatriceApi.Models.Requests;
using CalculatriceApi.Models.Responses;
using CalculatriceApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();
app.UseCors();

app.MapPost("/evaluate", (ExprRequest req) =>
{
    try
    {
        var service = new ExprService();
        var (result, treeJson, tokenDtos) = service.Evaluate(req.Input);
        return Results.Ok(new ExprResponse(result, treeJson, tokenDtos));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERREUR: {ex.Message}");
        Console.WriteLine($"STACK: {ex.StackTrace}");
        return Results.BadRequest(new { error = ex.Message });
    }
});
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");