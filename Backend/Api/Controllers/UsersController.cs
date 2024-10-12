using Api.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Api.Controllers;
[Route("/user")]
[ApiController]
public class UsersController : ControllerBase
{
    public class UserResponse(User user)
    {
        public int Id { get; set; } = user.Id;
        public string Login { get; set; } = user.Login;
    }

    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public User ToUser()
        {
            return new User
            {
                Email = Email,
                Login = Login,
                Password = Password
            };
        }
    }

    [HttpGet("{login}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<UserResponse> GetUser([FromRoute] string login, Model model)
    {
        var user = model.Users.GetByLogin(login);

        if (user == null)
        {
            return NotFound();
        }

        return new UserResponse(user);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<UserResponse> RegisterUser(RegisterRequest request, Model model)
    {
        var user = request.ToUser();

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

        return Created($"/user/{user.Id}", new UserResponse(user));
    }

    [HttpGet("{login}/events")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<IEnumerable<EventsController.EventResponse>> GetEventsForUser(string login, Model model)
    {
        var user = model.Users.GetByLogin(login);
        if (user == null)
        {
            return NotFound();
        }

        var events = model.Events.GetEventsForUser(user.Id);
        var eventResponses = new List<EventsController.EventResponse>();
        foreach (var _event in events)
        {
            eventResponses.Add(new EventsController.EventResponse(_event, user));
        }

        return eventResponses;
    }

    [HttpPost("add-event/{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult SubscribeToEvent([FromRoute] int id, Model model)
    {
        var user = Helpers.GetAuthorizedUser(HttpContext);

        var _event = model.Events.Get(id);
        if (_event == null)
        {
            return NotFound();
        }

        var error = model.Users.AddEventToUser(user.Id, _event.Id);
        if (error != null)
        {
            return BadRequest(error);
        }

        return NoContent();
    }
}