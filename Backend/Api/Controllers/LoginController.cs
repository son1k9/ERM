using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Net;

namespace Api.Controllers;
[Route("/login")]
[ApiController]
public class LoginController : ControllerBase
{
    public class LoginData
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class Token
    {
        public string Value { get; set; } = string.Empty;
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<Token> Login(LoginData loginData, Model model)
    {
        var user = model.Users.GetByEmailAndPassword(loginData.Email, loginData.Password);
        if (user == null)
        {
            return Unauthorized();
        }

        var token = model.Tokens.New(user.Id, TimeSpan.FromDays(3));
        if (token == null)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        return new Token { Value = token.Text };
    }
}
