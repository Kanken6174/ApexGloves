#include <GloveCode.hpp>

String readFingers(int* indexerValue, CD74HC4067 multiplexer)
{
  int readValue;
  String toBeSent;
  for (int i = 0; i != NUM_ART; i++)
  {
    multiplexer.channel(i);
    readValue = analogRead(SIGPIN);
    if (i >= 9 && i <= 13)
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
    toBeSent += marker[i];
    toBeSent += readValue;
    *indexerValue = i; //will be the index up to which readFingers() has used letters.
  }
  return toBeSent;
}