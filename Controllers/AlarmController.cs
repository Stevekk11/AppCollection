using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlovníHodiny.Models;

namespace SlovníHodiny.Controllers;
/// <summary>
/// Controller for managing alarm functionality
/// </summary>
[Authorize]
public class AlarmController : Controller
{
    private static List<Alarm> Alarms { get; set; } = new List<Alarm>();

    /// <summary>
    /// Displays a list of all alarms
    /// </summary>
    /// <returns>View containing list of alarms</returns>
    public IActionResult Index()
    {
        return View(Alarms);
    }

    /// <summary>
    /// Displays the alarm creation form
    /// </summary>
    /// <returns>View with empty alarm form</returns>
    public IActionResult Create()
    {
        return View(new Alarm
        {
            IsOn = true,
            RepeatDays = new List<DayOfWeek>()
        });
    }

    /// <summary>
    /// Processes the alarm creation form submission
    /// </summary>
    /// <param name="alarm">The alarm to create</param>
    /// <returns>Redirects to Index on success</returns>
    [HttpPost]
    public IActionResult Create(Alarm alarm)
    {
        alarm.Id = Alarms.Count > 0 ? Alarms.Max(a => a.Id) + 1 : 1;
        Alarms.Add(alarm);
        return RedirectToAction(nameof(Index));
    }
    /// <summary>
    /// Displays the alarm editing form
    /// </summary>
    /// <param name="id">ID of the alarm to edit</param>
    /// <returns>View with alarm details or NotFound</returns>
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var alarm = Alarms.FirstOrDefault(a => a.Id == id);
        if (alarm == null) return NotFound();
        return View(alarm);
    }
    /// <summary>
    /// Processes the alarm edit form submission
    /// </summary>
    /// <param name="alarm">The updated alarm data</param>
    /// <returns>Redirects to Index on success or NotFound</returns>
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
    /// <summary>
    /// Displays the alarm deletion confirmation page
    /// </summary>
    /// <param name="id">ID of the alarm to delete</param>
    /// <returns>View with alarm details or NotFound</returns>
    public IActionResult Delete(int id)
    {
        var alarm = Alarms.FirstOrDefault(a => a.Id == id);
        if (alarm == null) return NotFound();
        return View(alarm);
    }

    /// <summary>
    /// Processes the alarm deletion confirmation
    /// </summary>
    /// <param name="id">ID of the alarm to delete</param>
    /// <returns>Redirects to Index after deletion</returns>
    [HttpPost, ActionName("DeleteConfirmed")]
    public IActionResult DeleteConfirmed(int id)
    {
        var alarm = Alarms.FirstOrDefault(a => a.Id == id);
        if (alarm != null)
            Alarms.Remove(alarm);
        return RedirectToAction(nameof(Index));
    }

}