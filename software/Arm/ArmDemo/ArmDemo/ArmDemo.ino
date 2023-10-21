#include <Solver.h>
#include <UserInterface.h>
#include <ControlMessage.h>
#include <SoftwareSerial.h>

SoftwareSerial software_serial{ 7, 8 };
Solver solver{&software_serial};
UserInterface user_interface{&software_serial};

void setup() {
  software_serial.begin(9600);
  software_serial.println("starting demo...");
  user_interface.begin();
  solver.begin();
}

void readCommand(const Command& command){
  software_serial.println(command.type == SERVO_COMMAND);
  for(int i = 0; i < N_JOINTS;  i++){
    software_serial.println(command.data.servo_command.servo_delta[i]);
  }
}
void loop() {
  if(user_interface.hasMessage()){
      Command next_message = user_interface.getNextMessage();
      solver.execute(next_message);
      // readCommand(next_message);
  }
}
