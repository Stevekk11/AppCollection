using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SlovníHodiny.Models;

namespace SlovníHodiny.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Clock()
    {
        var clock = new Clock();
        var now = DateTime.Now;
        ViewBag.CurrentTime = now;
        return View(clock);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}