using Microsoft.AspNetCore.Mvc;
using SlovníHodiny.Models;

namespace SlovníHodiny.Controllers;

public class AlarmController : Controller
{
    private static List<Alarm> Alarms { get; set; } = new List<Alarm>();

    public IActionResult Index()
    {
        return View(Alarms);
    }

    public IActionResult Create()
    {
        return View(new Alarm
        {
            IsOn = true,
            RepeatDays = new List<DayOfWeek>()
        });
    }

    [HttpPost]
    public IActionResult Create(Alarm alarm)
    {
        alarm.Id = Alarms.Count > 0 ? Alarms.Max(a => a.Id) + 1 : 1;
        Alarms.Add(alarm);
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var alarm = Alarms.FirstOrDefault(a => a.Id == id);
        if (alarm == null) return NotFound();
        return View(alarm);
    }
    [HttpPost]
    public IActionResult Edit(Alarm alarm)
    {
        var existing = Alarms.FirstOrDefault(a => a.Id == alarm.Id);
        if (existing == null) return NotFound();

        // aktualizace hodnot:
        existing.Time = alarm.Time;
        existing.Label = alarm.Label;
        existing.IsOn = alarm.IsOn;
        existing.RepeatDays = alarm.RepeatDays;
        existing.Type = alarm.Type;
        existing.Sound = alarm.Sound;
        existing.SnoozeForMins = alarm.SnoozeForMins;
        return RedirectToAction(nameof(Index));
    }
    public IActionResult Delete(int id)
    {
        var alarm = Alarms.FirstOrDefault(a => a.Id == id);
        if (alarm == null) return NotFound();
        return View(alarm);
    }

    [HttpPost, ActionName("DeleteConfirmed")]
    public IActionResult DeleteConfirmed(int id)
    {
        var alarm = Alarms.FirstOrDefault(a => a.Id == id);
        if (alarm != null)
            Alarms.Remove(alarm);
        return RedirectToAction(nameof(Index));
    }

}