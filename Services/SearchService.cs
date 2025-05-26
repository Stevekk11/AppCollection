using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Web;
using System.Text;
using AppCollection.Models;


namespace AppCollection.Services;

/// <summary>
/// Service responsible for performing web searches and extracting structured data from the results.
/// Specifically designed to work with lustit.cz website for word searches.
/// </summary>

public class SearchService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the SearchService class.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory used for creating HTTP clients.</param>
    /// <param name="context"> database</param>
    public SearchService(IHttpClientFactory httpClientFactory, ApplicationDbContext context)
    {
        _httpClientFactory = httpClientFactory;
        _context = context;
    }

    /// <summary>
    /// Performs an asynchronous search query on a website for crosswords API.
    /// </summary>
    /// <param name="query">The search query string to look up.</param>
    /// <returns>A string containing the formatted search results or an error message if the search fails.</returns>
    public async Task<string?> SearchAsync(string query)
    {
        var client = _httpClientFactory.CreateClient();
        var url = "https://lustit.cz/najit?otazka=" + HttpUtility.UrlEncode(query);
        try
        {
            var html = await client.GetStringAsync(url);
            var hist = new SearchHistory
            {
                Word = query,
                Date = DateTime.Now,
            };
            _context.Add(hist);
            await _context.SaveChangesAsync();
            return ExtractTableData(html);
        }
        catch (Exception e)
        {
            return "Chyba načítání výsledků, " + e.Message;
        }
    }

    /// <summary>
    /// Extracts and formats table data from the HTML content retrieved from the search results.
    /// </summary>
    /// <param name="html">The HTML content to parse.</param>
    /// <returns>A formatted string containing the extracted data or an error message if extraction fails.</returns>
    private string? ExtractTableData(string html)
    {
        var sb = new StringBuilder();
        try
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var table = doc.DocumentNode.SelectSingleNode("//table[contains(@class, 'upravit')]");
            if (table != null)
            {
                var rows = table.SelectNodes(".//tr");
                if (rows != null)
                {
                    for (int i = 1; i < rows.Count; i++)
                    {
                        var cols = rows[i].SelectNodes(".//td");
                        if (cols != null && cols.Count >= 2)
                        {
                            string question = cols[0].InnerText.Trim();
                            string answer = cols[1].InnerText.Trim();
                            sb.AppendLine($"Otázka: {question} Odpověď: {answer}");
                        }
                    }
                }
            }

            if (sb.Length == 0)
                return "Chyba načítání výsledků - špatné slovo";
            return sb.ToString();
        }
        catch (Exception e)
        {
            return "Chyba HTML extrakce, zkuste to znova, " + e.Message;
        }
    }
}