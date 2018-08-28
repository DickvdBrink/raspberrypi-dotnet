using System;
using System.Linq;
using System.Text;
using System.Threading;
using lib_i2c;

namespace i2c_test
{
    public class BME280
    {
        public const int DEFAULTADDRESS = 0x77;
        public const int CHIPID = 0x60;

        private I2CBus bus;
        private int address;

        public BME280(I2CBus bus, int address = DEFAULTADDRESS)
        {
            this.bus = bus;
            this.address = address;

            bus.SetDevice(this.address);

            var chipId = new byte[1];
            bus.WriteData(new byte[] { 0xD0 });
            bus.ReadData(chipId);
            System.Console.WriteLine($"Chip id = {chipId[0]} Expected {CHIPID}");

            // SET CONTROL
            bus.WriteData(new byte[] { 0xF4, 0x3F});
        }

        public decimal ReadTemperature()
        {
            this.runCommand();
            return 0 ;
        }

        private void runCommand()
        {
            bus.SetDevice(this.address);
            bus.WriteData(new byte[] { 0xFA });

            byte[] result = new byte[3];
            bus.ReadData(result);
            System.Console.WriteLine($"{result[0]} {result[1]} {result[2]}");
        }
    }
}