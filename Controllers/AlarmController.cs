using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppCollection.Models;

namespace AppCollection.Controllers;
/// <summary>
/// Controller for managing alarm functionality
/// </summary>
[Authorize]
public class AlarmController : Controller
{
    private readonly ApplicationDbContext _context;

    public AlarmController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var alarms = _context.Alarms.ToList();
        return View(alarms);
    }

    [HttpPost]
    public IActionResult Create(Alarm alarm)
    {
        if (ModelState.IsValid)
        {
            _context.Alarms.Add(alarm);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(alarm);
    }

    public IActionResult Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var alarm = _context.Alarms.FirstOrDefault(m => m.Id == id);
        if (alarm == null)
        {
            return NotFound();
        }

        return View(alarm);
    }

    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var alarm = _context.Alarms.Find(id);
        if (alarm == null)
        {
            return NotFound();
        }
        return View(alarm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, [Bind("Id,Name,Time,IsActive")] Alarm alarm)
    {
        if (id != alarm.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _context.Update(alarm);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(alarm);
    }

    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var alarm = _context.Alarms.FirstOrDefault(m => m.Id == id);
        if (alarm == null)
        {
            return NotFound();
        }

        return View(alarm);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var alarm = _context.Alarms.Find(id);
        if (alarm != null)
        {
            _context.Alarms.Remove(alarm);
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }
}