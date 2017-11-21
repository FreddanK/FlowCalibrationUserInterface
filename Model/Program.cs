using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

// Change: Read from register
// Change: Write to register
// Change: How to do when multiple registers
// Note: Delete all Set...
// Note: Keep SetMode?


namespace Model
{
    class Program
    {
        static class Register
        {
            // DEFINE REGISTERS 
            // REGISTER: 450/451 (int32)
            public const int TargetInput = 450;
            // REGISTER: 200/201 (int32)
            public const int Position = 200;
            // REGISTER: 202 (int16)
            public const int Speed = 202;
            // REGISTER: 203 (int16)
            public const int Torque = 203;
            // REGISTER: 420/421 (int32)
            public const int Time = 420;
            // REGISTER: 170-173 (int16)
            public const int Pressure = 170;
            // REGISTER: 353 (int16)
            public const int Acceleration = 353;
            // REGISTER: 354 (int16)
            public const int Deacceleration = 354;
            // PositionRamp (Mode 21): Closed control of position with ramp control.
            // SpeedRamp (Mode 33): Speed control mode with ramp control.
            // Shutdown (Mode 4)
            public const int PositionRamp = 400;
            public const int SpeedRamp = 400;
            public const int Shutdown = 400;
        }
        static class Hardware
        {
            //DEFINE HARDWARE PARAMETERS
            public const double Pitch = 32; // [mm] of gearwheel
            public const double Diameter = 34; // [mm] of syringe
            public const double Area = Diameter * Diameter / 400 * Math.PI; // [cm^2] of syringe
        }
        static class Mode
        {
            public const int PositionRamp = 21;
            public const int SpeedRamp = 33;
            public const int Shutdown = 4;
            public const int MotorOff = 0;
        }

        //Data from Frontend to Backend
        
        public class Backend
        {
            struct MeasurementData
            {
                public double rpm;
                public double flow;
                public double volume;
                public double position;
                public double torque;
                public double pressure;
                public double time;
            };
            struct MotorMeasurementData
            {
                public double speed;
                public double position;
                public double torque;
                public double pressure;
                public double time;
            };

            // GET DATA
            static MeasurementData getMeasurements()
            {
                // Receives measurement data from motor, convert to other units and return them
                MeasurementData data;
                MotorMeasurementData motordata = getMotorMeasurements();
                data.rpm = MotorSpeedToRPM(motordata.speed);
                data.flow = MotorSpeedToFlow(motordata.speed);
                data.volume = MotorPositionToVolume(motordata.position);
                data.position = MotorPositionToPosition(motordata.position);
                data.torque = MotorTorqueToTorque(motordata.torque);
                data.pressure = MotorPressureToPressure(motordata.pressure);
                data.time = motordata.time;
                return data;
            }
            static MotorMeasurementData getMotorMeasurements()
            {
                // Receive measurement data from motor
                MotorMeasurementData data;
                data.speed = ReadFromRegister(Register.Speed);
                data.position = ReadFromRegister(Register.Position);
                data.torque = ReadFromRegister(Register.Torque);
                data.pressure = ReadFromRegister(Register.Pressure);
                data.time = ReadFromRegister(Register.Time);
                return data;
            }
            static double ReadFromRegister(int registerindex)
            {
                // READ FROM REGISTER INDEX
                double index = 1; //Dummie
                return index;
            }

