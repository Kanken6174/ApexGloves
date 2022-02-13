#include <GloveCode.hpp>

String readMPU6050(int* indexerValue)
{
  String toBeSent;
  int16_t valueFromAccelOrGyro = 0;
  Wire.beginTransmission(MPU_ADDR);
  Wire.write(0x3B);                  // starting with register 0x3B (ACCEL_XOUT_H) [MPU-6000 and MPU-6050 Register Map and Descriptions Revision 4.2, p.40]
  Wire.endTransmission(false);       // the parameter indicates that the Arduino will send a restart. As a result, the connection is kept active.
  Wire.requestFrom(MPU_ADDR, 14, 1); // request a total of 7*2=14 registers - accel*3*2, temprature, gyro*3*2
  int i = 0;
  for (i = 1; i < 7; i++)
  {
    if(i == 4){
      uint16_t temprature_dump = Wire.read() << 8 | Wire.read();  //we read this to dump the temperature which we don't need
    }
    valueFromAccelOrGyro = Wire.read() << 8 | Wire.read();
    toBeSent += String(marker[*indexerValue + i]) + String(valueFromAccelOrGyro);
  }

  *indexerValue += 6;

  return toBeSent;
}