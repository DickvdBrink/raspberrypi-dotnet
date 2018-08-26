##


Heavily inspired by the C example provided by raspberry pi here: http://www.raspberry-projects.com/pi/programming-in-c/i2c/using-the-i2c-interface
and the cross compilation story from here: https://github.com/davisford/cmake-cpputest

## Install tools

libi2c-dev

## build instructions

The current build instructions are for a Windows pc from WSL (Windows Subsystem for Linux) but should work mostly on other systems.

First install the required build tools.

```
sudo apt-get update && sudo apt-get install libi2c-dev cmake
```

Get the `raspberry pi` build tools by executing `git submodule update --init --recursive`.

Now execute the build.sh script which will first build the native library and after that builds the dotnet projects.

Everything is automatically uploaded to the raspberry pi, so you should update the ip adress.


