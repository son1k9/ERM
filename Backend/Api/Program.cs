using Api.Routes;
using Models;

namespace Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Services.AddProblemDetails();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var dbPath = builder.Configuration["DbDataSource"];
        if (dbPath == null)
        {
            Console.WriteLine("appsettings.json doesnt contain DbDataSource");
            return;
        }

        var factory = new SqliteConnectionFactory(dbPath);
        var model = new Model(factory);
        builder.Services.AddSingleton(model);

        var app = builder.Build();

        app.SetupRoutes();

        app.Run();
    }
}
