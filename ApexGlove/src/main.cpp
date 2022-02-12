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
//-------------------------------------------------------------------------------------------------------------------------

void setup()
{
  doSetup(); //setup.cpp
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
    dataToSendSerial += readMPU6050(indexerValue);
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