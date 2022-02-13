#include <registers.hpp>

void writeToRegister8bit(int message, int targetRegister, int i2cAddress){
    Wire.begin();
    Wire.beginTransmission(i2cAddress); // Begins a transmission to the I2C slave (GY-521 board)
    Wire.write(targetRegister);              // ask to write to this register
    Wire.write(message);                    // set this register's value to the message value (byte)
    Wire.endTransmission(true);
}