using Cloudweather.Precipitation.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle



builder.Services.AddDbContext<PrecipDbContext>(
    opts =>
    {
        opts.EnableSensitiveDataLogging();
        opts.EnableDetailedErrors();
        opts.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"));
    }, ServiceLifetime.Transient);

var app = builder.Build();

app.MapGet("/observation/{zip}", async (string zip,[FromQuery] int? days, PrecipDbContext db) =>
{
    if(days == null||days<1 || days >30)
    {
        return Results.BadRequest("Provide a days query parameter betweeen 1 and 30");
    }
    var startDate = DateTime.UtcNow - TimeSpan.FromDays(days.Value);
    var results = await db.Precipitation.Where(precip => precip.ZipCode == zip  && precip.CreatedOn >startDate).ToListAsync();
    return Results.Ok(results);

});

// Configure the HTTP request pipeline.


app.Run();
