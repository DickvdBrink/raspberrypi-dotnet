using System;
using System.Threading;
using lib_i2c;

namespace i2c_test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            I2CBus bus = new I2CBus();
            bus.Open("/dev/i2c-1");
            bus.SetDevice(0x58);

            SGP30 sensor = new SGP30(bus);
            string str = sensor.GetSerialNumber();
            Console.WriteLine($"SGP30 Serial number: {str}");
            sensor.InitAirQuality();
            // while(true)
            // {
            //     var result = sensor.MeasureAirQuality();
            //     Console.WriteLine($"{DateTime.Now}  {result.CO2_PerMinute}");
            //     Thread.Sleep(1000);
            // }

            BMP280 sensor2 = new BMP280(bus, 0x76);
            while(true)
            {
                var temperature = sensor2.ReadTemperature();
                Console.WriteLine($"{DateTime.Now} Temperature: {temperature}");
                Thread.Sleep(1000);
            }
        }
    }
}
