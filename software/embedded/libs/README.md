# Library Descriptions

## ArmConfig
* defines high-level constants used throughout all other libraries

## Comms:
* this code runs on the assembly-module microcontroller
* it reads and returns messages passed to it

## ControlMessage:
* Defines the different types of message packets

## Plan
* The Plan class is more or less a list of commands, for example, you can chain together multiple different move and action commands

## Radio:
* Currently unused, this class allows for wireless communication using nrf24l01

## ServoController:
* Code to write to the Dynamixel servos

## ServoControllerAbstract:
* abstract class which defines the neccessary functions for a servo controller

## SimServoController:
* This is used when we are running the embedded code in "sim"
* Running in "sim" means we will run all code as usual on the actual hardware (the microcontroller), except instead of commands to the servos, we will intercept the values and send them back to our simulator (Unity3D game engine). 

## LockController:
* This code is used on the assembly modules microcontroller. 
* When a command is recieved to actuate one of the locks on the assembly module, it actuates the correct servo

## Solver:
* 

# Library Interactions:


