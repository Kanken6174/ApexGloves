#ifdef BLE_COMPATIBLE

#include "GloveCode.hpp"

String ReadPacketsFromBluetooth(BluetoothSerial SerialBT, String appendSerialData)
{
    String incoming = "";
    if (SerialBT.available() > 0)
        appendSerialData = "";
    while (SerialBT.available() > 0) //get the number of bytes (characters) available that already arrived and stored in the serial receive buffer
        appendSerialData += SerialBT.read();

    return incoming;
}

bool processBLEswitch(BluetoothSerial SerialBT, int BleSwitchCooldownCycles, bool BleEnabled)
{
    if (BleSwitchCooldownCycles <= 0 && BleEnabled != digitalRead(BLUETOOTHPIN))
    {
        if (digitalRead(BLUETOOTHPIN) == HIGH)
        {
            btStart();
            SerialBT.begin("ApexGlove Prototype");
            Serial.println("SerialBT started: ");
            BleSwitchCooldownCycles = 20;
            BleEnabled = true;
        }
        else
        {
            SerialBT.end();
            Serial.println("SerialBT disabled: ");
            BleSwitchCooldownCycles = 20;
            BleEnabled = false;
        }
        BleSwitchCooldownCycles = 5;
    }
    else if (BleSwitchCooldownCycles > 0)
    {
        BleSwitchCooldownCycles--;
    }
    return BleEnabled;
}
#endif