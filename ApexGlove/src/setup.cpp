#include <GloveCode.hpp>

/**
 * @brief will setup the esp32 for usage within the glove
 */
void doSetup()
{
    pinMode(SIGPIN, INPUT);

    pinMode(S0PIN, OUTPUT);
    pinMode(S1PIN, OUTPUT);
    pinMode(S2PIN, OUTPUT);
    pinMode(S3PIN, OUTPUT);

    pinMode(BLUETOOTHPIN, INPUT);

    Wire.begin();
    Wire.beginTransmission(MPU_ADDR); // Begins a transmission to the I2C slave (GY-521 board)
    Wire.write(0x6B);                 // ask to write to the PWR_MGMT_1 register
    Wire.write(0);                    // set this register to zero (wakes up the MPU-6050)
    Wire.endTransmission(true);       // Frees I2C connection for later use

    Serial.begin(BAUDRATE);
}