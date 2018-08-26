using System;
using lib_i2c;

namespace i2c_test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Console.WriteLine("Calling native function");

            try {
                int bar = NativeMethods.NativeAdd(1, 3);
                Console.WriteLine(bar);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            I2CBus b = new I2CBus();
            b.Open("/dev/i2c-1");
            b.SetDevice(0x58);
        }
    }
}
