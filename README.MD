##


Heavily inspired by the C example provided by raspberry pi here: http://www.raspberry-projects.com/pi/programming-in-c/i2c/using-the-i2c-interface
and the cross compilation story from here: https://github.com/davisford/cmake-cpputest

## Install tools

libi2c-dev

## build

cmake -DCMAKE_TOOLCHAIN_FILE=Toolchain-raspberrypi-arm.cmake
