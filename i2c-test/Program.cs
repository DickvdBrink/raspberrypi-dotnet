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

            var baseLine = sensor.GetBaseline();
            Console.WriteLine($"Baseline: {baseLine.Raw_CO2} {baseLine.Raw_VOC}");

            //sensor.SetBaseline(0, 0); values are only available after 12 hours or something so no clue
            // what a valid baseline is

            // while (true)
            // {
            //     var result = sensor.MeasureAirQuality();
            //     Console.WriteLine($"{DateTime.Now}  {result.CO2_PartsPerMillion}");
            //     Thread.Sleep(1000);
            // }

            BME280 sensor2 = new BME280(bus, 0x76);
            while (true)
            {
                var temperature = sensor2.ReadTemperature();
                Console.WriteLine($"{DateTime.Now} Temperature: {temperature}");
                Thread.Sleep(1000);
            }
        }
    }
}
