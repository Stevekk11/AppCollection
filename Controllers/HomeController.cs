using Microsoft.AspNetCore.Mvc;
using SlovníHodiny.Models;
using SlovníHodiny.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;

namespace SlovníHodiny.Controllers;
/// <summary>
/// Main controller for the application. Displays the home page and handles requests for the clock and weather.
/// </summary>
[Authorize]
public class HomeController : Controller
{
    private readonly WeatherService _weatherService;

    public HomeController(WeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    /// <summary>
    /// Represents a clock model that provides functionality to retrieve active words
    /// representing the current time in a textual format based on a specific context.
    /// </summary>
    public async Task<IActionResult> Clock(string city)
    {
        var clockModel = new Clock();
        var cityToUse = string.IsNullOrWhiteSpace(city) ? "Praha" : city;
        var weatherModel = await _weatherService.GetWeatherAsync(cityToUse);

        var viewModel = new ClockWeatherViewModel
        {
            Clock = clockModel,
            Weather = weatherModel
        };
        return View(viewModel);
    }

    public IActionResult Privacy() => View();
    public IActionResult Index() => View();
}