            // Convert motor measured data to standard units
            static double MotorSpeedToRPM(double motorSpeed)
            {
                // Converting speed data from motor to RPM
                // Inputs:
                // motorSpeed (raw data from motor)
                double rpm = motorSpeed * 60 / 256;
                return rpm;
            }
            static double MotorSpeedToFlow(double motorSpeed)
            {
                // Converting speed data from motor to flow (ml/s)
                // Inputs:
                // motorSpeed (raw data from motor)

                // f = (motorSpeed * 60 / 256) / 60 * (pitch / 10) * area = (rev/min)*(min/s)*(cm/rev)*A = (cm/s * A)
                double f = motorSpeed * Hardware.Pitch * Hardware.Area / 2560;
                return f;
            }
            static double MotorPositionToVolume(double motorPosition)
            {
                // Converting position data (ticks) from motor to Volume (ml)
                // Inputs:
                // motorPosition (raw data from motor (ticks))

                // v = (motorPosition / 4096) * (pitch / 10) * Area = (Rotations * Length/Rotation * Area = Length * Area)
                double v = motorPosition * Hardware.Pitch * Hardware.Area / 40960;
                return v;
            }
            static double MotorTorqueToTorque(double motorTorque)
            {
                // Converting torque data (mNm) from motor to Torque (Nm)
                // Inputs:
                // motorTorque (raw data from motor (mNm))

                double t = motorTorque / 1000;
                return t;
            }
            static double MotorPressureToPressure(double motorPressureData)
            {
                // Converting pressure data (VDC [0,5]) from motor to pressure (N/m^2)
                // Inputs:
                // motorPressure (VDC)

                double pBias = 0; // Constant unwanted bias [VDC]
                double ampFactor = 1; // Linear relationship [N/m^2/VDC]
                double staticFactor = 0; // Static additive constant [N/m^2]

                double p = (motorPressureData - pBias) * ampFactor + staticFactor;
                return p;
            }
            static double MotorPositionToPosition(double motorPosition)
            {
                // Converting position data (ticks) from motor to position (cm)
                // Inputs:
                // motorPosition (raw data from motor (ticks))

                // p = (motorPosition / 4096) * (pitch / 10)  = (Rotations * Length/Rotation = Length)
                double p = motorPosition * Hardware.Pitch / 40960;
                return p;
            }

            // SEND & RECEIVE DATA
            static List<double> SendReceiveData(List<double> targetvalues, List<double> time, int writeRegister, int readRegister){
                // Send targetvalues to register "writeRegister" at specified times.
                // reads data from "readRegister"
            

                // abort if lists of different lengths
                if (targetvalues.Count() != time.Count()){
                    Console.WriteLine("SendTagetData() says: Lists not equal length");
                    Console.ReadLine(); // dummy to stop from executing
                }

                List <double> motorData = new List<double>(); // save data from motor

                Stopwatch stopWatch = new Stopwatch();
                Console.WriteLine("time: 0s");
                WriteToRegister(writeRegister,targetvalues[0]); //write initial target
                motorData.Add(ReadFromRegister(readRegister));  //read initial motor data

                int i = 1;
                stopWatch.Start();
                while (i < targetvalues.Count()){
                    
                    if (time[i] <= stopWatch.Elapsed.TotalSeconds){ // If more time have elapsed than time[i]
                        Console.WriteLine("time: {0}s", stopWatch.Elapsed.TotalSeconds);
                        WriteToRegister(writeRegister, targetvalues[i]); // write data
                        motorData.Add(ReadFromRegister(readRegister)); // read data
                        i += 1;
                    }

                    // in case of...
                    double overtime = 30;
                    if (stopWatch.Elapsed.TotalSeconds > overtime) {
                        Console.WriteLine("SendTagetData says: something is wrong");
                        stopWatch.Stop();
                        break;
                    }
                }
                stopWatch.Stop();
                return motorData;
            }

            static Tuple<List<double>, List<double>> SendReceiveData(List<double> targetvalues,List<double> time, int writeRegister, int readRegister1, int readRegister2)
            {
                // Send targetvalues to register "writeRegister" at specified times.
                // reads data from "readRegister"


                // abort if lists of different lengths
                if (targetvalues.Count() != time.Count())
                {
                    Console.WriteLine("SendTagetData() says: Lists not equal length");
                    Console.ReadLine(); // dummy to stop from executing
                }

                List<double> motorData1 = new List<double>(); // save data from motor
                List<double> motorData2 = new List<double>();
                Stopwatch stopWatch = new Stopwatch();
                Console.WriteLine("time: 0s");
                WriteToRegister(writeRegister, targetvalues[0]); //write initial target
                motorData1.Add(ReadFromRegister(readRegister1));  //read initial motor data
                motorData2.Add(ReadFromRegister(readRegister2));

                int i = 1;
                stopWatch.Start();
                while (i < targetvalues.Count())
                {

                    if (time[i] <= stopWatch.Elapsed.TotalSeconds)
                    { // If more time have elapsed than time[i]
                        Console.WriteLine("time: {0}s", stopWatch.Elapsed.TotalSeconds);
                        WriteToRegister(writeRegister, targetvalues[i]); // write data
                        motorData1.Add(ReadFromRegister(readRegister1)); // read data
                        motorData2.Add(ReadFromRegister(readRegister2));
                        i += 1;
                    }

                    // in case of...
                    double overtime = 30;
                    if (stopWatch.Elapsed.TotalSeconds > overtime)
                    {
                        Console.WriteLine("SendTagetData says: something is wrong");
                        stopWatch.Stop();
                        break;
                    }
                }
                stopWatch.Stop();
                return Tuple.Create(motorData1, motorData2);
            }

