# Neural Rocket
This project is an experiment to see if a neural network can be taught to launch space rockets. For this purpose, I used [Unity](https://unity.com/) (for visualisation) and [ML-Agents](https://github.com/Unity-Technologies/ml-agents) library (for neural network things).

Neural network used in this projects contains 4 layers, described below.

#### Input layer
*every value is normalized to [-1..1] range*
 - **altitude** - from 0 to MaxOperationalAltitude
 - **target altitude** - from 0 to MaxOperationalAltitude
 - **x speed** - from -OrbitalSpeed * 2 to OrbitalSpeed * 2
 - **y speed** - from -OrbitalSpeed to OrbitalSpeed
 - **z rotation** - from -180 to 180 degrees
 - **angle of attack** - from -90 to 90 degrees
 - **z rotation speed** - from -PI/2 to PI/2

where default values are:
 - **MaxOperationalAltitude** = 20000 meters
 - **OrbitalSpeed** = 3000 m/s

#### First hidden layer
*32 neurons*

#### Second hidden layer
*32 neurons*

#### Output layer
*every value is normalized to [-1..1] range*
 - **gimbal** - from -MaxGimbal to MaxGimbal
 - **thrust** - from 0 to 100 percents

where default values are:
 - **MaxGimbal** = 15 degrees

![test](./Media/neuralrocket1.gif)