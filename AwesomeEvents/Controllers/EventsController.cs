using AwesomeEvents.Entities;
using AwesomeEvents.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace AwesomeEvents.Controllers;

[Route("api/events")]
[ApiController]
public class EventsController : ControllerBase
{
    private readonly EventsDbContext _context;
    
    public EventsController(EventsDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public IActionResult GetAll()
    {
        var events = _context.Events.Where(currentEvent => !currentEvent.IsDeleted).ToList();

        return Ok(events);
    }
    
    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var currentEvent = _context.Events.Find(currentEvent => currentEvent.Id == id);

        if (currentEvent == null) return NotFound();
        
        return Ok(currentEvent);
    }
    
    [HttpPost]
    public IActionResult Create(Event currentEvent)
    {
        _context.Events.Add(currentEvent);
        
        return CreatedAtAction(nameof(GetById), new { id = currentEvent.Id }, currentEvent);
    }
    
    [HttpPatch("{id}")]
    public IActionResult Update(Guid id, Event nextCurrentEvent)
    {
        var currentEvent = _context.Events.Find(currentEvent => currentEvent.Id == id);

        if (currentEvent == null) return NotFound();

        currentEvent.Update(nextCurrentEvent.Title, nextCurrentEvent.Description, nextCurrentEvent.StartDate, nextCurrentEvent.EndDate);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        var currentEvent = _context.Events.Find(currentEvent => currentEvent.Id == id);

        if (currentEvent == null) return NotFound();
        
        currentEvent.Delete();

        return NoContent();
    }

    [HttpPost("{id}/speakers")]
    public IActionResult CreateSpeaker(Guid id, EventSpeaker eventSpeaker)
    {
        var currentEvent = _context.Events.Find(currentEvent => currentEvent.Id == id);

        if (currentEvent == null) return NotFound();
        
        currentEvent.Speakers.Add(eventSpeaker);

        return NoContent();
    }
}
