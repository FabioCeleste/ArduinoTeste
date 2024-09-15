using System.IO.Ports;
using System.Runtime.InteropServices;
using Weather.Models;

namespace Weather.Services;
public class ArduinoDataReader
{
    private readonly string _portName;
    private readonly int _baudRate;

    public ArduinoDataReader()
    {
        _portName = GetSerialPortName();
        _baudRate = 9600;
    }

    public WeatherHour? GetWeatherHour()
    {
        if (string.IsNullOrEmpty(_portName))
        {
            Console.WriteLine("Could not find a valid serial port for your system.");
            return null;
        }

        using (SerialPort serialPort = new SerialPort(_portName, _baudRate))
        {
            try
            {
                serialPort.Open();
                serialPort.ReadTimeout = 5000;

                try
                {
                    string data = serialPort.ReadLine();
                    string[] dataParts = data.Split(',');

                    if (dataParts.Length == 2)
                    {
                        var sensorData = new SensorData
                        {
                            Temperature = int.Parse(dataParts[0]),
                            Humidity = int.Parse(dataParts[1])
                        };

                        var timestamp = GetRoundedTimestamp(DateTime.Now);
                        Console.WriteLine($"[{timestamp}] Temperature: {sensorData.Temperature} °C, Humidity: {sensorData.Humidity} %");

                        var weatherHour = new WeatherHour(
                            sensorData.Humidity,
                            sensorData.Temperature,
                            timestamp
                        );

                        return weatherHour;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading data: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        return null;
    }

    private static string GetSerialPortName()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return "COM3";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return "/dev/ttyUSB0";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "/dev/tty.usbmodemXXXX";
        }

        return "";
    }

    private static DateTime GetRoundedTimestamp(DateTime dateTime)
    {
        int minutes = dateTime.Minute;
        int seconds = dateTime.Second;

        if (seconds >= 30)
        {
            minutes++;
        }

        var roundedTime = new DateTime(
            dateTime.Year,
            dateTime.Month,
            dateTime.Day,
            dateTime.Hour,
            minutes,
            0,
            DateTimeKind.Utc
        );

        return roundedTime;
    }
}

public class SensorData
{
    public int Temperature { get; set; }
    public int Humidity { get; set; }
}
