#include <Radio.h>
// #include <Solver.h>
#include <ControlMessage.h>


// #include <SoftwareSerial.h>
// SoftwareSerial DEBUG_SERIAL(7, 8); // DYNAMIXELShield UART RX/TX

// Solver solver;
Radio radio(false);

void setup() {
  Serial.begin(9600);
  // DEBUG_SERIAL.begin(115200);
  radio.begin();
  // solver.begin();
}

int count = 0;
void loop() {
  if(radio.hasMessage()){
    Serial.println("message received!");
    // Command message = radio.getNextMessage();
    // solver.execute(message);
  }
}
