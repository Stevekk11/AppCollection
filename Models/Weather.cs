namespace SlovníHodiny.Models;

public class Weather
{
    public DateTime Date { get; set; }
    public double TemperatureCelsius { get; set; }
    public string IconUrl { get; set; }
    public double? WindSpeedKmh { get; set; }
    public double? HumidityPercent { get; set; }
}