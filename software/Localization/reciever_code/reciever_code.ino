// ok, todo now: (low priority)
// 1. increase accuracy
// - add the temperature and humidity into the equation for the speed of sound (then use arduino sensor)
//  437 microsecond delay
// 2. clean up code

// 1. no use in getting ahead in those things yet, its still way too soon, 
// focus on the more important things for now

#define N_SENDERS 3

struct Point{
  double x;
  double y;
};

const int micro_delay = 100;

const int trigPin = 7;
const int echoPin = 6;
const int cePin = 9;
const int csnPin = 10;

int sender_distance[N_SENDERS] = {0,0,0};
int current_sender_idx = 0;

const Point sender_position[N_SENDERS] = {{0,0},{0,0},{0,0}};

const char expected_message[5] = "ping";

Radio radio;

long duration;
int distance;

void initUS(){
  pinMode(trigPin, OUTPUT);
  pinMode(echoPin, INPUT);
  digitalWrite(trigPin, LOW);
}

void setup() {
  Serial.begin(9600);
  radio.setup();
  initUS();
}

void loop() {
  while(!radio.hasMessage(expected_message));
  startRecieve();
}

void startRecieve(){
  receiveSender(current_sender_idx);
  updateSenderIndex();
  Point receiver_position = trilateration();
  Serial.print("Position: ");
  Serial.print(receiver_position.x);
  Serial.print(", ");
  Serial.println(receiver_position.y);
}

void updateSenderIndex(){
  current_sender_idx+=1;
  if(current_sender_idx >= N_SENDERS){
    current_sender_idx = 0;
  }
}

void receiveSender(int index){
  double toa = getTOA();
  double distance = getDistance(toa);
  sender_distance[index] = distance;
}

double getTOA(){
  delayMicroseconds(micro_delay);
  digitalWrite(trigPin, HIGH);
  delayMicroseconds(10);
  digitalWrite(trigPin, LOW);
  double toa = pulseIn(echoPin, HIGH);
  return toa;
}

double getDistance(double toa){
  double distance = toa * 0.034;
  return distance; 
}

Point trilateration(){
  double x1 = sender_position[0].x;
  double x2 = sender_position[1].x;
  double x3 = sender_position[2].x;
  double y1 = sender_position[0].y;
  double y2 = sender_position[1].y;
  double y3 = sender_position[2].y;
  
  double r1 = sender_distance[0];
  double r2 = sender_distance[1];
  double r3 = sender_distance[2];
  
  double A = 2*x2 - 2*x1;
  double B = 2*y2 - 2*y1;
  double C = r1*r1 - r2*r2 - x1*x1 + x2*x2 - y1*y1 + y2*y2;
  double D = 2*x3 - 2*x2;
  double E = 2*y3 - 2*y2;
  double F = r2*r2 - r3*r3 - x2*x2 + x3*x3 - y2*y2 + y3*y3;
  double x = (C*E - F*B) / (E*A - B*D);
  double y = (C*D - A*F) / (B*D - A*E);
  return {x, y};
}


