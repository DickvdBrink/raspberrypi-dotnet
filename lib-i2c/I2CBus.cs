using System;
using System.Runtime.InteropServices;

namespace lib_i2c
{
    public class I2CBus : IDisposable
    {
        // Inspired by http://www.raspberry-projects.com/pi/programming-in-c/i2c/using-the-i2c-interface

        // https://github.com/torvalds/linux/blob/master/include/uapi/linux/i2c-dev.h
        // Constants come from the linux kernel at the above link
        internal const int I2C_SLAVE = 0x0703;

        private int fd = -1;

        public void Open(string bus)
        {
            this.fd = NativeMethods.Open(bus, NativeMethods.OPEN_READ_WRITE);
        }

        public void SetDevice(int address)
        {
            NativeMethods.Ioctl(this.fd, I2C_SLAVE, address);
        }

        public int ReadData(byte[] buffer)
        {
            return NativeMethods.Read(this.fd, buffer, buffer.Length);
        }

        public int WriteData(byte[] buffer)
        {
            return NativeMethods.Write(this.fd, buffer, buffer.Length);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);  
        }

        protected virtual void Dispose(bool disposing)
        {  
            NativeMethods.Close(fd);
        }
    }
}
