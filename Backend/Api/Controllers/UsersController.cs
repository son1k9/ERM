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

    public class AuthorizedUserResponse(User user)
    {
        public int Id { get; set; } = user.Id;
        public string Email { get; set; } = user.Email;
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
                Email = Email.Trim(),
                Login = Login.Trim(),
                Password = Password
            };
        }
    }

    /// <summary>
    /// Get user by login
    /// </summary>
    /// <param name="login">User's login.</param>
    /// <returns>User information if found, otherwise NotFound.</returns>
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

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">Users information.</param>
    /// <returns>Created user inforamtion if succesfull, otherwise BadRequset with validation problems.</returns>
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

    /// <summary>
    /// Get a list of events of a user
    /// </summary>
    /// <param name="login">User's login.</param>
    /// <returns>A list of events of a user if succesfull, otherwise NotFound.</returns>
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
            var eventUser = model.Users.Get(_event.OrganizerId);
            if (eventUser == null)
            {
                return NotFound();
            }
            eventResponses.Add(new EventsController.EventResponse(_event, eventUser));
        }

        return eventResponses;
    }

    /// <summary>
    /// Subscribe authenticated user for an event. Requires authentication
    /// </summary>
    /// <param name="id">Event ID.</param>
    /// <returns>NoContent if succesfull, BadRequest with an error if occurs, otherwise NotFound.</returns>
    [HttpPatch("add-event/{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult SubscribeToEvent([FromRoute] int id, Model model)
    {
        var user = Helpers.GetAuthenticatedUser(HttpContext);

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

    /// <summary>
    /// Unsubscribe authenticated user from an event. Requires authentication
    /// </summary>
    /// <param name="id">Event ID.</param>
    /// <returns>NoContent if succesfull, BadRequest with an error if occurs, otherwise NotFound.</returns>
    [HttpPatch("remove-event/{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UnsubscribeFromEvent([FromRoute] int id, Model model)
    {
        var user = Helpers.GetAuthenticatedUser(HttpContext);

        var _event = model.Events.Get(id);
        if (_event == null)
        {
            return NotFound();
        }

        var error = model.Users.RemoveEventFromUser(user.Id, _event.Id);
        if (error != null)
        {
            return BadRequest(error);
        }

        return NoContent();
    }

    /// <summary>
    /// Get authenticated user. Requires authentication
    /// </summary>
    /// <returns>User information.</returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<AuthorizedUserResponse> GetUserForToken()
    {
        var user = Helpers.GetAuthenticatedUser(HttpContext);

        return new AuthorizedUserResponse(user);
    }
}