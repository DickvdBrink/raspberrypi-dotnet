using System;
using System.Linq;
using System.Text;
using System.Threading;
using lib_i2c;

namespace i2c_test
{
    public class BME280
    {
        private class CalibrationData
        {
            public ushort digitalTemperature1;
            public short digitalTemperature2;
            public short digitalTemperature3;
            public ushort digitalPresure1;
            public short digitalPresure2;
            public short digitalPresure3;
            public short digitalPresure4;
            public short digitalPresure5;
            public short digitalPresure6;
            public short digitalPresure7;
            public short digitalPresure8;
            public short digitalPresure9;
            public byte digitalHumidity1;
            public short digitalHumidity2;
            public byte digitalHumidity3;
            public short digitalHumidity4;
            public short digitalHumidity5;
            public byte digitalHumidity6;
            public int tFine;
        }

        public const int DEFAULTADDRESS = 0x77;
        public const int CHIPID = 0x60;

        private I2CBus bus;
        private int address;

        private CalibrationData calibrationData;

        public BME280(I2CBus bus, int address = DEFAULTADDRESS)
        {
            this.bus = bus;
            this.address = address;

            bus.SetDevice(this.address);

            var chipId = new byte[1];
            bus.WriteData(new byte[] { 0xD0 });
            bus.ReadData(chipId);
            System.Console.WriteLine($"Chip id = {chipId[0]} Expected {CHIPID}");

            this.getCalibrationData();
            this.getHumidityCalibrationData();
        }

        public decimal ReadTemperature()
        {
            int temperature = 0;
            byte[] result = this.runCommand(0xF7, 8);

            result = this.runCommand(0xFA, 3);

            // Copy from bosch datasheet
            temperature = (result[0] << 12) + (result[1] << 4) + (result[2] >> 4);

            var var1 = ((((temperature >> 3) - (this.calibrationData.digitalTemperature1 <<1))) * (this.calibrationData.digitalTemperature2)) >> 11;
            var var2 = (((((temperature >> 4) - (this.calibrationData.digitalTemperature1)) * ((temperature>>4) - (this.calibrationData.digitalTemperature1))) >> 12) * (this.calibrationData.digitalTemperature3)) >> 14;
            var t_fine = var1 + var2;

            return ((t_fine * 5 + 128) >> 8) / 100;
        }

        private void getCalibrationData()
        {
            byte[] result = this.runCommand(0x88, 26);
            this.calibrationData = new CalibrationData();

            calibrationData.digitalTemperature1 = (ushort)convertBytes(result[1], result[0]);
            calibrationData.digitalTemperature2 = (short)convertBytes(result[3], result[2]);
            calibrationData.digitalTemperature3 = (short)convertBytes(result[5], result[4]);
            calibrationData.digitalPresure1 = (ushort)convertBytes(result[7], result[6]);
            calibrationData.digitalPresure2 = (short)convertBytes(result[9], result[8]);
            calibrationData.digitalPresure3 = (short)convertBytes(result[11], result[10]);
            calibrationData.digitalPresure4 = (short)convertBytes(result[13], result[12]);
            calibrationData.digitalPresure5 = (short)convertBytes(result[15], result[14]);
            calibrationData.digitalPresure6 = (short)convertBytes(result[17], result[16]);
            calibrationData.digitalPresure7 = (short)convertBytes(result[19], result[18]);
            calibrationData.digitalPresure8 = (short)convertBytes(result[21], result[20]);
            calibrationData.digitalPresure9 = (short)convertBytes(result[23], result[22]);
            calibrationData.digitalHumidity1 = result[25];
        }

        private void getHumidityCalibrationData()
        {
            byte[] result = this.runCommand(0xE1, 7);
            // TODO
        }

        private int convertBytes(byte msb, byte lsb)
        {
            return (((ushort)msb << 8) + (ushort)lsb);
        }

        private byte[] runCommand(byte command, int resultLength)
        {
            bus.SetDevice(this.address);
            bus.WriteData(new byte[] { command });

            if (resultLength > 0)
            {
                byte[] result = new byte[resultLength];
                bus.ReadData(result);
                return result;
            }
            return Array.Empty<byte>();
        }
    }
}