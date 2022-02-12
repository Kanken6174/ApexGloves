#include <Arduino.h>
#include <registers.hpp>

class HMC5883L{
    private:
        int address;
    public:
        HMC5883L(int addressMag);
        String readValues(int* indexerValue,const char* indexes);
};