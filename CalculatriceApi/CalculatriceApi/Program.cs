using CalculatriceApi.Models.Requests;
using CalculatriceApi.Models.Responses;
using CalculatriceApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5173")
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

app.Run();