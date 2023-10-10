#include "Radio.h"

#define N_SENDERS 3

const int sender_delay_us = 100;
const int sender_pins[N_SENDERS] = {5, 6, 7};
char data_to_send[5] = "ping";

Radio radio;

void initSenders(){
  for(int i = 0; i < N_SENDERS; i++){
    pinMode(sender_pins[i], OUTPUT);
    digitalWrite(sender_pins[i], LOW);
  }
}

void setup() {
  Serial.begin(9600);
  radio.setup();
  initSenders();
}

void loop() {
  if(send()){
    startPing();
  }
  delay(100);
}

void startPing(){
  pingSender(sender_pins[current_sender_idx]);
  current_sender_idx+=1;
  if(current_sender_idx >= N_SENDERS){
    current_sender_idx = 0;
  }
}

void pingSender(int pin){
  delayMicroseconds(sender_delay_us);
  digitalWrite(pin, HIGH);
  delayMicroseconds(10);
  digitalWrite(pin, LOW);
}

