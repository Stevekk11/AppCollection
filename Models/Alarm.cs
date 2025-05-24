namespace AppCollection.Models;
/// <summary>
/// Alarm model, includes all alarm settings.
/// </summary>
public class Alarm
{
    private int id;
    private TimeSpan time;
    private string label;
    private bool isOn;
    public List<DayOfWeek> repeatDays = new List<DayOfWeek>();
    private AlarmType type;
    private string sound;
    private int snoozeForMins;

    public int Id
    {
        get => id;
        set => id = value;
    }

    public TimeSpan Time
    {
        get => time;
        set => time = value;
    }

    public string Label
    {
        get => label;
        set => label = value ?? throw new ArgumentNullException(nameof(value));
    }

    public bool IsOn
    {
        get => isOn;
        set => isOn = value;
    }

    public List<DayOfWeek> RepeatDays
    {
        get => repeatDays;
        set => repeatDays = value ?? throw new ArgumentNullException(nameof(value));
    }

    public AlarmType Type
    {
        get => type;
        set => type = value;
    }

    public string Sound
    {
        get => sound;
        set => sound = value ?? throw new ArgumentNullException(nameof(value));
    }

    public int SnoozeForMins
    {
        get => snoozeForMins;
        set => snoozeForMins = value;
    }
}



public enum AlarmType
{
    Beep,
    Nature,
    Radio
}
