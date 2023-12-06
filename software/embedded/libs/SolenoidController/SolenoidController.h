#ifndef SolenoidController_h
#define SolenoidController_h
#include <Servo.h>

class SolenoidController {
private:
	Servo self_servo;
	Servo other_servo;

public:
	SolenoidController();
	void begin();
	bool executeAction(uint8_t action);
};
#endif // !SolenoidController_h
