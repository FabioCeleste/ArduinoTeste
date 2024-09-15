using Weather.Models;
using ErrorOr;
using Weather.Contracts.Pagination;

namespace Weather.Services.WeatherHours;

public interface IWeatherHourService
{
    Task<ErrorOr<WeatherHour>> CreateWeatherHourAsync(WeatherHour weatherHour);
    Task<ErrorOr<WeatherHour>> GetWeatherHourById(DateTime id);
    Task<ErrorOr<PagedResult<WeatherHour>>> GetAllWeatherHoursAsync(int pageNumber, int pageSize);
    Task<ErrorOr<Deleted>> DeleteWeatherHourAsync(DateTime id);
    Task<ErrorOr<WeatherHour>> GetLastCreatedWeatherHourAsync();
}
