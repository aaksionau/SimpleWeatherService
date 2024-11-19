using Azure.Communication.Sms;
using SimpleWeatherService.Interfaces;
using System.Collections.Generic;
using Telegram.Bot;

namespace SimpleWeatherService.HostedServices
{
    public class OpenWeatherHostedService : BackgroundService
    {
        private readonly ILogger<OpenWeatherHostedService> logger;
        private readonly IOpenWeatherService openWeatherService;
        private readonly IConfiguration configuration;
        private double lat = 44.8297118;
        private double lon = -92.9151719;

        public OpenWeatherHostedService(
            ILogger<OpenWeatherHostedService> logger,
            IOpenWeatherService openWeatherService, 
            IConfiguration configuration)
        {
            this.logger = logger;
            this.openWeatherService = openWeatherService;
            this.configuration = configuration;
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
            var deltas = new Dictionary<string, ValueTuple<int, int>>()
            {
                { "morning", (5, 8) },
                { "afternoon", (12, 14) },
                { "evening", (17, 19) },
            };

            var weatherConditions = new List<string>()
            {
                "Thunderstorm",
                "Rain",
                "Snow",
                "Fog",
                "Tornado"
            };
            var next24Hours = report.Hourly.Take(24).ToList();

            foreach (var partOfDay in deltas.Keys)
            {
                var partOfDayReports = next24Hours.Where(x => x.DateTime.Hour >= deltas[partOfDay].Item1 && x.DateTime.Hour <= deltas[partOfDay].Item2);
                if (!partOfDayReports.Any()) continue;

                var mainWeatherConditions = partOfDayReports
                                                            .Where(x => x.Weather.FirstOrDefault() != null)
                                                            .Select(x => x.Weather.FirstOrDefault()!.Main);

                var intersect = mainWeatherConditions.Intersect(weatherConditions);
                var date = partOfDayReports.FirstOrDefault()!.DateTime.Date;
                if (intersect.Any())
                {
                    var weatherCondition = string.Join(", ", intersect);
                    await SendMessage($"[{date.ToString("M")}] In {partOfDay} there will be {weatherCondition} between {deltas[partOfDay].Item1} and {deltas[partOfDay].Item2} o'clock");
                }
            }
        }

        private async Task SendMessage(string message)
        {
            var telegramToken = this.configuration.GetValue<string>("TelegramToken");

            if (string.IsNullOrEmpty(telegramToken)) throw new ArgumentNullException("Telegram token was not set");

            var bot = new TelegramBotClient(telegramToken);

            await bot.SendMessage("388703389", message);
        }
    }
}
