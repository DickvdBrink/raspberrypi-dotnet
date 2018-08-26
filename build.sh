#!/bin/bash
if [ ! -e build ]; then
  mkdir build
fi
cd build

dotnetcmd=dotnet
if ! type $dotnetcmd 2>/dev/null && type dotnet.exe 12>/dev/null; then
  # Maybe it is called from wsl
  dotnetcmd=dotnet.exe
fi
cmake ../lib-i2c.native -DCMAKE_TOOLCHAIN_FILE=../lib-i2c.native/Toolchain-raspberryPi.cmake
make
$dotnetcmd publish ../i2c-test -o ../build/dist -r linux-arm

cp lib/libI2CNative.so dist
#scp dist/* pi@192.168.1.219:/home/pi/Desktop/dotnet/
rsync --delete --recursive dist/ pi@192.168.1.219:/home/pi/Desktop/dotnet/
