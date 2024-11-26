using Microsoft.Extensions.Caching.Memory;
using SimpleWeatherService.Interfaces;
using SimpleWeatherService.Models;
using Telegram.Bot;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SimpleWeatherService.HostedServices
{
    public class OpenWeatherHostedService : BackgroundService
    {
        private readonly ILogger<OpenWeatherHostedService> logger;
        private readonly IOpenWeatherService openWeatherService;
        private readonly IConfiguration configuration;
        private readonly IMemoryCache memoryCache;
        private List<string> weatherConditions;
        private Dictionary<string, ValueTuple<int, int>> tempDeltas;
        private List<int> lowTemps;
        private List<int> highTemps;

        private double lat = 44.8297118;
        private double lon = -92.9151719;

        public OpenWeatherHostedService(
            ILogger<OpenWeatherHostedService> logger,
            IOpenWeatherService openWeatherService,
            IConfiguration configuration,
            IMemoryCache memoryCache)
        {
            this.logger = logger;
            this.openWeatherService = openWeatherService;
            this.configuration = configuration;
            this.memoryCache = memoryCache;

            this.tempDeltas = new Dictionary<string, ValueTuple<int, int>>()
            {
                { "morning", (6, 8) },
                { "afternoon", (12, 14) },
                { "evening", (17, 19) },
            };


            this.weatherConditions = new List<string>()
            {
                "Thunderstorm",
                "Rain",
                "Snow",
                "Fog",
                "Tornado"
            };

            this.lowTemps = new List<int>()
            {
                2,
                12,
                22,
                32
            };

            this.highTemps = new List<int>()
            {
                62,
                72,
                82,
                92,
                102
            };
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("OpenWeather HostedService running.");

            await this.CheckWeatherAsync();

            using PeriodicTimer timer = new(TimeSpan.FromHours(6));

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await this.CheckWeatherAsync();
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("OpenWeather HostedService is stopping.");
            }
        }

        private async Task CheckWeatherAsync()
        {
            var report = await this.openWeatherService.GetWeatherForCoordinatesAsync(lat, lon);
            report.Hourly.ForEach(hourlyReport => hourlyReport.DateTime = hourlyReport.DateTime.AddSeconds(report.TimeZoneOffset));

            var next24Hours = report.Hourly.Take(24).ToList();

            foreach (var partOfDay in this.tempDeltas.Keys)
            {
                var partOfDayReports = next24Hours.Where(
                    x => x.DateTime.Hour >= this.tempDeltas[partOfDay].Item1
                    && x.DateTime.Hour <= this.tempDeltas[partOfDay].Item2);

                if (!partOfDayReports.Any())
                {
                    this.logger.LogInformation($"There is no data for requested hours.");
                    return;
                }

                await this.NotifyAboutMainWeatherConditionsAsync(partOfDay, partOfDayReports);
                await this.NotifyAboutTemperatureAsync(partOfDay, partOfDayReports);
            }
        }

        private async Task NotifyAboutTemperatureAsync(string partOfDay, IEnumerable<HourlyWeatherReportDto> partOfDayReports)
        {
            var partOfDayAvgTemp = partOfDayReports.Average(x => x.Temp);
            var lowerTemps = lowTemps.Where(x => x <= partOfDayAvgTemp);

            var lowTemp = lowTemps.Except(lowerTemps);
            var date = partOfDayReports.FirstOrDefault()!.DateTime.Date;

            var key = $"{date.ToString("M")}-{this.tempDeltas[partOfDay].Item1}-{this.tempDeltas[partOfDay].Item2}-temp";
            var day = date.ToString("d") == DateTime.Now.Date.ToString("d") ? "Today" : "Tomorrow";
            if (lowTemp.Any() && !this.memoryCache.TryGetValue(key, out bool _))
            {
                string message = $"{day} in the {partOfDay} temperature will be around {partOfDayAvgTemp.ToString("0.0")}°F";
                await SendMessage(message);
                this.memoryCache.Set(key, true);
            }

            var higherTemps = highTemps.Where(x => x >= partOfDayAvgTemp);
            var highTemp = highTemps.Except(highTemps);

            if (highTemp.Any() && !this.memoryCache.TryGetValue(key, out bool _))
            {
                string message = $"{day} in the {partOfDay} temperature will be around {partOfDayAvgTemp.ToString("0.0")}°F";
                await SendMessage(message);
                this.memoryCache.Set(key, true);
            }
        }

        private async Task NotifyAboutMainWeatherConditionsAsync(string partOfDay, IEnumerable<HourlyWeatherReportDto> partOfDayReports)
        {
            var mainWeatherConditions = partOfDayReports
                                                        .Where(x => x.Weather.FirstOrDefault() != null)
                                                        .Select(x => x.Weather.FirstOrDefault()!.Main);

            var intersect = mainWeatherConditions.Intersect(this.weatherConditions);
            var date = partOfDayReports.FirstOrDefault()!.DateTime.Date;
            if (!intersect.Any())
            {
                this.logger.LogInformation($"For {partOfDay} of {date.ToString("M")} there was not special weather conditions.");
                return;
            }

            var weatherCondition = string.Join(", ", intersect).ToLower();
            var day = date.ToString("d") == DateTime.Now.Date.ToString("d") ? "Today" : "Tomorrow";
            var key = $"{date.ToString("M")}-{this.tempDeltas[partOfDay].Item1}-{this.tempDeltas[partOfDay].Item2}-condition";
            if (!this.memoryCache.TryGetValue(key, out bool _))
            {
                string message = $"{day} in the {partOfDay} there will be {weatherCondition}";
                await SendMessage(message);
                this.memoryCache.Set(key, true);
            }
        }

        private async Task SendMessage(string message)
        {
            var telegramToken = this.configuration.GetValue<string>("TelegramToken");

            if (string.IsNullOrEmpty(telegramToken)) throw new ArgumentNullException("Telegram token was not set");

            var bot = new TelegramBotClient(telegramToken);

            await bot.SendMessage("-4568320372", message);
            this.logger.LogInformation($"Message was sent: {message}");
        }
    }
}
