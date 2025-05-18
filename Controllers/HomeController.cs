using Microsoft.AspNetCore.Mvc;
using SlovníHodiny.Models;
using SlovníHodiny.Services;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SlovníHodiny.Controllers;

public class HomeController : Controller
{
    private readonly WeatherService _weatherService;

    public HomeController(WeatherService weatherService)
    {
        _weatherService = weatherService;
    }

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

    //public IActionResult Index() => View();
    public IActionResult Privacy() => View();
    public IActionResult Index() => View();
}