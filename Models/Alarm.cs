using System.ComponentModel.DataAnnotations;

namespace AppCollection.Models;

/// <summary>
/// Alarm model
/// </summary>
public class Alarm
{
    [Key]
    public int Id { get; set; }
    [Required]
    public TimeSpan Time { get; set; }

    public string Label { get; set; }
    public bool IsOn { get; set; }
    public List<DayOfWeek> RepeatDays { get; set; } = new();
    public AlarmType Type { get; set; }
    public string Sound { get; set; }
    public int SnoozeForMins { get; set; }
}

public enum AlarmType
{
    Beep, Nature, Radio
}