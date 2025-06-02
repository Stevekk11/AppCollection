namespace AppCollection.Models;

/// <summary>
/// Represents the view model for search results, containing the user's query and the resulting data from the search operation.
/// </summary>
public class SearchResultViewModel
{
    public string? Query { get; set; }
    public string? Result { get; set; }
}