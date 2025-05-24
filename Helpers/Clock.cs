namespace AppCollection.Helpers;

/// <summary>
/// Represents an active word with associated textual content and context.
/// </summary>
/// <param name="Text">The text of the word.</param>
/// <param name="Context">
/// The context in which the word is used. Possible values include "hodiny" (hours) or "minuty" (minutes).
/// </param>
public record ActiveWord(string Text, string Context); // Context can be "hodiny" nebo "minuty"
public class Clock
{
    public string[] Hours = { "dvanáct", "jedna", "dvě", "tři", "čtyři", "pět", "šest", "sedm", "osm", "devět", "deset", "jedenáct", "dvanáct" };
    public string[] Minutes = {"nula","deset", "dvacet", "třicet", "čtyřicet", "padesát"};

    /// <summary>
    /// Retrieves a list of active words representing the current time in a textual form.
    /// </summary>
    /// <param name="time">The DateTime object representing the time for which active words will be generated.</param>
    /// <returns>A list of <see cref="ActiveWord"/> objects representing the active words based on the provided time.</returns>
    public List<ActiveWord> GetActiveWords(DateTime time)
    {
        var words = new List<ActiveWord>();

        int hour = time.Hour % 12;
        if (hour == 0) hour = 12;
        int minuteSlot = (int)Math.Round(time.Minute / 10.0);
        if (minuteSlot >= Minutes.Length) minuteSlot = 0;

        string hod = Hours[hour];
        string min = Minutes[minuteSlot];

        words.Add(new ActiveWord("a", "spojka"));

        if (hour == 1)
        {
            words.Add(new ActiveWord("je", "sloveso"));
            words.Add(new ActiveWord("hodina", "hodiny"));
        }
        else if (hour >= 2 && hour <= 4)
        {
            words.Add(new ActiveWord("jsou", "sloveso"));
            words.Add(new ActiveWord("hodiny", "hodiny"));
        }
        else
        {
            words.Add(new ActiveWord("je", "sloveso"));
            words.Add(new ActiveWord("hodin", "hodiny"));
        }

        words.Add(new ActiveWord(hod, "hodiny"));

        if (minuteSlot != 0)
        {
            words.Add(new ActiveWord(min, "minuty"));
            words.Add(new ActiveWord("minut", "minuty"));
        }

        return words;
    }
}