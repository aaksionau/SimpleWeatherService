using System.Collections.Generic;
using System;
using System.Text.Json.Serialization;
using SimpleWeatherService.Helpers;

namespace SimpleWeatherService.Models
{
    public class DailyWeatherReportDto
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
        [JsonPropertyName("moonrise")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime MoonRise { get; set; }
        [JsonPropertyName("moonset")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime MoonSet { get; set; }
        [JsonPropertyName("pressure")]
        public int Pressure { get; set; }
        [JsonPropertyName("dew_point")]
        public double DewPoint { get; set; }
        [JsonPropertyName("wind_speed")]
        public double WindSpeed { get; set; }
        [JsonPropertyName("wind_gust")]
        public double WindGust { get; set; }
        [JsonPropertyName("temp")]
        public DailyTempDto Temp { get; set; }
        [JsonPropertyName("feels_like")]
        public DailyTempDto FeelsLike { get; set; }
        public IEnumerable<GeneralWeatherDto> Weather { get; set; } = new List<GeneralWeatherDto>();
        public int Clouds { get; set; }
        [JsonPropertyName("pop")]
        public double Precipitation { get; set; }
        public int Humidity { get; set; }
        public double Rain { get; set; }
        public double Uvi { get; set; }

    }
}


