/**
 * @file GloveCode.h
 * @author Yorick Geoffre
 * @brief this header defines most esp32 pins for the apexGlove
 * @version 0.1
 * @date 2022-02-12
 * 
 * @copyright Copyright (c) 2022
 * 
 */

#include <Wire.h>       // This library allows you to communicate with I2C devices.
#include <CD74HC4067.h> // multiplexer lib
#include <hmc5883L.hpp>
#include <registers.hpp>
#include <Arduino.h>

#ifdef ESP32
    #include "ESP32_pins.hpp"
    #define BAUDRATE 115200
    //#define BLE_COMPATIBLE
#elif ESP8266
    #include "ESP8266_pins.hpp"
    #define BAUDRATE 74880
#else
    #include "GENERIC_AVR_pins.hpp"
    #define BAUDRATE 115200
#endif

#define DEBUGGING false //sets the debug mode (verbose logging) or not
#define DEBUG if(DEBUGGING) //adds a condition to check for if we are in debug mode

#pragma region I2CAddresses
    #define MPU_ADDR 0x68   // I2C address of the MPU-6050. If AD0 pin is set to HIGH on the MPU, the I2C address will be 0x69.
    #define MAG_ADDR 0x1E   //  I2C address of the magnetometer
#pragma endregion I2CAddresses

#ifdef BLE_COMAPTIBLE
    #define BLE_SWITCH_DEBOUNCE_CYCLES_DEFAULT 0
    #include <BluetoothSerial.h>
    #include <BLEUUID.h>
    #include <esp_bt.h>
#endif

#define NUM_ART 13
#define ID 'R'



//Defines the markers used to separate data in each frame
const char marker[] = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'Y', 'Z'};

void doSetup();

void ProcessSerialPackets(char incoming);

String ReadAvailableFromSerial();

#ifdef BLE_COMAPTIBLE
    bool processBLEswitch(BluetoothSerial SerialBT,int BleSwitchCooldownCycles, bool BleEnabled);
    String ReadPacketsFromBluetooth(BluetoothSerial SerialBT, String appendSerialData);
#endif

String readFingers(int* indexerValue, CD74HC4067 multiplexer);
String readMPU6050(int* indexerValue);

/**
 * @brief This function is used for communication with external I2C devices by directly writing to their registers
 * 
 * @param message the new value for the 8 bits of the register
 * @param address the address of the 8 bit register
 * @param i2cAddress the i2c address of the device
 */
void writeToRegister8bit(int message, int address, int i2cAddress);