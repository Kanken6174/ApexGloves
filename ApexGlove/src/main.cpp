#include <GloveCode.hpp>

String appendSerialData;
String dataToSendSerial = "";

#ifdef BLE_COMAPTIBLE
  bool BleEnabled = false;
  int BleSwitchCooldownCycles = BLE_SWITCH_DEBOUNCE_CYCLES_DEFAULT;
  BluetoothSerial SerialBT;
#endif

int indexerValue = 0;
CD74HC4067 multiplexer(S0PIN, S1PIN, S2PIN, S3PIN);
HMC5883L magnetometer(MAG_ADDR);
MPU6050 mpu;

// MPU control/status vars
bool dmpReady = false;  // set true if DMP init was successful
uint8_t mpuIntStatus;   // holds actual interrupt status byte from MPU
uint8_t devStatus;      // return status after each device operation (0 = success, !0 = error)
uint16_t packetSize;    // expected DMP packet size (default is 42 bytes)
uint16_t fifoCount;     // count of all bytes currently in FIFO
uint8_t fifoBuffer[64]; // FIFO storage buffer

// orientation/motion vars
Quaternion q;           // [w, x, y, z]         quaternion container
//-------------------------------------------------------------------------------------------------------------------------
void doMPU6050Setup();
void setup()
{
  doMPU6050Setup();
  doSetup(); //setup.cpp
  magnetometer.setup();
  DEBUG Serial.println("Setup done");
}

//-------------------------------------------------------------------------------------------------------------------------
void loop()
{
  dataToSendSerial = "";

  appendSerialData = ReadAvailableFromSerial();
#ifdef BLE_COMAPTIBLE
  if (btStarted())
  {
    appendSerialData += ReadPacketsFromBluetooth(SerialBT, appendSerialData);
  }
#endif

  ProcessSerialPackets(appendSerialData[0]);
  appendSerialData = "";

#ifdef BLE_COMAPTIBLE
  BleEnabled = processBLEswitch(SerialBT, BleSwitchCooldownCycles, BleEnabled);
#endif
}

//-------------------------------------------------------------------------------------------------------------------------
/**
 * @brief Will process a char received on the serial interface as a specific instruction for the glove
 * @param incoming the char which was received from serial
 */
void ProcessSerialPackets(char incoming)
{
  DEBUG Serial.println("Got character "+incoming);
  if (incoming == '#') //if data inside c equals to end character (#) which corresponds to "send me data" order
  {
    dataToSendSerial += readFingers(&indexerValue, multiplexer);
    DEBUG dataToSendSerial += " | ";
    //dataToSendSerial += readMPU6050(&indexerValue);
    if(dmpReady){
      mpu.dmpGetCurrentFIFOPacket(fifoBuffer);
      mpu.dmpGetQuaternion(&q, fifoBuffer);
      dataToSendSerial += String(q.w)+"W"+String(q.x)+"X"+String(q.y)+"Y"+String(q.z)+"Z";
    }
    DEBUG dataToSendSerial += " | ";
    dataToSendSerial += magnetometer.readValues(&indexerValue,marker);
    #ifdef BLE_COMAPTIBLE
      if (BleEnabled)
        SerialBT.println(dataToSendSerial);
      else
    #endif
      Serial.println(dataToSendSerial);

    incoming = ' ';
  }
  else if (incoming == '?') // end char (?) which corresponds to "reboot" order
  {
    incoming = ' ';
    ESP.restart(); //call reset
  }
  else if (incoming == '!') // end char (!) which corresponds to "identify yourself" order
  {
    Serial.println(ID);
    #ifdef BLE_COMAPTIBLE
      if (BleEnabled)
        SerialBT.println(ID);
    #endif
    incoming = ' ';
  }
  else if (incoming == '$')
  {
    #if(defined(BLE_COMPATIBLE) && defined(DEBUGGING))
    String t = digitalRead(BLUETOOTHPIN) ? "1" : "0";
    String e = btStarted() ? "on" : "off";
    Serial.println("pin: " + t + " btStarted :" + e);
      SerialBT.println("If you can hear me, it works!");
    #endif
  }
  else{
    //ignore
  }
  Serial.flush();
}

void doMPU6050Setup(){
  // join I2C bus (I2Cdev library doesn't do this automatically)
  #if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
      Wire.begin();
      Wire.setClock(400000); // 400kHz I2C clock. Comment this line if having compilation difficulties
  #elif I2CDEV_IMPLEMENTATION == I2CDEV_BUILTIN_FASTWIRE
      Fastwire::setup(400, true);
  #endif
  Serial.println(F("Initializing I2C devices..."));
  mpu.initialize();
  Serial.println(F("Testing device connections..."));
  Serial.println(mpu.testConnection() ? F("MPU6050 connection successful") : F("MPU6050 connection failed"));

  // load and configure the DMP
  Serial.println(F("Initializing DMP..."));
  devStatus = mpu.dmpInitialize();

  // supply your own gyro offsets here, scaled for min sensitivity
  mpu.setXGyroOffset(220);
  mpu.setYGyroOffset(76);
  mpu.setZGyroOffset(-85);
  mpu.setZAccelOffset(1788); // 1688 factory default for my test chip

  // make sure it worked (returns 0 if so)
  if (devStatus == 0) {
        // Calibration Time: generate offsets and calibrate our MPU6050
        mpu.CalibrateAccel(6);
        mpu.CalibrateGyro(6);
        mpu.PrintActiveOffsets();
        // turn on the DMP, now that it's ready
        Serial.println(F("Enabling DMP..."));
        mpu.setDMPEnabled(true);

        // set our DMP Ready flag so the main loop() function knows it's okay to use it
        Serial.println(F("DMP ready! Waiting for first interrupt..."));
        dmpReady = true;

        // get expected DMP packet size for later comparison
        packetSize = mpu.dmpGetFIFOPacketSize();
    } else {
        // ERROR!
        // 1 = initial memory load failed
        // 2 = DMP configuration updates failed
        // (if it's going to break, usually the code will be 1)
        Serial.print(F("DMP Initialization failed (code "));
        Serial.print(devStatus);
        Serial.println(F(")"));
    }
}