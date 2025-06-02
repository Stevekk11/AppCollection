using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using AppCollection.Models;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;

namespace AppCollection.Controllers;

/// <summary>
/// Controller for managing public transport departures
/// </summary>
[Authorize]
public class DeparturesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public DeparturesController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    /// <summary>
    /// Displays departure information for a specified stop
    /// </summary>
    /// <param name="stopName">Name of the stop to show departures for</param>
    /// <returns>View with departure information</returns>
    public IActionResult Transport(string stopName = "")
    {
        var model = BuildViewModel(stopName);
        return View(model);
    }

    /// <summary>
    /// Processes stop search form submission and saves the stop to search history
    /// </summary>
    /// <param name="stopName">Name of the stop to search for</param>
    /// <returns>Redirects to Index action with search parameters</returns>
    [HttpPost]
    public IActionResult Search(string stopName)
    {
        var hist = new SearchHistory
        {
            Word = stopName,
            Date = DateTime.Now,
            UserId = GetCurrentUserId()
        };
        _context.Add(hist);
        _context.SaveChanges();
        var culture = Thread.CurrentThread.CurrentCulture.Name;
        return RedirectToAction("Transport", new { stopName, actionType = "search", culture});
    }

    /// <summary>
    /// Builds the view model for departure information
    /// </summary>
    /// <param name="stopName">Name of the stop to fetch departures for</param>
    /// <returns>Populated DepartureViewModel instance</returns>
    private DepartureViewModel BuildViewModel(string stopName)
    {
        var vm = new DepartureViewModel
        {
            Now = DateTime.Now,
            AvailableStops = new List<string>(), // Will fill shortly, avoid LOH warnings PLSS
            Placeholder = "",
        };

        var stopsJson = System.IO.File.ReadAllText("wwwroot/stops.json");
        var stopsData = JObject.Parse(stopsJson);
        var stopNames = stopsData["stopGroups"]!
            .Select(item => item["name"]?.ToString())
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Distinct()
            .ToList();

        vm.AvailableStops = stopNames;
        if (stopNames.Any())
        {
            var random = new Random();
            vm.Placeholder = stopNames[random.Next(stopNames.Count)];
        }

        if (!string.IsNullOrEmpty(stopName))
        {
            // Call Golemio API
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-Access-Token", _configuration["Golemio:ApiKey"]);
            var url = "https://api.golemio.cz/v2/pid/departureboards";
            var query =
                $"names={Uri.EscapeDataString(stopName)}&minutesBefore=0&minutesAfter=100&timeFrom={DateTime.UtcNow:o}&includeMetroTrains=true&airCondition=true&preferredTimezone=Europe/Prague&mode=departures&order=real&filter=none&skip=canceled&limit=20";
            try
            {
                var result = client.GetStringAsync($"{url}?{query}").Result;
                var data = JObject.Parse(result);
                Console.WriteLine(result);
                vm.Departures = (data["departures"] ?? new JArray())
                    .Select(item => new DepartureViewModel.DepartureDto
                    {
                        Route = item["route"]?["short_name"]?.ToString(),
                        Destination = item["trip"]?["headsign"]?.ToString(),
                        Minutes = int.TryParse(item["departure_timestamp"]?["minutes"]?.ToString(), out var m) ? m : 0,
                        PlatformCode = item["stop"]?["platform_code"]?.ToString(),
                        Delay = int.TryParse(item["delay"]?["minutes"]?.ToString(), out var d) ? d : 0,
                        Ac = (item["trip"]?["is_air_conditioned"]?.Type == JTokenType.Boolean)
                            ? item["trip"]["is_air_conditioned"].Value<bool>()
                            : false,
                        Krypl = (item["trip"]?["is_wheelchair_accessible"]?.Type == JTokenType.Boolean)
                            ? item["trip"]["is_wheelchair_accessible"].Value<bool>()
                            : false
                    })
                    .ToList();
            }
            catch
            {
                vm.Error = "Failed to fetch departures. Please try again.";
            }

            vm.StopName = stopName;
        }

        return vm;
    }

    /// <summary>
    /// Retrieves the unique identifier of the currently authenticated user.
    /// </summary>
    /// <returns>The ID of the currently authenticated user.</returns>
    /// <exception cref="Exception">Thrown if the user is not found or authentication details are invalid.</exception>
    private int GetCurrentUserId()
    {
        var username = User.Identity.Name;
        var usertype = Convert.ToByte(User.FindFirst("Usertype").Value);
        var user = _context.Logins.FirstOrDefault(x => x.Username == username && x.Usertype == usertype);
        return user?.Id_Login ?? throw new Exception("User not found");
    }
}