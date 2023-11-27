using AutoMapper;
using AwesomeEvents.Entities;
using AwesomeEvents.Models;
using AwesomeEvents.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AwesomeEvents.Controllers;

[Route("api/events")]
[ApiController]
public class EventsController : ControllerBase
{
    private readonly EventsDbContext _context;
    private readonly IMapper _mapper;
    
    public EventsController(EventsDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    /// <summary>
    ///     Get all events
    /// </summary>
    /// <returns>Events collection</returns>
    /// <response code="200">Success</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var events = _context.Events.Where(currentEvent => !currentEvent.IsDeleted).ToList();
        var viewModel = _mapper.Map<List<EventViewModel>>(events);

        return Ok(viewModel);
    }
    
    /// <summary>
    ///     Get a specific event
    /// </summary>
    /// <param name="id">Event identifier</param>
    /// <returns>A specific event</returns>
    /// <response code="200">Success</response>
    /// <response code="404">Not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var currentEvent = _context.Events
                                   .Include(currentEvent => currentEvent.Speakers)
                                   .SingleOrDefault(currentEvent => currentEvent.Id == id);

        if (currentEvent == null) return NotFound();

        var viewModel = _mapper.Map<EventViewModel>(currentEvent);
        
        return Ok(viewModel);
    }
    
    /// <summary>
    ///     Create an event
    /// </summary>
    /// <remarks>{"id":"3fa85f64-5717-4562-b3fc-2c963f66afa6","title":"string","description":"string","startDate":"2023-11-27T16:38:17.837Z","endDate":"2023-11-27T16:38:17.837Z","speakers":[{"id":"3fa85f64-5717-4562-b3fc-2c963f66afa6","name":"string","talkTitle":"string","talkDescription":"string","linkedInUrl":"string","eventId":"3fa85f64-5717-4562-b3fc-2c963f66afa6"}],"isDeleted":true}</remarks>
    /// <param name="inputModel">New event data</param>
    /// <returns>The new event</returns>
    /// <response code="201">Success created</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult Create(EventInputModel inputModel)
    {
        var currentEvent = _mapper.Map<Event>(inputModel);
        
        _context.Events.Add(currentEvent);
        _context.SaveChanges();
        
        return CreatedAtAction(nameof(GetById), new { id = currentEvent.Id }, currentEvent);
    }
    
    /// <summary>
    ///     Update a specific event
    /// </summary>
    /// <remarks>{"id":"3fa85f64-5717-4562-b3fc-2c963f66afa6","title":"string","description":"string","startDate":"2023-11-27T16:39:40.878Z","endDate":"2023-11-27T16:39:40.878Z","speakers":[{"id":"3fa85f64-5717-4562-b3fc-2c963f66afa6","name":"string","talkTitle":"string","talkDescription":"string","linkedInUrl":"string","eventId":"3fa85f64-5717-4562-b3fc-2c963f66afa6"}],"isDeleted":true}</remarks>
    /// <param name="id">Event identifier</param>
    /// <param name="inputModel">Event data</param>
    /// <returns>Nothing</returns>
    /// <response code="204">No content</response>
    /// <response code="404">Not found</response>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(Guid id, EventInputModel inputModel)
    {
        var nextCurrentEvent = _mapper.Map<Event>(inputModel);
        var currentEvent = _context.Events.SingleOrDefault(currentEvent => currentEvent.Id == id);

        if (currentEvent == null) return NotFound();

        currentEvent.Update(nextCurrentEvent.Title, nextCurrentEvent.Description, nextCurrentEvent.StartDate, nextCurrentEvent.EndDate);

        _context.Events.Update(currentEvent);
        _context.SaveChanges();
        
        return NoContent();
    }

    /// <summary>
    ///     Delete an event
    /// </summary>
    /// <param name="id">Event identifier</param>
    /// <returns>Nothing</returns>
    /// <response code="204">No content</response>
    /// <response code="404">Not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        var currentEvent = _context.Events.SingleOrDefault(currentEvent => currentEvent.Id == id);

        if (currentEvent == null) return NotFound();
        
        currentEvent.Delete();

        _context.SaveChanges();

        return NoContent();
    }

    /// <summary>
    ///     Create new speaker
    /// </summary>
    /// <remarks>{"id":"3fa85f64-5717-4562-b3fc-2c963f66afa6","name":"string","talkTitle":"string","talkDescription":"string","linkedInUrl":"string","eventId":"3fa85f64-5717-4562-b3fc-2c963f66afa6"}</remarks>
    /// <param name="id">Event identifier</param>
    /// <param name="inputModel">Speaker data</param>
    /// <returns>Nothing</returns>
    /// <response code="204">No content</response>
    /// <response code="404">Not found</response>
    [HttpPost("{id}/speakers")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult CreateSpeaker(Guid id, EventSpeakerInputModel inputModel)
    {
        var eventSpeaker = _mapper.Map<EventSpeaker>(inputModel);

        eventSpeaker.Id = id;
        
        var hasEvent = _context.Events.Any(currentEvent => currentEvent.Id == id);

        if (!hasEvent) return NotFound();

        _context.EventSpeakers.Add(eventSpeaker);
        _context.SaveChanges();

        return NoContent();
    }
}
