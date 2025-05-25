using AppCollection.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppCollection.Controllers;

/// <summary>
/// Provides functionality for managing and interacting with events in the application.
/// This controller supports operations such as retrieving user-specific events,
/// displaying a calendar view, and performing CRUD (Create, Read, Update, Delete)
/// operations on event data.
/// </summary>
/// <remarks>
/// This controller interacts with the application's database through the
/// ApplicationDbContext class and assumes authentication is in place to obtain
/// the current user's context for operations.
/// </remarks>
public class EventsController : Controller
{
    private readonly ApplicationDbContext _context;
    public EventsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Events/UserEvents
    public async Task<IActionResult> UserEvents()
    {
        int userId = GetCurrentUserId();
        var events = await _context.Events
            .Where(e => e.UserId == userId)
            .Select(e => new {
                id = e.Id,
                title = e.Name,
                start = e.StartDate,
                end = e.EndDate
            })
            .ToListAsync();
        return Json(events);
    }
    // GET
    public IActionResult Calendar()
    {
        return View();
    }


    // POST: /Events/Add
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] Event model)
    {
        int userId = GetCurrentUserId();
        model.UserId = userId;
        _context.Events.Add(model);
        await _context.SaveChangesAsync();
        return Ok(model);
    }

    // PUT: /Events/Edit/
    [HttpPut]
    public async Task<IActionResult> Edit(int id, [FromBody] Event model)
    {
        var evt = await _context.Events.FindAsync(id);
        if (evt == null) return NotFound();
        evt.Name = model.Name;
        evt.StartDate = model.StartDate;
        evt.EndDate = model.EndDate;
        await _context.SaveChangesAsync();
        return Ok(evt);
    }

    // DELETE: /Events/Delete/
    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        var evt = await _context.Events.FindAsync(id);
        if (evt == null) return NotFound();
        _context.Events.Remove(evt);
        await _context.SaveChangesAsync();
        return Ok();
    }

    /// <summary>
    /// Retrieves the ID of the currently authenticated user from the database.
    /// </summary>
    /// <returns>The ID of the current user as an integer.</returns>
    /// <exception cref="Exception">Thrown if no matching user is found in the database.</exception>
    private int GetCurrentUserId()
    {
        var username = User.Identity.Name;
        var usertype = Convert.ToByte(User.FindFirst("Usertype").Value);
        var user = _context.Logins.FirstOrDefault(x => x.Username == username && x.Usertype == usertype);
        return user?.Id_Login ?? throw new Exception("User not found");
    }
}
