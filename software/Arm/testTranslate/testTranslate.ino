#include <Radio.h>
#include <ControlMessage.h>

struct Point test_point {50,50,0};
struct Command test_command{MOVE, test_point, 4, 5};

struct Point test_point2 {150,50,0};
struct Command test_command2{MOVE, test_point2, 6, 7};

Radio radio(true);
bool message1 = true;
void setup(){
  Serial.begin(9600);
  radio.begin();
}

void loop(){
  Serial.println("starting loop");
  if(message1){
    radio.sendMessage(test_command);
  } else {
    radio.sendMessage(test_command2);
  }
  message1 = !message1;
  Serial.println("sent message!");
  delay(1000);
}