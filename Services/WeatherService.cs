using System.Net.Http;
using System.Threading.Tasks;
using SlovníHodiny.Models;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace SlovníHodiny.Services;

public class WeatherService
{
    private readonly HttpClient _httpClient;

    public WeatherService(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
    }

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