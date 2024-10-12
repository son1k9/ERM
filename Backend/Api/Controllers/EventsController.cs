using System.Net;
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
                Name = Name,
                Description = Description,
                Date = Date,
                Place = Place,
                Canceled = Canceled
            };
        }
    }


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

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<EventResponse> CreateEvent(EventRequest request, Model model)
    {
        var user = Helpers.GetAuthorizedUser(HttpContext);
        var _event = request.ToEvent();

        var errors = _event.Validate();
        if (errors.Count > 0)
        {
            return ValidationProblem(new ValidationProblemDetails(errors));
        }

        _event.OrganizerId = user.Id;
        //TODO: Handle invalid foreign key exception
        _event.Id = model.Events.Insert(_event);

        return Created($"/event/{_event.Id}", new EventResponse(_event, user));
    }

    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateEvent([FromRoute] int id, [FromBody] EventRequest request, Model model)
    {
        var user = Helpers.GetAuthorizedUser(HttpContext);
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
        //Handle invalid foreign key exception
        if (model.Events.Update(_event))
        {
            return NoContent();
        }

        return NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteEvent(int id, Model model)
    {
        var user = Helpers.GetAuthorizedUser(HttpContext);
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

    [HttpGet("{id}/users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<IEnumerable<UsersController.UserResponse>> GetUsersForEvent(int id, Model model)
    {
        if (model.Events.Get(id) == null)
        {
            return NotFound();
        }

        var list = model.Users.GetUsersForEvent(id);
        var dtosList = new List<UsersController.UserResponse>();
        foreach (var e in list)
        {
            dtosList.Add(new UsersController.UserResponse(e));
        }

        return dtosList;
    }
}
