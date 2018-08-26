SET (CMAKE_SYSTEM_NAME Linux)
SET (CMAKE_SYSTEM_VERSION 1)

SET (PI_TOOLS_HOME ${CMAKE_CURRENT_SOURCE_DIR}/../raspberrypi-tools)
SET (PiToolsDir arm-bcm2708/gcc-linaro-arm-linux-gnueabihf-raspbian-x64)

# specify the cross compiler
SET (CMAKE_C_COMPILER ${PI_TOOLS_HOME}/${PiToolsDir}/bin/arm-linux-gnueabihf-gcc)
SET (CMAKE_CXX_COMPILER ${PI_TOOLS_HOME}/${PiToolsDir}/bin/arm-linux-gnueabihf-g++)

# where is the target environment
SET (CMAKE_FIND_ROOT_PATH ${PI_TOOLS_HOME}/${PiToolsDir})

# search for programs in the build host directories
SET (CMAKE_FIND_ROOT_PATH_MODE_PROGRAM NEVER)

# for libraries and headers in the target directories
SET (CMAKE_FIND_ROOT_PATH_MODE_LIBRARY ONLY)
SET (CMAKE_FIND_ROOT_PATH_MODE_INCLUDE ONLY)

add_definitions(-Wall -std=c11)