#include <GloveCode.hpp>
/**
 * @brief Will read the available strings from the seriazl interface (blocking until data is available)
 * 
 * @return String the frame received from the serial interface
 */
String ReadAvailableFromSerial()
{
    DEBUG Serial.println("Waiting for serial input");
    String appendSerialData = "";
    while(Serial.available() <= 0){}
    appendSerialData += (char)Serial.read(); //append data in c and store it in this variable
    appendSerialData.trim();
    Serial.flush();
    DEBUG Serial.println("Got string \""+appendSerialData+"\"");

    return appendSerialData;
}