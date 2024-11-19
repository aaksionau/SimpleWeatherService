using System.Collections.Generic;
using System;
using System.Text.Json.Serialization;
using SimpleWeatherService.Helpers;

namespace SimpleWeatherService.Models
{
    public class HourlyWeatherReportDto
    {
        [JsonPropertyName("dt")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime DateTime { get; set; }
        public double Temp { get; set; }
        [JsonPropertyName("feels_like")]
        public double FeelsLike { get; set; }
        public int Humidity { get; set; }
        public int Pressure { get; set; }
        [JsonPropertyName("dew_point")]
        public double DewPoint { get; set; }
        public double Uvi { get; set; }
        public int Clouds { get; set; }
        public int Visibility { get; set; }
        [JsonPropertyName("wind_speed")]
        public double Windspeed { get; set; }
        [JsonPropertyName("wind_deg")]
        public int WindDeg { get; set; }
        [JsonPropertyName("wind_gust")]
        public double WindGust { get; set; }
        [JsonPropertyName("pop")]
        public double Precipitation { get; set; }
        public IEnumerable<GeneralWeatherDto> Weather { get; set; }
    }
}
