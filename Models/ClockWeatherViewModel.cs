using AppCollection.Helpers;

namespace AppCollection.Models;
/// <summary>
/// Simple weather model.
/// </summary>
public class ClockWeatherViewModel
{
    public Clock Clock { get; set; }
    public Weather Weather { get; set; }
}