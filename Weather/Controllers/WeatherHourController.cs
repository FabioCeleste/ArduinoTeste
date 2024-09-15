using Microsoft.AspNetCore.Mvc;
using Weather.Models;
using Weather.Services.WeatherHours;
using Weather.Contracts.Pagination;
using Weather.Contracts.WeatherHour;
using Weather.Services;

namespace WeatherApi.Controllers
{
    [ApiController]
    [Route("api/weatherhours")]
    public class WeatherHourController : ControllerBase
    {
        private readonly IWeatherHourService _weatherHourService;

        public WeatherHourController(IWeatherHourService weatherHourService)
        {
            _weatherHourService = weatherHourService;
        }

        [HttpGet]
        public ActionResult<WeatherHourResponse> GetLastCreatedWeatherHour()
        {
            var arduinoDataReader = new ArduinoDataReader();

            var weatherHour = arduinoDataReader.GetWeatherHour();

            if (weatherHour == null)
            {
                return StatusCode(500, "Error: Could not retrieve weather data from Arduino.");
            }


            var response = new WeatherHourResponse(
                weatherHour.Temperature,
                weatherHour.Humidity,
                weatherHour.SaveHour
            );

            return Ok(response);
        }

        [HttpGet("all")]
        public async Task<ActionResult<PagedResult<WeatherHourResponse>>> GetAllWeatherHours([FromQuery] GetAllWeatherHoursRequest query)
        {
            try
            {
                var result = await _weatherHourService.GetAllWeatherHoursAsync(query.Page ?? 1, query.Pagesize ?? 15);

                if (result.IsError)
                {
                    return BadRequest(new
                    {
                        error = result.Errors
                    });
                }

                var weatherHours = result.Value.Items.Select(weatherHour => new WeatherHourResponse(
                    weatherHour.Temperature,
                    weatherHour.Humidity,
                    weatherHour.SaveHour
                )).ToList();

                var pagedResult = new PagedResult<WeatherHourResponse>(weatherHours, result.Value.TotalCount);

                return Ok(pagedResult);
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WeatherHourResponse>> GetWeatherHourById(DateTime id)
        {
            try
            {
                var result = await _weatherHourService.GetWeatherHourById(id);

                if (result.IsError)
                {
                    return BadRequest(new
                    {
                        error = result.Errors
                    });
                }

                var weatherHour = result.Value;

                var response = new WeatherHourResponse(
                   weatherHour.Temperature,
                    weatherHour.Humidity,
                    weatherHour.SaveHour
                );

                return Ok(response);
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateWeatherHourRequest request)
        {
            try
            {
                DateTime now = DateTime.UtcNow;
                var weatherHour = new WeatherHour(
                request.Humidity, request.Temperature, now
                );

                var result = await _weatherHourService.CreateWeatherHourAsync(weatherHour);

                if (result.IsError)
                {
                    return BadRequest(new { error = result.Errors });
                }

                var created = result.Value;

                return new JsonResult(created);
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(DateTime id)
        {
            try
            {
                var result = await _weatherHourService.DeleteWeatherHourAsync(id);

                if (result.IsError)
                {
                    return BadRequest(new
                    {
                        error = result.Errors
                    });
                }

                return Ok(new { id = id });
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}
