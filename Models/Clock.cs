namespace SlovníHodiny.Models;

public class Clock
{
    public string[] Hours = { "dvanáct", "jedna", "dvě", "tři", "čtyři", "pět", "šest", "sedm", "osm", "devět", "deset", "jedenáct", "dvanáct" };
    public string[] Minutes = {"nula","deset", "dvacet", "třicet", "čtyřicet", "padesát"};

    public HashSet<string> GetActiveWords(DateTime time)
    {
        var words = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        //přiřazení hodin a minut
        int hour = time.Hour % 12;
        if (hour == 0) hour = 12;
        int minuteSlot = (int)Math.Round(time.Minute / 5.0);
        if (minuteSlot >= Minutes.Length) minuteSlot = 0;

        string hod = Hours[hour];
        string min = Minutes[minuteSlot];


        // Spojky a tvar
        if (hour == 1)
        {
            words.Add("je");
            words.Add("hodina");
        }
        else if (hour >= 2 && hour <= 4)
        {
            words.Add("jsou");
            words.Add("hodiny");
        }
        else
        {
            words.Add("je");
            words.Add("hodin");
        }

        words.Add(hod);
        words.Add(min);
        words.Add("minut");

        return words;
    }
}
