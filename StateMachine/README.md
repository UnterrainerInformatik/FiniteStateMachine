# FiniteStateMachine

This project is a finite state machine designed to be used in games.

#### Description

So far this is a test-project.

At some time in the future maybe this will replace the code we have for keyboard-input, clicked buttons on the GUI or for the connection procedure when setting up peer2peer connections in our games.

The idea is to generate a single finite machine for every 'layer' of input that your engine allows. In our example it's a multi-button GUI. Some of the buttons will stay pressed and the GUI will enter a 'selection-grid-mode' until the left button is pressed again. Some of them just do immediate actions and become immediately released afterwards. Some of both of those have a refresh-time and stay disabled for that period.
In this example the state of the GUI ('selection-grid-mode' or not) would be such a machine.

We will place those machines in a single class where they could 'talk with each other' by reading their respective states. That way it will be possible to construct and react on compound states.

A nice example for such a machine is the setup of a multiplayer game. It would be like the following:

##### Server:

* Send the 'load level' signal to other players
* Load level
* Display 'waiting for other players' message-box
* Wait for all other players to finish loading
* Send the level-data to other players and wait for acknowledgement from each of them
* Remove the 'waiting for other players' message-box
* Send 'start' signal to other players
* Start the game



#### Test-drive

Time to take it for a test-drive.

The motivation for this project came from a nice article I found [here](http://gameprogrammingpatterns.com/state.html) which comes with some examples. We'll try to solve the proposed problems with our new project.

*By the way: [This](http://gameprogrammingpatterns.com/) seems to be a great book, so try to support the author in any way possible for you.*

He's making a point using a **F**inite **S**tate **M**achine (FSM) that looks like this:

* ducking --(release down)--> standing
* standing --(press down)--> ducking
* standing --(press B)--> jumping
* jumping --(press down)--> diving

So the file ```GameProgrammingPatterns1.cs``` in the test-folder contains that machine.

