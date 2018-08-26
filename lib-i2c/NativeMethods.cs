using System;
using System.Runtime.InteropServices;

namespace lib_i2c
{
    public /*internal*/ class NativeMethods
    {
        [DllImport("libI2CNative", EntryPoint = "NativeAdd")]
        public static extern int NativeAdd(int a, int b);

        [DllImport("libI2CNative", EntryPoint = "I2CSmbusReadI2CBlockData")]
        public static extern int I2CSmbusReadI2CBlockData(int file, byte cmd, byte length, byte[] values);

        [DllImport("libI2CNative", EntryPoint = "I2CSmbusWriteI2CBlockData")]
        public static extern int I2CSmbusWriteI2CBlockData(int file, byte cmd, byte length, byte[] values);

        // https://github.com/torvalds/linux/blob/master/include/uapi/asm-generic/fcntl.h
        // Constants come from the linux kernel at the above link
        internal const int  OPEN_READ_WRITE = 2;

        [DllImport("libc", EntryPoint = "open")]
        public static extern int Open(string fileName, int mode);

        [DllImport("libc", EntryPoint = "close")]
        public static extern int Close(int fd);
 
        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        public extern static int Ioctl(int fd, int request, int data);
 
        [DllImport("libc", EntryPoint = "read", SetLastError = true)]
        internal static extern int Read(int handle, byte[] data, int length);
        
        [DllImport("libc", EntryPoint = "write", SetLastError = true)]
        internal static extern int Write(int handle, byte[] data, int length);

    }
}
