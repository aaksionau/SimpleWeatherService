using SimpleWeatherService.Models;

namespace SimpleWeatherService.Interfaces
{
    public interface IOpenWeatherService
    {
        Task<WeatherReportDto> GetWeatherForCoordinatesAsync(double lat, double lon);
    }
}
