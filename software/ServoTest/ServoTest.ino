#include <Servo.h>

#define lock_servo_pin 5
#define self_servo_pin 7

int angle_increment = 10;

class ServoWrapper {
  private:
  int angle;
  int pin;

  Servo servo;
  public:
  ServoWrapper(int servo_pin) : angle{30}, pin{servo_pin} {
    servo.attach(servo_pin);
    servo.write(angle);
   }

   void checkInput(char up, char down){
        if(Serial.available() > 0)  {
          bool was_valid = true;
          int incomingData= Serial.read();
          if(incomingData == up){
            angle -= angle_increment;
            if(angle <= 0){
              angle = 0;
            }
          } else if (incomingData == down){
            angle += angle_increment;
            if(angle >= 180){
              angle = 180;
            }
          } else {
            was_valid = false;
          }
          if(was_valid){
            Serial.print(pin); Serial.print(": ");
            Serial.println(angle);
          }
        }
        servo.write(angle);
   }
};

ServoWrapper lock_puller_servo(lock_servo_pin);
ServoWrapper self_lock_servo(self_servo_pin);

void setup() {
  Serial.begin(9600);
  Serial.println("I am alive");
}

// 7 is connected to the top
// 
void loop() 
{ 
  lock_puller_servo.checkInput('1', '2');
  self_lock_servo.checkInput('3', '4');
}