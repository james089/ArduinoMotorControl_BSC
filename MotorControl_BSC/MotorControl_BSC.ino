#include <CmdMessenger.h>  // CmdMessenger

const int motorADirPin = 8;
const int motorAStepPin = 9;

const int motorBDirPin = 10;
const int motorBStepPin = 11;

const int ledPin = 13;

int numberOfSteps = 200;
int numberOfMicroSteps = 2000;
int pulseWidth = 5;  // microseconds 1~60
int stepInterval = 100; // microseconds

// Attach a new CmdMessenger object to the default Serial port
CmdMessenger cmdMessenger = CmdMessenger(Serial);

// Commands
enum
{
  setLED,
  rotateAngle, // 
};

// Callbacks define on which received commands we take action 
void attachCommandCallbacks()
{
  cmdMessenger.attach(rotateAngle, OnRotateAngle);
  cmdMessenger.attach(setLED, OnSetLed);
}

void setup() { 
  pinMode(motorADirPin, OUTPUT);
  pinMode(motorAStepPin, OUTPUT);
  pinMode(motorBDirPin, OUTPUT);
  pinMode(motorBStepPin, OUTPUT);
  pinMode(ledPin, OUTPUT);
  
  digitalWrite(motorADirPin, LOW);
  digitalWrite(motorAStepPin, LOW);
  digitalWrite(motorBDirPin, LOW);
  digitalWrite(motorBStepPin, LOW);

  Serial.begin(115200);  
  // Adds newline to every command
  cmdMessenger.printLfCr();   
  // Attach my application's user-defined callback methods
  attachCommandCallbacks();
  //Serial.write("GO?");
}

int motorIndex;
double angle;

// Function code
void OnRotateAngle(){
  motorIndex = cmdMessenger.readInt16Arg();
  angle = cmdMessenger.readDoubleArg();
  
  switch (motorIndex)
  {
    case 0:
        _runAngle(motorADirPin, motorAStepPin, angle);break;
    case 1:
        _runAngle(motorBDirPin, motorBStepPin, angle);break;
  }
}

// Motor Interaction code
void _runAngle(int directionPin, int stepPin, double angle){
    if (angle < 0)
    {
      angle = abs(angle);
      //digitalWrite(ledPin, LOW);
      digitalWrite(directionPin, LOW);
    }
    else
    {
      //digitalWrite(ledPin, HIGH);
      digitalWrite(directionPin, HIGH);
    }
    
    for (int i = 0; i < (numberOfMicroSteps * angle) / 360.0; i ++)
    {    
      delayMicroseconds(pulseWidth);
      digitalWrite(stepPin, HIGH);
      delayMicroseconds(pulseWidth);
      digitalWrite(stepPin, LOW);
      delayMicroseconds(stepInterval);
    }
}

// Blinking led variables 
bool ledState = 0;   // Current state of Led
int ledIndex = 0;

void OnSetLed()
{
  ledIndex = cmdMessenger.readInt16Arg();
  ledState = cmdMessenger.readBoolArg();
  switch (ledIndex)
  {
    case 13: digitalWrite(ledPin, ledState?HIGH:LOW);break;
  }
}

void loop() { 
  // Process incoming serial data, and perform callbacks
  cmdMessenger.feedinSerialData();
}
