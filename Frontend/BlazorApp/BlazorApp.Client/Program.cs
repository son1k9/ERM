using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

await builder.Build().RunAsync();

builder.Services.AddScoped(sp =>
	new HttpClient
	{
		BaseAddress = new Uri(builder.Configuration["FrontendUrl"] ?? "https://localhost:7040")
	});