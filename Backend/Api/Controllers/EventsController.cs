using Api.DTOs;
using Api.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Api.Controllers;
[Route("/event")]
[ApiController]
public class EventsController : ControllerBase
{
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<EventDTO> GetEvent(int id, Model model)
    {
        var _event = model.Events.Get(id);

        if (_event == null)
        {
            return NotFound();
        }

        var user = model.Users.Get(_event.OraganizerId);
        if (user == null)
        {
            return NotFound();
        }
        var userDTO = new UserDTO(user);

        return new EventDTO(_event, userDTO);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<EventDTO> CreateEvent(Event _event, Model model)
    {
        var errors = _event.Validate();
        if (errors.Count > 0)
        {
            return ValidationProblem(new ValidationProblemDetails(errors));
        }

        //Handle invalid foreign key exception
        _event.Id = model.Events.Insert(_event);

        var user = model.Users.Get(_event.OraganizerId);
        var userDTO = new UserDTO(user!);

        return Created($"/event/{_event.Id}", new EventDTO(_event, userDTO));
    }

    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateEvent(int id, Event _event, Model model)
    {
        var errors = _event.Validate();
        if (errors.Count > 0)
        {
            return ValidationProblem(new ValidationProblemDetails(errors));
        }

        _event.Id = id;
        //Handle invalid foreign key exception
        if (model.Events.Update(_event))
        {
            return NoContent();
        }

        return NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteEvent(int id, Model model)
    {
        if (model.Events.Delete(id))
        {
            return NoContent();
        }

        return NotFound();
    }

    [HttpGet("{id}/users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<IEnumerable<UserDTO>> GetUsersForEvent(int id, Model model)
    {
        if (model.Events.Get(id) == null)
        {
            return NotFound();
        }

        var list = model.Users.GetUsersForEvent(id);
        var dtosList = new List<UserDTO>();
        foreach (var e in list)
        {
            dtosList.Add(new UserDTO(e));
        }

        return dtosList;
    }
}
