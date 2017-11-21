
## FlowCalibration
	* ControlPage - Handles the view and layout of the frontend. Data is accesed through bindings in ControlPageViewModel. 
					Contains all event reactions triggered by the user.

	* ControlPageViewModel - Holds the data that should be shown in the view. Calls profileGenerator. Uses functions from the Model. 

	* ProfileGenerator - Contains functions to generate lists with DataPoints that can be plotted in the view.

	Model features that needs to be reached:
	* Initialize modbus communication - Create an instance of ModbusCommunication with port name as argument. 
	* Initialize motor control - Create an instance of MotorControl with the ModbusCommunication object as argument.
	* Run flow profile - Profile converter takes a list of flows and times and returns a list with positions and times. 
						 Motor control takes the list with positions and times and do the sequence while recording actual
						 Position, Velocity and Time and returns that.
						 Profile converter converts the Position and Velocity back to flow and volume to be plotted in the view.

## Model
	
	* ProfileConverter - Contains functions for converting between flow and volume and flow to position and velocity.
						 Needs parameters related to the mechanical construction, syringe diameter.

	* MotorControl - Controls the motor with a list of position and times or velocity and times. 
				     Can convert from position to ticks or velocity to ticks per second and the other way.
					 Implements homing sequence.

	* ModbusCommunication - Wrapper for the NModBus4.Serial library.
							Handles conversions between different signed and unsigned integer and short types and stuff like that.

	* Program - Class with console test program for the Model.