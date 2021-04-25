#include "Wire.h" // This library allows you to communicate with I2C devices.
#define NUM_ART 12 // number of potentiometers, or "articulations"
#include <CD74HC4067.h>// multiplexer lib
//--------------------------------------------------------------------------------------------------------------------
void(* resetFunc) (void) = 0;//declare reset function at address 0
const int MPU_ADDR = 0x68;                      // I2C address of the MPU-6050. If AD0 pin is set to HIGH, the I2C address will be 0x69.
const char ID = 'R';                            //indicates which glove this is (here L = left, R = right)
char tmp_str[7];                                // temporary variable used in convert function
char incoming;                                  //incoming data storage
String appendSerialData;                        //Stores data from serial (using incoming)
const int artPin[] = {A0, A1, A2, A3, A6, A7}; //Indicates every analog pin connected to a potentiometer
const char marker[] = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'Y', 'Z'};
const char invMarker[] = {'F', 'E', 'D', 'C', 'B', 'A'};

int indexerValue = 0;
int timeToWait = 100;                                                                   //defines the wait time for each loop (might go unused because of serial)
const String artPinName[6] = {"pinkie", "ring", "Middle", "Index", "thumbX", "thumbY"}; //Used to name each pin to the corresponding finger
CD74HC4067 multiplexer(5, 6, 7, 8);

//--------------------------------------------------------------------------------------------------------------------

void setup()
{
  for (int i = 0; i < NUM_ART; i++)
    pinMode(artPin[i], INPUT);


  Wire.begin();
  Wire.beginTransmission(MPU_ADDR); // Begins a transmission to the I2C slave (GY-521 board)
  Wire.write(0x6B);                 // PWR_MGMT_1 register
  Wire.write(0);                    // set to zero (wakes up the MPU-6050)
  Wire.endTransmission(true);       // Frees I2C connection for later use

  Serial.begin(38400);
}
//-------------------------------------------------------------------------------------------------------------------------
void loop()
{
  String dataToSendSerial = " ";
  //  Serial.println("pinkie ring Middle Index thumbX thumbY accelerometer_x accelerometer_y accelerometer_z gyro_x gyro_y gyro_z");

  while (Serial.available() > 0) //get the number of bytes (characters) available that already arrived and stored in the serial receive buffer
  {
    incoming = Serial.read();     //read incoming serial data and store it into c variable
    appendSerialData += incoming; //append data in c and store it in this variable
  }
  if (incoming == '#') //if data inside c equals to end character (#) which corresponds to "send me data" order
  {
    digitalWrite(LED_BUILTIN, HIGH);
    dataToSendSerial += readFingers();
    dataToSendSerial += readMPU6050();
    Serial.print(dataToSendSerial);
    Serial.print('\n');
    incoming = ' ';
    digitalWrite(LED_BUILTIN, LOW);
  }
  else if (incoming == '?') // end char (?) which corresponds to "kill yourself" order
  {
    incoming = ' ';
    resetFunc(); //call reset 
  }
  else if (incoming == '!') // end char (!) which corresponds to "identify yourself" order
  {
    Serial.print(ID);
    Serial.print('\n');
    incoming = ' ';
  }
}
//-------------------------------------------------------------------------------------------------------------------------
String readFingers()
{
  int readValue;
  String toBeSent;
  for(int i = 0; i != NUM_ART; i++)
  {
    multiplexer.channel(i);
    readValue = analogRead(A0);
    if(i >= 8 && i <= 11){
      if(readValue >= 1000){
        readValue = 1;
      }
      else
      {
        readValue = 0;
      }
    }
    toBeSent += marker[i];
    toBeSent += readValue;
    indexerValue = i; //will be the index up to which readFingers() has used letters.
  }
  return toBeSent;
}
//-------------------------------------------------------------------------------------------------------------------------
String readMPU6050()
{
  String toBeSent;
  int16_t valueFromAccelOrGyro = 0;
  /*int16_t accelerometer_x, accelerometer_y, accelerometer_z; // variables for accelerometer raw data
  int16_t gyro_x, gyro_y, gyro_z;                            // variables for gyro raw data
  int16_t temperature;                                       // variables for temperature data */
  Wire.beginTransmission(MPU_ADDR);
  Wire.write(0x3B);                        // starting with register 0x3B (ACCEL_XOUT_H) [MPU-6000 and MPU-6050 Register Map and Descriptions Revision 4.2, p.40]
  Wire.endTransmission(false);             // the parameter indicates that the Arduino will send a restart. As a result, the connection is kept active.
  Wire.requestFrom(MPU_ADDR, 7 * 2, true); // request a total of 7*2=14 registers

  // "Wire.read()<<8 | Wire.read();" means two registers are read and stored in the same variable
 /* accelerometer_x = Wire.read() << 8 | Wire.read(); // reading registers: 0x3B (ACCEL_XOUT_H) and 0x3C (ACCEL_XOUT_L)
  accelerometer_y = Wire.read() << 8 | Wire.read(); // reading registers: 0x3D (ACCEL_YOUT_H) and 0x3E (ACCEL_YOUT_L)
  accelerometer_z = Wire.read() << 8 | Wire.read(); // reading registers: 0x3F (ACCEL_ZOUT_H) and 0x40 (ACCEL_ZOUT_L)
  temperature = Wire.read() << 8 | Wire.read();     // reading registers: 0x41 (TEMP_OUT_H) and 0x42 (TEMP_OUT_L)
  gyro_x = Wire.read() << 8 | Wire.read();          // reading registers: 0x43 (GYRO_XOUT_H) and 0x44 (GYRO_XOUT_L)
  gyro_y = Wire.read() << 8 | Wire.read();          // reading registers: 0x45 (GYRO_YOUT_H) and 0x46 (GYRO_YOUT_L)
  gyro_z = Wire.read() << 8 | Wire.read();          // reading registers: 0x47 (GYRO_ZOUT_H) and 0x48 (GYRO_ZOUT_L)

  // print out data
  toBeSent += "N";
  toBeSent += accelerometer_x;
  toBeSent += "O";
  toBeSent += accelerometer_y;
  toBeSent += "P";
  toBeSent += accelerometer_z;
  // the following equation was taken from the documentation [MPU-6000/MPU-6050 Register Map and Description, p.30]
  //Serial.print(" "); Serial.print(temperature/340.00+36.53); //No need for this, this is the room temp.
  toBeSent += "Q";
  toBeSent += gyro_x;
  toBeSent += "R";
  toBeSent += gyro_y;
  toBeSent += "S";
  toBeSent += gyro_z;
  */
  for(int i = 1; i != 6; i++) //this is the same as the block of code that has been comented out, just neater.
  {
    valueFromAccelOrGyro = Wire.read() << 8 | Wire.read();
    toBeSent += marker[indexerValue + i] + valueFromAccelOrGyro;
  }
  return toBeSent;
}
//-------------------------------------------------------------------------------------------------------------------------