            // SEND DATA
            static void SetMotorRPM(double rpm)
            {
                // Converts RPM to motor speed and store in register
                // REGISTER: 202 (int16)
                double motorSpeedData = RPMToMotorSpeed(rpm);
                WriteToRegister(Register.Speed, motorSpeedData);
            }
            static void SetMotorFlow(double flow)
            {
                // Converts flow (ml/s) to motor speed and store in register
                // REGISTER: 202 (int16)
                double motorSpeedData = FlowToMotorSpeed(flow);
                WriteToRegister(Register.Speed, motorSpeedData);
            }
            static void SetMotorVolume(double volume)
            {
                // Converts volume (ml) to motor position and store in register
                // REGISTER: 200/201 (int32)
                double motorPosition = VolumeToMotorPosition(volume);
                WriteToRegister(Register.Position, motorPosition);
            }
            static void SetMotorMaxAcc(double acc)
            {
                // Converts acceleration (ml/s^2) to motor acceleration and store in register
                // REGISTER: 353/354 (int16/int16)

                double motorAcc = AccToMotorAcc(acc);

                // STORE IN REGISTERS 353 or 354
                if (motorAcc >= 0)
                { // Max acceleration
                    WriteToRegister(Register.Acceleration, motorAcc);
                }
                else
                { // Max deacceleration
                    motorAcc = -motorAcc; //Positive
                    WriteToRegister(Register.Deacceleration, motorAcc);
                }
            }

            // Convert standard units to motor units
            static double RPMToMotorSpeed(double rpm)
            {
                // Converting RPM to motor speed data
                // Inputs:
                // RPM
                double motorSpeed = rpm * 256 / 60;
                return motorSpeed;
            }
            static double FlowToMotorSpeed(double flow)
            {
                // Converting flow (ml/s) to motor speed data
                // Inputs:
                // flow (ml/s)

                double motorSpeed = flow * 2560 / Hardware.Pitch / Hardware.Area;
                return motorSpeed;
            }
            static double VolumeToMotorPosition(double volume)
            {
                // Converting volume (ml) to motor position data (ticks)
                // Inputs:
                // volume (ml)

                double motorPosition = volume * 40960 / Hardware.Pitch / Hardware.Area;
                return motorPosition;
            }
            static double TorqueToMotorTorque(double torque)
            {
                // Converting torque (Nm) to motor Torque (mNm)
                // Inputs:
                // motorTorque (raw data from motor (mNm))

                double motorTorque = 1000 * torque;
                return motorTorque;
            }
            static double AccToMotorAcc(double acc)
            {
                // Converting acceleration to motor accereration (pulses/s^2)/256
                // Inputs:
                // acc (m/s^2)
                double motorAccereration = acc * Hardware.Pitch * 4096000 / 256;
                return motorAccereration;
            }

            static List<double> FlowListToMotorSpeed(List<double> flow)
            {
                // Converting flow (ml/s) to motor speed data
                // Inputs:
                // flow (ml/s)
                for (int i = 1; i <= flow.Count; i++)
                {
                    flow[i-1] = FlowToMotorSpeed(flow[i-1]);
                }
                return flow;
            }

            static List<double> MotorSpeedListFlow(List<double> motorSpeed)
            {
                // Converting flow (ml/s) to motor speed data
                // Inputs:
                // flow (ml/s)
                for (int i = 1; i <= motorSpeed.Count; i++)
                {
                    motorSpeed[i - 1] = MotorSpeedToFlow(motorSpeed[i - 1]);
                }
                return motorSpeed;
            }

            static void WriteToRegister(int registerindex, double data)
            {
                // Writes data [data] in register [registerindex]
                //WRITE DATA TO INDEX
                Console.WriteLine("{0} written to register {1}",data,registerindex);
            }

            static void SetMode(int mode)
            {
                WriteToRegister(Register.PositionRamp, mode);
            }

            // Calculate velocity parameters
            static void LinearMovementToVelocity(double t0, double t1, double ft0, double ft1)
            {
                SetMode(Mode.SpeedRamp);                  // Set linear movement between velocities
                SetMotorMaxAcc(GetAcc(t0, t1, ft0, ft1)); //Acc to motor
                SetMotorFlow(ft1);                        //Speed to motor
            }

