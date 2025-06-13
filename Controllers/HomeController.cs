using System.Text;
using Microsoft.AspNetCore.Mvc;
using AppCollection.Models;
using AppCollection.Services;
using AppCollection.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace AppCollection.Controllers;

/// <summary>
/// Main controller for the application. Displays the home page and handles requests for the clock and weather.
/// </summary>
[Authorize]
public class HomeController : Controller
{
    private readonly WeatherService _weatherService;
    private ApplicationDbContext _context;

    public HomeController(WeatherService weatherService, ApplicationDbContext context)
    {
        _weatherService = weatherService;
        _context = context;
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

    public IActionResult DatabaseError()
    {
        ViewBag.DatabaseUnavailable = true;
        return View("NoDatabase");
    }

    public IActionResult Privacy() => View();
    public IActionResult Error() => View();
    public IActionResult Credits() => View();

    public IActionResult Index()
    {
        // Get the 10 most recent search history records
        var recentSearches = _context.Set<SearchHistory>()
            .OrderByDescending(sh => sh.Date)
            .Take(10)
            .ToList();

        ViewBag.Searches = recentSearches;
        return View();
    }

    /// <summary>
    /// Retrieves and displays log files from the server. Filters and sorts log files
    /// by their last write time, providing a view with the list of available log files.
    /// </summary>
    /// <returns>A view containing the list of log files or an empty list if no logs exist.</returns>
    public IActionResult Log()
    {
        var logs = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        if (!Directory.Exists(logs))
        {
            ViewBag.LogFiles = new List<FileInfo>();
            return View();
        }

        var logFiles = new DirectoryInfo(logs).GetFiles("*.log")
            .OrderByDescending(f => f.LastWriteTime).ToList();
        ViewBag.LogFiles = logFiles;
        return View();
    }

    /// <summary>
    /// Provides functionality to download a specified log file from the server.
    /// </summary>
    /// <param name="fileName">The name of the log file to be downloaded.</param>
    /// <returns>An <see cref="IActionResult"/> that facilitates the download of the file if it exists; otherwise, a NotFound result if the file does not exist or the path is invalid.</returns>
    public IActionResult DownloadLog(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return NotFound();
        }

        var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        var filePath = Path.Combine(logPath, fileName);
        if (!System.IO.File.Exists(filePath) || Path.GetDirectoryName(filePath) != Path.GetFullPath(logPath))
            return NotFound();

        var contentType = "text/plain"; // Set appropriate content type if needed
        return PhysicalFile(filePath, contentType, fileName);
    }

    /// <summary>
    /// Provides functionality to preview the content of a log or text file safely, allowing limited content
    /// to be read and ensuring security measures like directory traversal prevention.
    /// </summary>
    /// <param name="fileName">The name of the log or text file to be previewed.</param>
    /// <returns>An <see cref="IActionResult"/> representing the previewed content of the file in plain text format,
    /// or appropriate error responses for invalid or unsupported retrieval operations.</returns>
    [HttpGet]
    public IActionResult PreviewLog(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return BadRequest();

        var logsPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        var filePath = Path.Combine(logsPath, fileName);

        // Prevent directory traversal
        if (!System.IO.File.Exists(filePath) || Path.GetDirectoryName(filePath) != Path.GetFullPath(logsPath))
            return NotFound();

        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        if (ext != ".log" && ext != ".txt")
            return BadRequest("Preview not supported for this file type.");

        string content;
        try
        {
            // Read a safe portion only (e.g., 100KB)
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var maxSize = 102_400;
                var buffer = new byte[maxSize];
                var bytesRead = stream.Read(buffer, 0, maxSize);
                content = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                if (stream.Length > maxSize)
                    content += "\n...\n(Log truncated for convenience. Full log is available in the 'Logs' folder. Download the log for full details.)";
            }
        }
        catch
        {
            return StatusCode(500, "Failed to read log file.");
        }

        return Content(content, "text/plain", Encoding.UTF8);
    }
}