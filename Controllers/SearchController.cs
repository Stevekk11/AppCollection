using Microsoft.AspNetCore.Mvc;
using SlovníHodiny.Models;
using SlovníHodiny.Services;

namespace SlovníHodiny.Controllers;

public class SearchController : Controller
{
    private readonly SearchService _searchService;

    public SearchController(SearchService searchService)
    {
        _searchService = searchService;
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
            model.Result = await _searchService.SearchAsync(model.Query);
        }
        else
        {
            model.Result = "Zadejte slovo ke hledání";
        }
        return View(model);
    }

}