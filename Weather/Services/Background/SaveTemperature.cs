using Weather.Services.WeatherHours;

namespace Weather.Services.Backgrounds;
public class TemperatureUpdateService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private Timer _timer = null!;

    public TemperatureUpdateService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(GetTemperatureFromArduino, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
        return Task.CompletedTask;
    }

    private async void GetTemperatureFromArduino(object? state)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var weatherHourService = scope.ServiceProvider.GetRequiredService<IWeatherHourService>();
            var arduinoDataReader = new ArduinoDataReader();

            var weatherHour = arduinoDataReader.GetWeatherHour();

            if (weatherHour != null)
            {
                await weatherHourService.CreateWeatherHourAsync(weatherHour);
            }
        }
    }

    public override void Dispose()
    {
        _timer?.Dispose();
        base.Dispose();
    }
}

public class SensorData
{
    public int Temperature { get; set; }
    public int Humidity { get; set; }
}

