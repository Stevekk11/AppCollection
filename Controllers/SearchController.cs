using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppCollection.Models;
using AppCollection.Services;

namespace AppCollection.Controllers;

/// <summary>
/// Provides functionality for handling search operations in the application.
/// </summary>
[Authorize]
public class SearchController : Controller
{
    private readonly SearchService _searchService;
    private ApplicationDbContext _context;

    public SearchController(SearchService searchService, ApplicationDbContext context)
    {
        _searchService = searchService;
        _context = context;
    }
    [HttpGet]
    public IActionResult Index()
    {
        return View(new SearchResultViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Index(SearchResultViewModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Query))
        {
            model.Result = await _searchService.SearchAsync(model.Query, GetCurrentUserId());
        }
        else
        {
            model.Result = "Zadejte slovo ke hledání";
        }
        return View(model);
    }

    private int GetCurrentUserId()
    {
        var username = User.Identity.Name;
        var usertype = Convert.ToByte(User.FindFirst("Usertype").Value);
        var user = _context.Logins.FirstOrDefault(x => x.Username == username && x.Usertype == usertype);
        return user?.Id_Login ?? throw new Exception("User not found");
    }

}