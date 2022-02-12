#include <GloveCode.hpp>

String readMPU6050(int indexerValue)
{
  String toBeSent;
  int16_t valueFromAccelOrGyro = 0;
  Wire.beginTransmission(MPU_ADDR);
  Wire.write(0x3B);                  // starting with register 0x3B (ACCEL_XOUT_H) [MPU-6000 and MPU-6050 Register Map and Descriptions Revision 4.2, p.40]
  Wire.endTransmission(false);       // the parameter indicates that the Arduino will send a restart. As a result, the connection is kept active.
  Wire.requestFrom(MPU_ADDR, 14, 1); // request a total of 7*2=14 registers

  for (int i = 1; i != 6; i++)
  {
    valueFromAccelOrGyro = Wire.read() << 8 | Wire.read();
    toBeSent += String(marker[indexerValue + i]) + String(valueFromAccelOrGyro);
  }
  return toBeSent;
}