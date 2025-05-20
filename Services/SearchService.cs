using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Web;
using System.Text;


namespace SlovníHodiny.Services;

public class SearchService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SearchService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string?> SearchAsync(string query)
    {
        var client = _httpClientFactory.CreateClient();
        var url = "https://lustit.cz/najit?otazka=" + HttpUtility.UrlEncode(query);
        try
        {
            var html = await client.GetStringAsync(url);
            return ExtractTableData(html);
        }
        catch (Exception e)
        {
            return "Chyba načítání výsledků, " + e.Message;
        }
    }

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