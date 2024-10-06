using Api.Extentions;
using Models;

namespace Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var factory = new SqliteConnectionFactory(Directory.GetCurrentDirectory() + "\\Db\\events.db");
        var model = new Model(factory);
        builder.Services.AddSingleton(model);

        var app = builder.Build();

        app.SetupRoutes();

        app.Run();
    }
}
