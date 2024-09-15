namespace Weather.Models
{
    public class WeatherHour
    {
        public int Humidity { get; set; }
        public int Temperature { get; set; }
        public DateTime SaveHour { get; set; }
        public WeatherHour() { }
        public WeatherHour(int humidity, int temperature, DateTime saveHour)
        {
            Humidity = humidity;
            Temperature = temperature;
            SaveHour = saveHour;
        }
    }
}