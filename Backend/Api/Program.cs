
using Models;

namespace Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet("/weatherforecast", (HttpContext httpContext) =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = summaries[Random.Shared.Next(summaries.Length)]
                })
                .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast")
        .WithOpenApi();

        
        var factory = new SqliteConnectionFactory(Directory.GetCurrentDirectory() + "\\Db\\events.db");
        var models = new Model(factory);

        app.MapGet("/user", (HttpContext httpContext) =>
        {
            var idStr = httpContext.Request.Query["id"];
            if (int.TryParse(idStr, out int id)) 
            {
                var user = models.Users.Get(id);
                if (user != null)
                {
                    return new
                    {
                        Email = user.Email,
                        Login = user.Login,
                        Phone = user.Phone
                    };
                }
            }
            return null;
        });

        app.MapGet("/userInsert", (HttpContext httpContext) =>
        {
            models.Users.Insert(new User
            {
                Email = "test@test.test",
                Login = "test_login",
                Phone = "89209004534",
                Password = "password"
            });
        });

        app.Run();
    }
}
