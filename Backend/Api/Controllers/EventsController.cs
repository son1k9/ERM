using Api.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Api.Controllers;
[Route("/event")]
[ApiController]
public class EventsController : ControllerBase
{
    public class EventResponse(Event _event, User user)
    {
        public int Id { get; set; } = _event.Id;
        public string Name { get; set; } = _event.Name;
        public string Description { get; set; } = _event.Description;
        public DateTime Date { get; set; } = _event.Date;
        public string Place { get; set; } = _event.Place;
        public bool Canceled { get; set; } = _event.Canceled;
        public UsersController.UserResponse Organizer { get; set; } = new UsersController.UserResponse(user);
    }

    public class EventRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
        public string Place { get; set; } = string.Empty;
        public bool Canceled { get; set; }

        public Event ToEvent()
        {
            return new Event
            {
                Name = Name.Trim(),
                Description = Description.Trim(),
                Date = Date,
                Place = Place.Trim(),
                Canceled = Canceled
            };
        }
    }


    /// <summary>
    /// Get event by ID
    /// </summary>
    /// <param name="id">Event ID.</param>
    /// <returns>Event details if found, otherwise NotFound.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<EventResponse> GetEvent(int id, Model model)
    {
        var _event = model.Events.Get(id);

        if (_event == null)
        {
            return NotFound();
        }

        var user = model.Users.Get(_event.OrganizerId);
        if (user == null)
        {
            return NotFound();
        }

        return new EventResponse(_event, user);
    }

    /// <summary>
    /// Create a new event. Requires authentication
    /// </summary>
    /// <param name="request">Event details for creation.</param>
    /// <returns>Created event details if succesfull, otherwise BadRequest with validation errors.</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<EventResponse> CreateEvent(EventRequest request, Model model)
    {
        var user = Helpers.GetAuthenticatedUser(HttpContext);
        var _event = request.ToEvent();

        var errors = _event.Validate();
        if (errors.Count > 0)
        {
            return ValidationProblem(new ValidationProblemDetails(errors));
        }

        _event.OrganizerId = user.Id;
        _event.Id = model.Events.Insert(_event);

        return Created($"/event/{_event.Id}", new EventResponse(_event, user));
    }

    /// <summary>
    /// Update an existing event. Requires authentication
    /// </summary>
    /// <param name="id">Event ID.</param>
    /// <param name="request">Event details.</param>
    /// <returns>No content if succesfull, BadRequest with validation problems if they exist, otherwise NotFound.</returns>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateEvent([FromRoute] int id, [FromBody] EventRequest request, Model model)
    {
        var user = Helpers.GetAuthenticatedUser(HttpContext);
        var eventToCheck = model.Events.Get(id);
        if (eventToCheck == null)
        {
            return NotFound();
        }
        else if (eventToCheck.OrganizerId != user.Id)
        {
            return Unauthorized();
        }

        var _event = request.ToEvent();
        var errors = _event.Validate();
        if (errors.Count > 0)
        {
            return ValidationProblem(new ValidationProblemDetails(errors));
        }

        _event.Id = id;
        _event.OrganizerId = user.Id;
        if (model.Events.Update(_event))
        {
            return NoContent();
        }

        return NotFound();
    }

    /// <summary>
    /// Delete an existing event. Requires authentication
    /// </summary>
    /// <param name="id">Event ID.</param>
    /// <returns>NoContent if succesfull, otherwise NotFound.</returns>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteEvent(int id, Model model)
    {
        var user = Helpers.GetAuthenticatedUser(HttpContext);
        var eventToCheck = model.Events.Get(id);
        if (eventToCheck == null)
        {
            return NotFound();
        }
        else if (eventToCheck.OrganizerId != user.Id)
        {
            return Unauthorized();
        }

        if (model.Events.Delete(id))
        {
            return NoContent();
        }

        return NotFound();
    }

    /// <summary>
    /// Get a list of users for event
    /// </summary>
    /// <param name="id">Event ID.</param>
    /// <returns>List of users for event if succesfull, otherwise NotFound.</returns>
    [HttpGet("{id}/users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<IEnumerable<UsersController.UserResponse>> GetUsersForEvent(int id, Model model)
    {
        if (model.Events.Get(id) == null)
        {
            return NotFound();
        }

        var users = model.Users.GetUsersForEvent(id);
        var userResponses = new List<UsersController.UserResponse>();
        foreach (var user in users)
        {
            userResponses.Add(new UsersController.UserResponse(user));
        }

        return userResponses;
    }

    /// <summary>
    /// Get events from pagined list
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>List of events</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<IEnumerable<EventResponse>> GetEvents([FromQuery] int page, [FromQuery] int pageSize, Model model)
    {
        if (page <= 0 || pageSize <= 0)
        {
            return BadRequest();
        }

        var events = model.Events.GetEvents(page, pageSize);

        var eventResponses = new List<EventResponse>();
        foreach (var _event in events)
        {
            var eventUser = model.Users.Get(_event.OrganizerId);
            if (eventUser == null)
            {
                return NotFound();
            }
            eventResponses.Add(new EventResponse(_event, eventUser));
        }

        return eventResponses;
    }
}
