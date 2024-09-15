using Weather.Models;
using ErrorOr;
using Weather.Data;
using Microsoft.EntityFrameworkCore;
using Weather.Contracts.Pagination;

namespace Weather.Services.WeatherHours
{
    public class WeatherHourService : IWeatherHourService
    {
        private readonly DataContext _dataContext;

        public WeatherHourService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ErrorOr<WeatherHour>> CreateWeatherHourAsync(WeatherHour weatherHour)
        {
            try
            {
                _dataContext.WeatherHours.Add(weatherHour);
                await _dataContext.SaveChangesAsync();
                return weatherHour;
            }
            catch (Exception ex)
            {
                return Error.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ErrorOr<WeatherHour>> GetWeatherHourById(DateTime id)
        {
            try
            {
                var weatherHour = await _dataContext.WeatherHours
                    .FirstOrDefaultAsync(w => w.SaveHour == id);

                if (weatherHour == null)
                {
                    return Error.NotFound("WeatherHour not found");
                }

                return weatherHour;
            }
            catch (Exception ex)
            {
                return Error.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ErrorOr<PagedResult<WeatherHour>>> GetAllWeatherHoursAsync(int pageNumber, int pageSize)
        {
            try
            {
                var pageSizeWithMaxLimit50 = pageSize;
                if (pageSizeWithMaxLimit50 > 50)
                {
                    pageSizeWithMaxLimit50 = 50;
                }

                var totalCount = await _dataContext.WeatherHours.CountAsync();

                var weatherHours = await _dataContext.WeatherHours
                    .OrderBy(w => w.SaveHour)
                    .Skip((pageNumber - 1) * pageSizeWithMaxLimit50)
                    .Take(pageSizeWithMaxLimit50)
                    .ToListAsync();

                if (weatherHours == null || weatherHours.Count == 0)
                {
                    return Error.NotFound("WeatherHours not found");
                }

                var pagedResult = new PagedResult<WeatherHour>(weatherHours, totalCount);

                return pagedResult;
            }
            catch (Exception ex)
            {
                return Error.Failure(description: ex.Message);
            }
        }

        public async Task<ErrorOr<Deleted>> DeleteWeatherHourAsync(DateTime id)
        {
            try
            {
                var weatherHour = await _dataContext.WeatherHours.FindAsync(id);
                if (weatherHour == null)
                {
                    return Error.NotFound("WeatherHour not found");
                }

                _dataContext.WeatherHours.Remove(weatherHour);
                await _dataContext.SaveChangesAsync();
                return new Deleted();
            }
            catch (Exception ex)
            {
                return Error.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ErrorOr<WeatherHour>> GetLastCreatedWeatherHourAsync()
        {
            try
            {
                var lastCreatedWeatherHour = await _dataContext.WeatherHours
                    .OrderByDescending(w => w.SaveHour)
                    .FirstOrDefaultAsync();

                if (lastCreatedWeatherHour is null)
                {
                    return Error.NotFound("No weather hours found.");
                }


                return lastCreatedWeatherHour;

            }
            catch (Exception ex)
            {
                return Error.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}

