
namespace Weather.Contracts.WeatherHour;

public record CreateWeatherHourRequest(
    int Humidity,
    int Temperature,
    DateTime SaveHour
    );
