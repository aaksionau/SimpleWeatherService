using KnowWeatherApp.Common.Repositories;
using SimpleWeatherService.Configurations;
using SimpleWeatherService.HostedServices;
using SimpleWeatherService.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IOpenWeatherService, OpenWeatherService>();
builder.Services.AddHostedService<OpenWeatherHostedService>();
builder.Services.Configure<OpenWeatherSettings>(builder.Configuration.GetSection("OpenWeatherSettings"));
builder.Services.AddMemoryCache();

builder.Services.AddHttpClient("WeatherAPI", options =>
{
    var openWeatherConfig = builder.Configuration.GetSection("OpenWeatherSettings").Get<OpenWeatherSettings>();
    if (openWeatherConfig == null) throw new ArgumentException("Weather API was not provided");

    options.BaseAddress = new Uri(uriString: openWeatherConfig.API);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
