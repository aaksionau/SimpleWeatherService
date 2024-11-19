using System.Text.Json.Serialization;

namespace SimpleWeatherService.Models
{
    public class WeatherReportDto
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string TimeZone { get; set; }
        [JsonPropertyName("timezone_offset")]
        public int TimeZoneOffset { get; set; }
        public CurrentWeatherReportDto Current { get; set; } = new CurrentWeatherReportDto();
        public List<HourlyWeatherReportDto> Hourly { get; set; } = new List<HourlyWeatherReportDto>();
        public List<DailyWeatherReportDto> Daily { get; set; } = new List<DailyWeatherReportDto>();
        public List<WeatherAlertDto> Alerts { get; set; } = new List<WeatherAlertDto>();

        public void AdjustTime()
        {
            this.Hourly.ForEach(report =>
            {
                report.DateTime = report.DateTime.AddSeconds(this.TimeZoneOffset);
            });
        }
    }
}
