#include <hmc5883L.hpp>
#include <registers.hpp>

HMC5883L::HMC5883L(int addressMag) : address(addressMag){

    writeToRegister8bit(0x01, 0x0B, address);   //startup
    writeToRegister8bit(0x09, 0x1D, address);   //continuous measure mode
}

String HMC5883L::readValues(int* indexerValue,const char* indexes){
    String toReturn = "";
    uint16_t valueFromMag = 0;

    Wire.beginTransmission(address);
    Wire.write(0x03);                  // starting with register 0x03 (Data Output X MSB Register) [HMC5883L datasheet page 11]
    Wire.endTransmission(false);       // the parameter indicates that the Arduino will send a restart. As a result, the connection is kept active.
    Wire.requestFrom(address, 6, 1); // request a total of 3*2=6 registers, and send the stop message

    for (int i = 1; i != 3; i++)
    {
        valueFromMag = Wire.read() << 8 | Wire.read();  //we read the two registers at the same time, and concat them to get the full uint16_t (2*8)
        toReturn += String(indexes[(*indexerValue) + i]) + String(valueFromMag);
    }
    *indexerValue += 3;
}