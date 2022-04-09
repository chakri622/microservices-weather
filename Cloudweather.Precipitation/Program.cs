using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


var app = builder.Build();

app.MapGet("/observation/{zip}", (string zip,[FromQuery] int? days) =>
{
    return Results.Ok(zip);

});

// Configure the HTTP request pipeline.


app.Run();
