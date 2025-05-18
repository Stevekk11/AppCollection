namespace SlovníHodiny.Models;

public class DepartureViewModel
{
    public string StopName { get; set; }
    public string? Placeholder { get; set; }
    public List<string> AvailableStops { get; set; } = [];
    public List<DepartureDto> Departures { get; set; } = [];
    public DateTime Now { get; set; }
    public string? Error { get; set; }

    public class DepartureDto
    {
        public string Route { get; set; }
        public string Destination { get; set; }
        public int Minutes { get; set; }
        public string PlatformCode { get; set; }
        public int Delay { get; set; }
        public bool Ac { get; set; }
        public bool Krypl { get; set; }
    }
}