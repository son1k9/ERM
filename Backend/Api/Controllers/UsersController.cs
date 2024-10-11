using Api.DTOs;
using Api.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Net;

namespace Api.Controllers;
[Route("/user")]
[ApiController]
public class UsersController : ControllerBase
{
    [HttpGet("{login}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<UserDTO> GetUser([FromRoute] string login, Model model)
    {
        var user = model.Users.GetByLogin(login);

        if (user == null)
        {
            return NotFound();
        }

        return new UserDTO(user);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<UserDTO> RegisterUser(User user, Model model)
    {
        var errors = user.Validate();
        if (errors.Count > 0)
        {
            return ValidationProblem(new ValidationProblemDetails(errors));
        }

        var result = model.Users.Insert(user);
        if (result.Error == UserError.DuplicateEmail)
        {
            errors.Add("Email", [UserError.DuplicateEmail]);
            return ValidationProblem(new ValidationProblemDetails(errors));
        }
        if (result.Error == UserError.DuplicateLogin)
        {
            errors.Add("Login", [UserError.DuplicateLogin]);
            return ValidationProblem(new ValidationProblemDetails(errors));
        }
        user.Id = result.Value;

        return Created($"/user/{user.Id}", new UserDTO(user));
    }

    [HttpGet("{login}/events")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<IEnumerable<EventDTO>> GetEventsForUser(string login, Model model)
    {
        var user = model.Users.GetByLogin(login);
        if (user == null)
        {
            return NotFound();
        }

        var userDTO = new UserDTO(user);
        var events = model.Events.GetEventsForUser(user.Id);
        var eventDTOs = new List<EventDTO>();
        foreach (var _event in events)
        {
            eventDTOs.Add(new EventDTO(_event, userDTO));
        }

        return eventDTOs;
    }

    [HttpPost("add-event")]
    public IActionResult SubscribeToEvent([FromBody] int eventId, Model model)
    {
        return NotFound();
    }
}