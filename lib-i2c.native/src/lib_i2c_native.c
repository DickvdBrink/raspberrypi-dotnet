#include <stdint.h>
#include "smbus.h"

int32_t NativeAdd(int32_t a, int32_t b)
{
    return a + b;
}

__s32 I2CSmbusReadI2CBlockData(int file, __u8 command, __u8 length,
					   __u8 *values)
{
    return i2c_smbus_read_i2c_block_data(file, command, length, values);
}

__s32 I2CSmbusWriteI2CBlockData(int file, __u8 command, __u8 length,
					   __u8 *values)
{
    return i2c_smbus_write_i2c_block_data(file, command, length, values);
}

