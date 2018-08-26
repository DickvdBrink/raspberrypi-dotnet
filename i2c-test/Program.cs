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

            I2CBus b = new I2CBus();
            b.Open("/dev/i2c-1");
            b.SetDevice(0x58);

            SGP30 sensor = new SGP30(b);
            string str = sensor.GetSerialNumber();
            Console.WriteLine($"Serial number: {str}");

            sensor.InitAirQuality();

            while(true)
            {
                var result = sensor.MeasureAirQuality();
                Console.WriteLine($"{DateTime.Now}  {result.CO2_PerMinute}");
                Thread.Sleep(1000);
            }
        }
    }
}
