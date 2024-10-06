using Api.Handlers;
using Api.Middleware;

namespace Api.Extentions;

public static class WebApplicationExtention
{
    public static void SetupRoutes(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(
            async context => await Results.Problem().ExecuteAsync(context)));

        app.UseStatusCodePages(async statusCodeContext => await Results.Problem(
            statusCode: statusCodeContext.HttpContext.Response.StatusCode)
            .ExecuteAsync(statusCodeContext.HttpContext));

        app.UseRequestLogger();
        app.UseHttpsRedirection();

        app.MapGet("/user/{id}", UserHandlers.GetUser);

        app.MapPost("/user/create", UserHandlers.RegisterUser);
    }
}
