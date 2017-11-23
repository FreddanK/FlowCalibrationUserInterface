using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Model
{
    //WHAT UNITS IN LISTS RECORDED...

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

        static class Hardware
        {
            //DEFINE HARDWARE PARAMETERS
            public const double Pitch = 32; // [mm] of gearwheel
            public const double TicksPerRev = 4096; // [ticks per revolution position data]
            public const double VelocityResolution = 16; // [velocity resolution is position resolution / constant]
        }
        static class Register
        {
            // DEFINE REGISTERS 
            // REGISTER: 450/451 (int32)
            public const Int32 TargetInput = 450;
            // REGISTER: 200/201 (int32)
            public const Int32 Position = 200;
            // REGISTER: 202 (int16)
            public const Int16 Speed = 202;
            // REGISTER: 203 (int16)
            public const Int16 Torque = 203;
            // REGISTER: 420/421 (int32)
            public const Int32 Time = 420;
            // REGISTER: 170-173 (int16)
            public const Int16 Pressure = 170;
            // REGISTER: 353 (int16)
            public const Int16 Acceleration = 353;
            // REGISTER: 354 (int16)
            public const Int16 Deacceleration = 354;
            // PositionRamp (Mode 21): Closed control of position with ramp control.
            // SpeedRamp (Mode 33): Speed control mode with ramp control.
            // Shutdown (Mode 4)
            public const Int16 PositionRamp = 400;
            public const Int16 SpeedRamp = 400;
            public const Int16 Shutdown = 400;
            public const Int16 Mode = 400;
        }

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
            if (ticks.Count() != times.Count())
            {
                throw new Exception("Input lists not of equal length");
            }

            // SET MODE?

            Stopwatch stopWatch = new Stopwatch();

            //Write to MODBUS INIT, always time = 0

            int i = 1;

            stopWatch.Start();

            while (i < ticks.Count())
            {

                if (times[i] <= stopWatch.Elapsed.TotalSeconds)
                { // If more time have elapsed than time[i]
                    // WRITE TO MODUS WriteToRegister(writeRegister, targetvalues[i]); // write data
                    i += 1;
                }

                // READ DATA FROM MODBUS
                RecordedTimes.Add(ModCom.ReadModbus(Register.Time, false));
                RecordedPositions.Add(ModCom.ReadModbus(Register.Position, true));
                RecordedVelocities.Add(ModCom.ReadModbus(Register.Speed, false));

                double overtime = 30;
                if (stopWatch.Elapsed.TotalSeconds > overtime)
                {
                    Console.WriteLine("RunTickSequence running more than [overtime] seconds. Function stopped.");
                    stopWatch.Stop();
                    break;
                }
            }
            stopWatch.Stop();
        }

        public List<int> PositionToTick(List<Double> positions)
        {
            List<int> ticks = new List<int>();
            for (int i = 0; i < positions.Count; i++)
            {
                ticks[i] = (int)Math.Round(positions[i] * 10 * Hardware.TicksPerRev / Hardware.Pitch);
            }
            return ticks;
        }

        public List<Double> TickToPosition(List<int> ticks)
        {
            List<Double> position = new List<Double>();
            for (int i = 0; i < ticks.Count; i++)
            {
                position[i] = ticks[i] * Hardware.Pitch / Hardware.TicksPerRev /10;
            }
            return position;
            
        }

        public List<Double> TicksPerSecondToVelocity(List<int> ticksPerSecond)
        {
            List<Double> velocity = new List<Double>();
            for (int i = 0; i < ticksPerSecond.Count; i++)
            {
                velocity[i] = ticksPerSecond[i] * Hardware.Pitch * Hardware.VelocityResolution/Hardware.TicksPerRev/10;
            }
            return velocity;
        }
    }
}
