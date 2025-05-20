using System.Net.Http;
using System.Threading.Tasks;
using SlovníHodiny.Models;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace SlovníHodiny.Services;

/// <summary>
/// Service for retrieving weather information from wttr.in API.
/// </summary>

public class WeatherService
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the WeatherService class.
    /// </summary>
    /// <param name="httpClient">Optional HTTP client for making API requests. If not provided, a new instance will be created.</param>
    public WeatherService(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
    }

    /// <summary>
    /// Retrieves current weather information for the specified city.
    /// </summary>
    /// <param name="city">The name of the city to get weather for. Defaults to "Praha".</param>
    /// <returns>A Weather object containing current weather conditions, or null if the request fails.</returns>
    public async Task<Weather?> GetWeatherAsync(string city = "Praha")
    {
        try
        {
            string url = $"https://wttr.in/{city}?format=j1";
            var json = await _httpClient.GetStringAsync(url);
            var data = JObject.Parse(json);

            var current = data["current_condition"]?.First;
            if (current == null) return null;

            double temp = double.Parse((string)current["temp_C"]!, CultureInfo.InvariantCulture);
            double wind = double.Parse((string)current["windspeedKmph"]!, CultureInfo.InvariantCulture);
            double humidity = double.Parse((string)current["humidity"]!, CultureInfo.InvariantCulture);
            string iconUrl = "/cloud-blau.png";

            return new Weather
            {
                Date = DateTime.Now,
                TemperatureCelsius = temp,
                WindSpeedKmh = wind,
                HumidityPercent = humidity,
                IconUrl = iconUrl
            };
        }
        catch(Exception exception)
        {
            Console.WriteLine(exception.Message);
            return null;
        }
    }
}