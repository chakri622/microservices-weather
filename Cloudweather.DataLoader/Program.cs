

using Cloudweather.DataLoader.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appSettings.json")
    .AddEnvironmentVariables()
    .Build();


//Get Services Configuration
var servicesConfig = config.GetSection("Services");
var temperatureServiceConfig = servicesConfig.GetSection("Temperature");
var tempServiceHost = temperatureServiceConfig["Host"];
var tempServicePort = temperatureServiceConfig["port"];


var precipServiceConfig = servicesConfig.GetSection("Precipitation");
var precipServiceHost = precipServiceConfig["Host"];
var precipServicePort = precipServiceConfig["Port"];

var zipCodes = new List<string>
{
    "73026",
    "68104",
    "04401",
    "32808",
    "19717"
};

Console.WriteLine("Starting Dataload ...");

//create http clients for temperature and http services
var temperatureHttpClient = new HttpClient();
temperatureHttpClient.BaseAddress = new Uri($"http://{tempServiceHost}:{tempServicePort}");

var precipitationHttpClient = new HttpClient();
precipitationHttpClient.BaseAddress = new Uri($"http://{precipServiceHost}:{precipServicePort}");

//Process Zip codes

foreach (var zip in zipCodes)
{
    Console.WriteLine("Processing zip codes...");
    var from = DateTime.Now.AddYears(-2);
    var to = DateTime.Now;
    for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
    {
        var temps = postTemp(zip, day, temperatureHttpClient);
        PostPrecip(temps[0], zip, day, precipitationHttpClient);
    }



}

void PostPrecip(int lowTemp, string zip, DateTime day, HttpClient precipitationHttpClient)
{
    var rand = new Random();
    var isPrecip = rand.Next(2) < 1;
    PrecipitationModel precipitation;
    if (isPrecip)
    {
        var precipInches = rand.Next(1, 16);
        if (lowTemp < 32)
        {
            precipitation = new PrecipitationModel
            {
                AmountInches = precipInches,
                WeatherType = "snow",
                ZipCode = zip,
                CreatedOn = day
            };
        }
        else
        {

            precipitation = new PrecipitationModel
            {
                AmountInches = precipInches,
                WeatherType = "rain",
                ZipCode = zip,
                CreatedOn = day
            };

        }
    }
    else
    {
        precipitation = new PrecipitationModel
        {
            AmountInches = 0,
            WeatherType = "none",
            ZipCode = zip,
            CreatedOn = day
        };
    }

    var precipResponse = precipitationHttpClient.PostAsJsonAsync("observation", precipitation).Result;

    if (precipResponse.IsSuccessStatusCode)
    {
        Console.WriteLine($"Posted precipitation: Date: {day:d}" +
                          $"Zip: {zip}" +
                          $"Type: {precipitation.WeatherType}" +
                          $"Amount (in.): {precipitation.AmountInches}"

            );
    }
    else
    {
        Console.WriteLine(precipResponse.ToString());
    }

   

}

List<int> postTemp(string zip, DateTime day, HttpClient temperatureHttpClient)
{
    var rand =new Random();
    var t1 = rand.Next(0, 100);
    var t2=rand.Next(0, 100);
    var hiloTemps = new List<int> { t1, t2 };
    hiloTemps.Sort();
    var temperatureObservation = new TemperatureModel
    {
        TempLowF = hiloTemps[0],
        TempHighF = hiloTemps[1],
        ZipCode = zip,
        CreatedOn = day
    };

    var tempResponse = temperatureHttpClient.PostAsJsonAsync("observation",temperatureObservation).Result;
    if (tempResponse.IsSuccessStatusCode)
    {
        Console.WriteLine($"Posted Temperature: Date: {day:d}" +
            $"Zip: {zip}" +
            $"Lo (F): {hiloTemps[0]}" +
            $"Hi (F): {hiloTemps[1]}");
    }
    else
    {
        Console.WriteLine(tempResponse.ToString()); 
    }
    return hiloTemps;
}