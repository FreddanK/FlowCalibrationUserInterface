using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    class MotorControl
	// MotorControl - Controls the motor with a list of position and times or velocity and times. 
	//     		      Can convert from position to ticks or velocity to ticks per second and the other way.
	//				  Implements homing sequence.
    //                Needs to have the pulley wheel size as parameter.
    //                Assumes to get an initialized modbus object when created
    {
        Double PulleyDiameter { get; set; }
        ModbusCommunication ModCom { get; set; }

        List<Double> RecordedTimes { get; set; }
        List<Double> RecordedPositions { get; set; }
        List<Double> RecordedVelocities { get; set; }

        public MotorControl(ModbusCommunication modCom)
        {
            ModCom = modCom;

            PulleyDiameter = 10; //TODO unit, value?

            RecordedTimes = new List<Double>();
            RecordedPositions = new List<Double>();
            RecordedVelocities = new List<Double>();
        }

        public void RunTickSequence(List<int> ticks, List<double> times)
        {

        }

        public List<int> PositionToTick(List<Double> positions)
        {
            return new List<int>();
        }

        public List<Double> TickToPosition(List<int> ticks)
        {
            return new List<Double>();
        }

        public List<Double> TicksPerSecondToVelocity(List<int> ticksPerSecond)
        {
            return new List<Double>();
        }
    }
}
