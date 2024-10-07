using Api.Handlers;
using Api.Middleware;

namespace Api.Routes;

public static class WebApplicationExtention
{
    public static void SetupRoutes(this WebApplication app)
    {
        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }

        app.UseRequestLogger();
        app.UseHttpsRedirection();

        var users = app.MapGroup("/user");
        users.MapGet("/{id}", UserHandlers.GetUser);
        users.MapPost("/", UserHandlers.RegisterUser);
    }
}
