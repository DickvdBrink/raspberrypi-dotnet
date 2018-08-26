using System;
using System.Linq;
using System.Text;
using System.Threading;
using lib_i2c;

namespace i2c_test
{
    public class SGP30
    {
        public const int DEFAULTADDRESS = 0x58;
        private const int CRC_INIT = 0xff;
        private const int CRC_POLYNOMIAL = 0x31;

        private I2CBus bus;
        private int address;

        struct SGPCommand
        {
            public byte[] OpCode;
            public int ResponseLength;
            public int ResponseTimeMs;
            public int ParameterLength;
        }

        public SGP30(I2CBus bus, int address = DEFAULTADDRESS)
        {
            this.bus = bus;
            this.address = address;
        }

        public string GetSerialNumber()
        {
            SGPCommand cmd = new SGPCommand()
            {
                OpCode = new byte[] { 0x36, 0x82 },
                ResponseLength = 9,
                ResponseTimeMs = 1,
                ParameterLength = 0
            };
            var result = this.runCommand(cmd);
            return string.Join(" ", result.Select(x => string.Format("0x{0:X4}", x)));
        }

        public void GetFeatureSetVersion()
        {

        }

        public void InitAirQuality()
        {
            SGPCommand cmd = new SGPCommand()
            {
                OpCode = new byte[] { 0x20, 0x08 },
                ResponseLength = 0,
                ResponseTimeMs = 10,
                ParameterLength = 0
            };
            var result = this.runCommand(cmd);
        }

        public (int CO2_PerMinute, int VOC_PerMinute) MeasureAirQuality()
        {
            SGPCommand cmd = new SGPCommand()
            {
                OpCode = new byte[] { 0x20, 0x08 },
                ResponseLength = 6,
                ResponseTimeMs = 12,
                ParameterLength = 0
            };
            var result = this.runCommand(cmd);
            return (CO2_PerMinute: result[0], VOC_PerMinute: result[1]);
        }

        public void GetBaseline()
        {

        }

        public void SetBaseline()
        {

        }

        public void SetHumidity()
        {

        }

        public void MeasureRawSignals()
        {

        }

        private short[] runCommand(SGPCommand cmd, byte[] parameters = null)
        {
            var bytesToWrite = new byte[cmd.OpCode.Length + cmd.ParameterLength];
            if (cmd.ParameterLength > 0 && parameters?.Length != cmd.ParameterLength)
            {
                throw new ArgumentException(nameof(parameters));
            }

            Array.Copy(cmd.OpCode, 0, bytesToWrite, 0, cmd.OpCode.Length);
            if (parameters != null)
            {
                Array.Copy(parameters, 0, bytesToWrite, cmd.OpCode.Length, cmd.ParameterLength);
            }
            var argBytes = bytesToWrite.AsSpan(1);
            bus.WriteI2cBlockData(bytesToWrite[0], argBytes.ToArray());
            Thread.Sleep(cmd.ResponseTimeMs);
            if (cmd.ResponseLength > 0)
            {
                var result = new byte[cmd.ResponseLength];
                bus.ReadI2cBlockData(0, result);
                short[] serial = ReadCheckSummedWords(result);
                return serial;
            }
            return Array.Empty<short>();
        }

        private short[] ReadCheckSummedWords(byte[] data)
        {
            var span = data.AsSpan();
            var length = data.Length / 3;
            var result = new short[length];
            foreach (var i in Enumerable.Range(0, length))
            {
                result[i] = ReadCheckSummedWord(span.Slice(i * 3, 3));
            }
            return result;
        }

        private short ReadCheckSummedWord(Span<byte> data)
        {
            var words = data.Slice(0, 2);
            var crcWords = CRC(words);
            if (crcWords != data[2])
            {
                throw new Exception($"Invalid CRC. Got {crcWords}, Expected: {data[2]}");
            }
            return (short)(words[0] << 8 | words[1]);
        }

        private byte CRC(Span<byte> data)
        {
            byte crc = CRC_INIT;
            for (int i = 0; i < data.Length; i++)
            {
                byte b = data[i];
                crc ^= b;
                foreach (var _ in Enumerable.Range(0, 8))
                {
                    if ((crc & 0x80) != 0)
                    {
                        crc = (byte)((crc << 1) ^ CRC_POLYNOMIAL);
                    }
                    else
                    {
                        crc <<= 1;
                    }
                }
            }
            return (byte)(crc & 0xff);
        }
    }
}
