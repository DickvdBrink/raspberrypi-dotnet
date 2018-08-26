dotnet.exe publish i2c-test -o output -r linux-arm

scp i2c-test/output/* pi@192.168.1.219:/home/pi/Desktop/dotnet/