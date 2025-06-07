namespace AppCollection.Models;

public class RadioSong
{
    public int Id { get; set; }
    public string Interpret { get; set; }
    public string Song { get; set; }
    public DateTime BeginAt { get; set; }
    public DateTime EndAt { get; set; }
    public string Image { get; set; }
}