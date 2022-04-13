using Cloudweather.Report.BusinessLogic;
using Cloudweather.Report.Config;
using Cloudweather.Report.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpClient();
builder.Services.AddTransient<IWeatherReportAggregator, WeatherReportAggregator>();
builder.Services.AddOptions();
builder.Services.Configure<WeatherDataConfig>(builder.Configuration.GetSection("WeatherDataConfig"));

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


builder.Services.AddDbContext<WeatherReportDbContext>(
    opts =>
    {
        opts.EnableSensitiveDataLogging();
        opts.EnableDetailedErrors();
        opts.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"));
    }, ServiceLifetime.Transient);
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGet("/weather-report/{zip}", async (string zip, [FromQuery] int? days, IWeatherReportAggregator weatherReportAggregator) =>
 {
     if(days==null || days < 1 || days > 30)
     {
         return Results.BadRequest("Please provide a days query parameter with values between 1 and 30");
     }
     var report = await weatherReportAggregator.BuildReport(zip, days.Value);
     return Results.Ok(report);
 });

app.Run();
