using System;
using System.Text.Json.Serialization;
using SimpleWeatherService.Helpers;

namespace SimpleWeatherService.Models
{
    public class CurrentWeatherReportDto
    {
        [JsonPropertyName("dt")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime DateTime { get; set; }
        [JsonPropertyName("sunrise")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime Sunrise { get; set; }
        [JsonPropertyName("sunset")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime Sunset { get; set; }
        [JsonPropertyName("temp")]
        public double Temp { get; set; }
        [JsonPropertyName("pressure")]
        public int Pressure { get; set; }
        [JsonPropertyName("feels_like")]
        public double FeelsLike { get; set; }
        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }
        [JsonPropertyName("dew_point")]
        public double DewPoint { get; set; }
        [JsonPropertyName("uvi")]
        public double Uvi { get; set; }
        [JsonPropertyName("clouds")]
        public int Clouds { get; set; }
        [JsonPropertyName("visibility")]
        public int Visibility { get; set; }
        [JsonPropertyName("wind_speed")]
        public double Windspeed { get; set; }
        [JsonPropertyName("wind_deg")]
        public int WindDeg { get; set; }
        [JsonPropertyName("wind_gust")]
        public double WindGust { get; set; }
    }
}
