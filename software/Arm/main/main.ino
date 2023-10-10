#include <Comms.h>

#define LED_PIN 9
Comms comms;
void setup(){
  pinMode(LED_PIN, OUTPUT);
  digitalWrite(LED_PIN, LOW);
  comms.begin();
}

void loop(){
  if(comms.hasMessage()){
    uint8_t message = comms.getNextMessage();
    comms.sendMessage();
    digitalWrite(LED_PIN, HIGH);
    delay(500);
    digitalWrite(LED_PIN, LOW);
  }
}