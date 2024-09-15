namespace Weather.Contracts.WeatherHour;

public record GetAllWeatherHoursRequest(
 int? Page,
 int? Pagesize
);