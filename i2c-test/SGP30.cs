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
        private const int FEATURE_SET_BITMASK = 0b_0000_0000_1110_0000;

        private const int WORD_LENGTH = 2;
        private const int CRC_LENGTH = 1;

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
                ResponseLength = 3,
                ResponseTimeMs = 1,
                ParameterLength = 0
            };
            var result = this.runCommand(cmd);
            return string.Join(" ", result.Select(x => string.Format("0x{0:X4}", x)));
        }

        public int GetFeatureSetVersion()
        {
            SGPCommand cmd = new SGPCommand()
            {
                OpCode = new byte[] { 0x20, 0x2f },
                ResponseLength = 1,
                ResponseTimeMs = 2,
                ParameterLength = 0
            };
            var result = this.runCommand(cmd);
            return result[0];
        }

        public void InitAirQuality()
        {
            SGPCommand cmd = new SGPCommand()
            {
                OpCode = new byte[] { 0x20, 0x03 },
                ResponseLength = 0,
                ResponseTimeMs = 10,
                ParameterLength = 0
            };
            var result = this.runCommand(cmd);
        }

        public (ushort CO2_PartsPerMillion, ushort VOC_PartsPerMillion) MeasureAirQuality()
        {
            SGPCommand cmd = new SGPCommand()
            {
                OpCode = new byte[] { 0x20, 0x08 },
                ResponseLength = 2,
                ResponseTimeMs = 12,
                ParameterLength = 0
            };
            var result = this.runCommand(cmd);
            return (CO2_PartsPerMillion: result[0], VOC_PartsPerMillion: result[1]);
        }

        public (ushort Raw_CO2, ushort Raw_VOC) GetBaseline()
        {
            SGPCommand cmd = new SGPCommand()
            {
                OpCode = new byte[] { 0x20, 0x15 },
                ResponseLength = 2,
                ResponseTimeMs = 10,
                ParameterLength = 0
            };
            var result = this.runCommand(cmd);
            return (Raw_CO2: result[0], Raw_VOC: result[1]);
        }

        public void SetBaseline(ushort a, ushort b)
        {
            SGPCommand cmd = new SGPCommand()
            {
                OpCode = new byte[] { 0x20, 0x1e },
                ResponseLength = 0,
                ResponseTimeMs = 10,
                ParameterLength = 2
            };
            this.runCommand(cmd, new ushort[] {a, b});
        }

        public void SetHumidity()
        {

        }

        public void MeasureRawSignals()
        {

        }

        private ushort[] runCommand(SGPCommand cmd, ushort[] parameters = null)
        {
            bus.SetDevice(this.address);

            var bytesToWrite = new byte[cmd.OpCode.Length + (cmd.ParameterLength * (WORD_LENGTH + CRC_LENGTH))];
            if (cmd.ParameterLength > 0 && parameters?.Length != cmd.ParameterLength)
            {
                throw new ArgumentException(nameof(parameters));
            }

            Array.Copy(cmd.OpCode, 0, bytesToWrite, 0, cmd.OpCode.Length);
            if (parameters != null)
            {
                foreach(var i in Enumerable.Range(0, parameters.Length))
                {
                    Span<byte> span = bytesToWrite.AsSpan().Slice(i * (WORD_LENGTH + CRC_LENGTH) + cmd.OpCode.Length, (WORD_LENGTH + CRC_LENGTH));
                    span[0] = (byte)(parameters[i] >> 8);
                    span[1] = (byte)(parameters[i] & 0xFF);
                    span[2] = CRC(span.Slice(0, WORD_LENGTH));
                }
            }
            var argBytes = bytesToWrite.AsSpan(1);
            bus.WriteI2cBlockData(bytesToWrite[0], argBytes.ToArray());
            Thread.Sleep(cmd.ResponseTimeMs);
            if (cmd.ResponseLength > 0)
            {
                var data = new byte[cmd.ResponseLength * (WORD_LENGTH + CRC_LENGTH)];
                bus.ReadI2cBlockData(0, data);
                ushort[] result = ReadCheckSummedWords(data);
                return result;
            }
            return Array.Empty<ushort>();
        }

        private ushort[] ReadCheckSummedWords(byte[] data)
        {
            var span = data.AsSpan();
            var length = data.Length / 3;
            var result = new ushort[length];
            foreach (var i in Enumerable.Range(0, length))
            {
                result[i] = ReadCheckSummedWord(span.Slice(i * 3, 3));
            }
            return result;
        }

        private ushort ReadCheckSummedWord(Span<byte> data)
        {
            var words = data.Slice(0, 2);
            var crcWords = CRC(words);
            if (crcWords != data[2])
            {
                throw new Exception($"Invalid CRC. Got {crcWords}, Expected: {data[2]}");
            }
            return (ushort)(words[0] << 8 | words[1]);
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
