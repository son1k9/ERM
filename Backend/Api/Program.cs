using Api.Middleware;
using Models;

namespace Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var dbPath = builder.Configuration["DbDataSource"];
        if (dbPath == null)
        {
            Console.WriteLine("appsettings.json doesnt contain DbDataSource");
            return;
        }

        var factory = new SqliteConnectionFactory(dbPath);
        var model = new Model(factory);
        builder.Services.AddSingleton(model);

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
        {
            
        });

        builder.Services.AddProblemDetails();
        builder.Services.AddAuthentication().AddScheme<CustomBearerTokenAuthSchemeOptions,
        CustomBearerTokenAuthSchemeHandler>("BearerTokens", options =>
        {
            options.Model = model;
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }

        app.UseRequestLogger();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