            static double GetAcc(double t0, double t1, double vt0, double vt1)
            {
                // Returns acceleration required (units/(s^2)) to bring the system from velocity vt0 at time t0 to vt1 at time t1
                // Inputs:
                // t0 (s)
                // t1 (s)
                // vt0 (units/s)
                // vt1 (units/s)
                if (t1 <= t0)
                {
                    //ERROR MESSAGE
                }
                double acc = (vt1 - vt0) / (t1 - t0);
                return acc;
            }
           
            static void Main(string[] args)
            {
                double motors = 100;
                double motorp = 10000;
                int motort = 4593;

                double ms = MotorSpeedToRPM(motors);
                double mf = MotorSpeedToFlow(motors);
                double mv = MotorPositionToVolume(motorp);
                double mt = MotorTorqueToTorque(motort);

                double cs = RPMToMotorSpeed(23.4375);
                double cf = FlowToMotorSpeed(11.3490034610931);
                double cv = VolumeToMotorPosition(70.931271631832);
                double ct = TorqueToMotorTorque(4.593);

                double t0 = 1;
                double t1 = 1.05;
                double vt0 = 4;
                double vt1 = 4.095;
                double ma = GetAcc(t0, t1, vt0, vt1);

                Console.WriteLine(" RPM: {0}, Volume: {1}, Flow: {2}, Torque: {3}, Acc: {4}, A: {5}", ms, mv, mf, mt, ma, Hardware.Area);
                Console.WriteLine(" RPM: {0}, Volume: {1}, Flow: {2}, Torque: {3}", cs, cv, cf, ct);

                int a = Register.Position;
                double b1 = ReadFromRegister(a);
                a = Register.Speed;
                double b2 = ReadFromRegister(a);
                a = Register.Torque;
                double b3 = ReadFromRegister(a);
                a = Register.Time;
                double b4 = ReadFromRegister(a);
                a = Register.Pressure;
                double b5 = ReadFromRegister(a);
                a = Register.Acceleration;
                double b6 = ReadFromRegister(a);
                a = Register.Deacceleration;
                double b7 = ReadFromRegister(a);
                a = Register.PositionRamp;
                double b8 = ReadFromRegister(a);
                a = Register.SpeedRamp;
                double b9 = ReadFromRegister(a);
                a = Register.Shutdown;
                double b10 = ReadFromRegister(a);
                a = 459040;
                double b11 = ReadFromRegister(a);
                Console.WriteLine(" b1: {0}, b2: {1}, b3: {2}, b4: {3}, b5: {4}, b6: {5}, b7: {6}, b8: {7}, b9: {8}, b10: {9}, b11: {10}", b1, b2, b3, b4, b5, b6, b7, b8, b9, b10, b11);

                MotorMeasurementData x = getMotorMeasurements();
                Console.WriteLine(" speed: {0}, pos: {1}, torque: {2}, time: {3}, pressure: {4}", x.speed, x.position, x.torque, x.time, x.pressure);

                MeasurementData xx = getMeasurements();
                Console.WriteLine(" flow: {0}, pos: {1}, torque: {2}, time: {3}, pressure: {4}, rpm: {5}, volume: {6}", xx.flow, xx.position, xx.torque, xx.time, xx.pressure, xx.rpm, xx.volume);

                double cc = RPMToMotorSpeed(60);
                Console.WriteLine(" RPM: {0}", cc);

                SetMode(Register.PositionRamp);
                SetMode(Register.SpeedRamp);
                SetMode(Register.Shutdown);
                SetMode(34534534);

                List<double> time = new List<double>(); // save data from motor
                List<double> flow = new List<double>(); // save data from motor

                time.Add(0.1);
                time.Add(0.2);
                time.Add(0.3);
                time.Add(0.4);

                flow.Add(1);
                flow.Add(3);
                flow.Add(2);
                flow.Add(4);

                flow = FlowListToMotorSpeed(flow);

                Console.WriteLine("time: {0} {1} {2} {3}, Speed: {4} {5} {6} {7}", time[0], time[1], time[2], time[3], flow[0], flow[1], flow[2], flow[3]);

                List<double> motorData = SendReceiveData(flow, time ,Register.TargetInput, Register.Position );
                Console.WriteLine("firstdata = {0}, secondData = {1}",motorData[0],motorData[1]);



                //Console.Title = Console.ReadLine();
            }
        }
    }
}
