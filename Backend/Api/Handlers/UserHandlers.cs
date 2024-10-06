using Api.Extentions;
using Models;

namespace Api.Handlers;

public static class UserHandlers
{
    public static IResult GetUser(int id, ILogger<Program> logger, Model model, HttpContext httpContext)
    {
        var user = model.Users.Get(id);

        if (user == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(new
        {
            user.Email,
            user.Login,
            user.Phone
        });
    }

    public static IResult RegisterUser(User user, ILogger<Program> logger, Model model, HttpContext httpContext)
    {
        var errors = user.Validate();
        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        if (model.Users.Insert(user) > 0)
        {
            return Results.Ok();
        }

        return Results.StatusCode(500);
    }
}
