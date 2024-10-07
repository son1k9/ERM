using Api.Validation;
using Microsoft.AspNetCore.Http.HttpResults;
using Models;

namespace Api.Handlers;

public static class UserHandlers
{
    public static IResult GetUser(int id, Model model, HttpContext httpContext)
    {
        var user = model.Users.Get(id);

        if (user == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new
        {
            user.Email,
            user.Login,
        });
    }

    public static IResult RegisterUser(User user, Model model, HttpContext httpContext)
    {
        var errors = user.Validate();
        if (errors.Count > 0)
        {
            return TypedResults.ValidationProblem(errors);
        }

        //TODO: Add exception type for same email or login and include this information in the response
        user.Id = model.Users.Insert(user);
        return TypedResults.Created($"/user/{user.Id}", new
        {
            user.Email,
            user.Login,
        });
    }
}
