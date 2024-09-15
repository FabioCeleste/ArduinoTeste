namespace Weather.Contracts.WeatherHour;

public record WeatherHourResponse(
        int Humidity,
        int Temperature,
        DateTime SaveHour
    );
