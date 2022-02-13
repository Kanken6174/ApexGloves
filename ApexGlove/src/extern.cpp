#include <GloveCode.hpp>
//A1162 B1017 C3999 D3070 E2439 F2767 G1746 H1563 I1866|J0 K0 L0 M0|N-4720 O-404 P14480 Q-3968 R291
String readFingers(int* indexerValue, CD74HC4067 multiplexer)
{
  int readValue;
  String toBeSent;
  for (int i = 0; i != NUM_ART; i++)
  {
    multiplexer.channel(i);
    readValue = analogRead(SIGPIN);
    if (i >= 9 && i <= 13)  //contacts en alu
    {
      if (readValue >= ANALOG_TRIGGER)
      {
        readValue = 1;
      }
      else
      {
        readValue = 0;
      }
    }
    if(i == 8){ //interrupteur du joystick
      readValue = (readValue != 0) ? 0 : 1;
    }
    toBeSent += marker[i];
    toBeSent += readValue;
    *indexerValue = i; //will be the index up to which readFingers() has used letters.
  }
  return toBeSent;
}