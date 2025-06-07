using System.Text.Json;
using AppCollection.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppCollection.Controllers;
[Route("api/radio")]
public class RadioController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private string? _streamUrl;
    private string? _nowPlaying;

    public RadioController(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _streamUrl = _configuration["RadioURL:HitradioCity"];
        _nowPlaying = _configuration["NowPlaying:HitradioCity"];
    }

    public async Task<IActionResult> Player()
    {
        var song = await GetCurrentSongInfo();
        ViewBag.StreamUrl = _streamUrl;
        return View(song);
    }

    private async Task<RadioSong?> GetCurrentSongInfo()
    {
        try
        {
            var response = await _httpClient.GetAsync(_nowPlaying);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var json = await response.Content.ReadAsStringAsync();
            var song = JsonSerializer.Deserialize<RadioSong>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return song;
        }
        catch (Exception e)
        {
            ViewBag.RadioErr = e.Message;
            return null;
        }
    }
    [HttpGet("now")]
    public async Task<IActionResult> GetNowPlaying()
    {
        using var http = new HttpClient();
        var url = _nowPlaying;
        var response = await http.GetFromJsonAsync<RadioSong>(url);

        if (response == null)
            return NotFound();

        return Ok(response);
    }
}