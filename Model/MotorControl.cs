using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Model
{  
    public class MotorControl
	// MotorControl - Controls the motor with a list of position and times or velocity and times. 
	//     		      Can convert from position to ticks or velocity to ticks per second and the other way.
	//				  Implements homing sequence.
    //                Needs to have the pulley wheel size as parameter.
    //                Assumes to get an initialized modbus object when created
    {
        ModbusCommunication ModCom { get; set; }

        public List<Double> RecordedTimes { get; set; }
        public List<Double> RecordedPositions { get; set; }
        public List<Double> RecordedVelocities { get; set; }
        public List<Double> RecordedTorques { get; set; }
        public List<Double> RecordedPressures { get; set; }

        struct Hardware
        {
            //DEFINE HARDWARE PARAMETERS
            public const Double Pitch = 32; // [mm] of gearwheel
            public const Double TicksPerRev = 4096; // [ticks per revolution position data]
            public const Double VelocityResolution = 16; // [velocity resolution is position resolution / constant]
            public const Double TimePerSecond = 2000; // [time register +2000 each second]
            public const Double MotorTorquePerTorque = 1000; // [motor Torue [mNm] per Torque [Nm]]
            public const Double PressureGain = 1; // [motor Pressure [VDC] to Pressure [?] gain]
            public const Double PressureBias = 0; // [motor Pressure [VDC] to Pressure [?] bias]
        }
        public struct Register
        {
            // DEFINE REGISTERS 
            // REGISTER: 450/451 (int32)
            public const ushort TargetInput = 450;
            // REGISTER: 200/201
            public const ushort Position = 200;
            // REGISTER: 202 
            public const ushort Speed = 202;
            // REGISTER: 203 
            public const ushort Torque = 203;
            // REGISTER: 420/421
            public const ushort Time = 420;
            // REGISTER: 170-173
            public const ushort Pressure = 170;
			// REGISTER: 152 OutputControl 3
			public const ushort OutputControl3 = 152;
			// REGISTER: 163 Output 3
			public const ushort Output3 = 162;
            // REGISTER: 353 
            public const ushort Acceleration = 353;
            // REGISTER: 354 
            public const ushort Deacceleration = 354;
            // PositionRamp (Mode 21): Closed control of position with ramp control.
            // SpeedRamp (Mode 33): Speed control mode with ramp control.
            // Shutdown (Mode 4)
            public const ushort Mode = 400;
            // REGISTER: 204: Max torque allowed
			public const ushort MotorTorqueMax = 204;
            // REGISTER: 410: current Status
			public const ushort Status = 410;
 

            // 20 possible events. Mapped by increase of 20.... I guess
            // goes from 680 to 699
            public const ushort EventControl = 680;
            // Event Target Register 
            // goes from 700 to 719
            public const ushort EventTrgReg = 700;
            // Event Target Data
            // goes from 720 to 739
            public const ushort EventTrgData = 720;
            // Event Source Register
            // goes from 740 to 759
            public const ushort EventSrcReg = 740;
            // Event Source Data
			// goes from 760 to 779
			public const ushort EventSrcData = 760;
            // Event Destination Register
			// goes from 780 to 799
			public const ushort EventDstReg = 780;

        }
        struct Mode
        {
            public const Int16 PositionRamp = 21;
            public const Int16 SpeedRamp = 33;
            public const Int16 Shutdown = 4;
            public const Int16 MotorOff = 0;
            public const Int16 Beep = 60;
        }

        struct EventLogic
        {
            /* Note that only bits 0-3 of Register.EventControl is considered here.
            0    Always      true
            1   =       Equal
            2   !=      Not equal
            3   <       Less than
            4   >       Greater than
            5   or      Bitwise or
            6   nor     Bitwise not or
            7   and     Bitwise and
            8   nand    Bitwise not and
            9   xor     Bitwise exclusive or
            10  nxor    Bitwise not exclusive or
            11  +       Add
            12  -       Subtract
            13  *       Multiply
            14  /       Divide
            15  Value   Takes data value directly
            */
        }

        public void CreateEvent(ushort EventNr,
                                Int16 TrgData,
                                Int16 TrgReg,
                                ushort EventLogic,
                                Int16 DstRegister,
                                ushort SrcData,
                                Int16 SrcRegister)
        {
            /* (ushort) EventNr, nr between 0-19 
             * (Int16)  TrgData, Value that is used for event triggers
             * (Int16)  TrgReg, Register that is read and used to trigger event
             * (ushort) EventLogic, 0XA--B where A is how the event should behave
             * and B is how the event should be triggered
             * (Int16) DstRegister, destination register
             * (ushort) SrcData, what should be written to the destination register
             * (Int16) SrcRegister, combined with SrcData
             */
            ModCom.RunModbus((ushort)(Register.EventTrgData + EventNr), TrgData);
            ModCom.RunModbus((ushort)(Register.EventTrgReg + EventNr), TrgReg);
            ModCom.RunModbus((ushort)(Register.EventControl + EventNr), EventLogic);
            ModCom.RunModbus((ushort)(Register.EventDstReg + EventNr), DstRegister);
            ModCom.RunModbus((ushort)(Register.EventSrcData + EventNr), SrcData);
            ModCom.RunModbus((ushort)(Register.EventSrcReg + EventNr),SrcRegister);

        }

        public MotorControl(ModbusCommunication modCom)
        {
            ModCom = modCom;

            RecordedTimes = new List<Double>();
            RecordedPositions = new List<Double>();
            RecordedVelocities = new List<Double>();
            RecordedTorques = new List<Double>();
            RecordedPressures = new List<Double>();

            // create event reading the maximum torque status register
            CreateEvent((ushort)0,
                                   (Int16)(0B000000000100000), //bitmask to get torque from status register
                                   (Int16)(MotorControl.Register.Status),
                                   (ushort)0XF007, // logical 'and' between bitmask and status register
                                   (Int16)(MotorControl.Register.Mode),
                                   (ushort)0,
                                   (Int16)0); //no source register
            
            // set a maximum allowed torque
            ModCom.RunModbus(MotorControl.Register.MotorTorqueMax, (Int16) 100);


			// make sure output register 1 is 0
			ModCom.RunModbus(MotorControl.Register.Output3, (Int16) 0);

            ModCom.RunModbus(MotorControl.Register.OutputControl3, (Int16)0);

			// Set output 1 high if target input is not 0
			CreateEvent((ushort) 1,
                        (Int16) 0,
                        (Int16) MotorControl.Register.TargetInput,
                        (ushort) 0XF005, // 0 or TargetInput as trigger, then write specified value
                        (Int16) MotorControl.Register.Output3,
                        (ushort) 1,
                        (Int16) 0); //no source
            
            // Set output 1 low if target input is 0.
			CreateEvent((ushort) 2,
            			(Int16) 0,
            			(Int16) MotorControl.Register.TargetInput,
            			(ushort) 0XF004, // 0 and TargetInput as trigger, then write specified value
            			(Int16) MotorControl.Register.Output3,
            			(ushort) 0,
                        (Int16) 0); //no source
		}

        public void RunWithPosition(List<Double> positions, List<Double> times)
        {
            List<Int32> ticks = PositionToTick(positions);
            RunTickSequence(ticks, times, Mode.PositionRamp);
        }

        public void RunWithVelocity(List<Double> velocities, List<Double> times)
        {
            List<Int32> ticks = VelocityToTicksPerSecond(velocities);
            RunTickSequence(ticks, times, Mode.SpeedRamp);
        }

        public void RunTickSequence(List<Int32> ticks, List<Double> times, Int16 mode)
        {
            if (ticks.Count() != times.Count())
            {
                throw new Exception("Input lists not of equal length");
            }

            int sequenceLength = ticks.Count();


            Stopwatch stopWatch = new Stopwatch();

            Int32 [] MotorRecordedTimes = new int[sequenceLength];
            Int32 [] MotorRecordedPositions = new Int32 [sequenceLength];
            Int32 [] MotorRecordedVelocities = new Int32 [sequenceLength];
            Int32 [] MotorRecordedTorques = new Int32 [sequenceLength];
            Int32 [] MotorRecordedPressures = new Int32 [sequenceLength];
            Double [] StopwatchRecordedTimes = new Double [sequenceLength];


            ModCom.RunModbus(Register.Mode, Mode.MotorOff);
            ModCom.RunModbus(Register.Position, (Int32)0);
            ModCom.RunModbus(Register.Speed, (Int32)0);
            ModCom.RunModbus(Register.TargetInput,(Int32)0);

            // Set mode
            ModCom.RunModbus(Register.Mode, mode);
            // Set time = 0
            ModCom.RunModbus(Register.Time, (Int32)0);
            stopWatch.Start();

            int i = 0;
            while(i < sequenceLength)
            {
                if (times[i] <= stopWatch.Elapsed.TotalSeconds)
                { // If more time have elapsed than time[i]
                    // Write target value
                    ModCom.RunModbus(Register.TargetInput,(Int32)ticks[i]);

                    // Read values that should be logged
                    //MotorRecordedTimes[i] = ModCom.ReadModbus(Register.Time, 2, false);
                    MotorRecordedPositions[i] = ModCom.ReadModbus(Register.Position, 2, true);
                    //MotorRecordedVelocities[i] = ModCom.ReadModbus(Register.Speed, 1, false);
                    StopwatchRecordedTimes[i] = stopWatch.Elapsed.TotalSeconds;
                    //MotorRecordedTorques[i] = ModCom.ReadModbus(Register.Torque, 1, false);
                    //MotorRecordedPressures[i] = ModCom.ReadModbus(Register.Pressure, 1, false);

                    i++;
                }
                
                //double overtime = 30;
                //if (stopWatch.Elapsed.TotalSeconds > overtime)
                //{
                //    Console.WriteLine("RunTickSequence running more than [overtime] seconds. Function stopped.");
                //    stopWatch.Stop();
                //    break;
                //}
            }
            // Set target to zero
            ModCom.RunModbus(Register.TargetInput, (Int32)0);

            ModCom.RunModbus(Register.Mode, Mode.MotorOff);

            stopWatch.Stop();
            //TODO if garbage collection was turned off, turn it on here

            // Convert units
            //RecordedTimes = TimeToSeconds(MotorRecordedTimes);
            RecordedPositions = TickToPosition(MotorRecordedPositions);
            RecordedVelocities = TicksPerSecondToVelocity(MotorRecordedVelocities);
            RecordedTorques = MotorTorquesToTorques(MotorRecordedTorques);
            RecordedPressures = MotorPressureToPressure(MotorRecordedPressures);
            RecordedTimes = StopwatchRecordedTimes.ToList<Double>();
        }

        public List<int> PositionToTick(List<Double> positions)
        {
            List<int> ticks = new List<int>();
            for (int i = 0; i < positions.Count; i++)
            {
                ticks.Add(-(int)Math.Round(positions[i] * 10 * Hardware.TicksPerRev / Hardware.Pitch));
            }
            return ticks;
        }

        public List<Int32> VelocityToTicksPerSecond(IList<Double> velocities)
        {
            List<Int32> ticks = new List<Int32>();
            for (int i = 0; i < velocities.Count; i++)
            {
                //TODO this conversion is the same as for position. Check so that it is correct.
                ticks.Add(-(int)Math.Round(velocities[i] * 10 * Hardware.TicksPerRev / Hardware.Pitch / Hardware.VelocityResolution));
            }
            return ticks;
        }

        public List<Double> TickToPosition(IList<int> ticks)
        {
            List<Double> position = new List<Double>();
            for (int i = 0; i < ticks.Count; i++)
            {
                position.Add(-(Double)ticks[i] * Hardware.Pitch / Hardware.TicksPerRev /10);
            }
            return position;
        }

        public List<Double> TicksPerSecondToVelocity(IList<int> ticksPerSecond)
        {
            List<Double> velocity = new List<Double>();
            for (int i = 0; i < ticksPerSecond.Count; i++)
            {
                velocity.Add(-(Double)ticksPerSecond[i] * Hardware.Pitch * Hardware.VelocityResolution/Hardware.TicksPerRev/10);
            }
            return velocity;
        }

        public List<Double> TimeToSeconds(IList<int> time)
        {
            List<Double> seconds = new List<Double>();
            for (int i = 0; i < time.Count; i++)
            {
                seconds.Add( (Double)time[i] / Hardware.TimePerSecond);
            }
            return seconds;
        }

        public List<Double> MotorTorquesToTorques(IList<int> motorTorques)
        {
            List<Double> torques = new List<Double>();
            for (int i = 0; i < motorTorques.Count; i++)
            {
                torques.Add((Double)motorTorques[i] / Hardware.MotorTorquePerTorque);
            }
            return torques;
        }
        public List<Double> MotorPressureToPressure(IList<int> motorPressure)
        {
            List<Double> pressure = new List<Double>();
            for (int i = 0; i < motorPressure.Count; i++)
            {
                pressure.Add((Double)motorPressure[i] * Hardware.PressureGain + Hardware.PressureBias);
            }
            return pressure;
        }

        public void ManualControl()
        {
            // Allows for small manual corrections of position using the motor
            Console.WriteLine("Manual control active");
			Console.WriteLine("Press enter to exit, + to increase and - to decrease position");
			int CurrentPosition = ModCom.ReadModbus(Register.Position, 2, true);
			int LastPosition = CurrentPosition;
			ConsoleKeyInfo input = new ConsoleKeyInfo();

			while (Console.ReadKey().Key != ConsoleKey.Enter)
			{
				input = Console.ReadKey(true);

				if (input.KeyChar == '+')
				{
					CurrentPosition = CurrentPosition + 50;
					Console.WriteLine("position increased");
				}
				if (input.KeyChar == '-')
				{
					CurrentPosition = CurrentPosition - 50;
					Console.WriteLine("position decreased");
				}
				if (CurrentPosition != LastPosition)
				{
					ModCom.RunModbus(Register.TargetInput, CurrentPosition);
					LastPosition = CurrentPosition;
				}

			}
			Console.WriteLine("Exiting manual control");
        }
    }
}
