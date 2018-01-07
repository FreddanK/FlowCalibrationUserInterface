using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Model
{
    class Program
    {
        static class Register
        {
            // DEFINE REGISTERS 
            // REGISTER: 450/451 (int32)
            public const ushort TargetInput = 450;
            // REGISTER: 200/201 (int32)
            public const ushort Position = 200;
            // REGISTER: 202 (int16)
            public const ushort Speed = 202;
            // REGISTER: 203 (int16)
            public const ushort Torque = 203;
            // REGISTER: 420/421 (int32)
            public const ushort Time = 420;
            // REGISTER: 170-173 (int16)
            public const ushort Pressure = 170;
            // REGISTER: 353 (int16)
            public const ushort Acceleration = 353;
            // REGISTER: 354 (int16)
            public const ushort Deacceleration = 354;
            // PositionRamp (Mode 21): Closed control of position with ramp control.
            // SpeedRamp (Mode 33): Speed control mode with ramp control.
            // Shutdown (Mode 4)
            public const ushort PositionRamp = 400;
            public const ushort SpeedRamp = 400;
            public const ushort Shutdown = 400;
            public const ushort Mode = 400;
            public const ushort MotorTorqueMax = 204;
            public const ushort Status = 410;
        }

        static class Mode
        {
            public const int PositionRamp = 21;
            public const int SpeedRamp = 33;
            public const int Shutdown = 4;
            public const int MotorOff = 0;
            public const int Beep = 60;
        }

        public static void Main(string[] args)
        {
            String portName = ModbusCommunication.GetSerialPortName();
            ModbusCommunication modCom = new ModbusCommunication(portName);
            modCom.RunModbus(Register.Mode, (Int16)1);
            modCom.RunModbus(Register.TargetInput, 0);
            modCom.RunModbus(Register.Mode, (Int16)21);

            //modCom.RunModbus(Register.TargetInput,0);
            //modCom.RunModbus(Register.Mode,(Int16)33);
            //modCom.RunModbus(Register.TargetInput,100);
            //Thread.Sleep(2000);
            //modCom.RunModbus(Register.TargetInput,2000);
            //Thread.Sleep(2000);
            //modCom.RunModbus(Register.TargetInput,8000);
            //Thread.Sleep(2000);
            //modCom.RunModbus(Register.TargetInput,30000);
            //Thread.Sleep(2000);
            //modCom.RunModbus(Register.TargetInput,2000);
            //Thread.Sleep(2000);
            //modCom.RunModbus(Register.TargetInput,0);
            //modCom.RunModbus(Register.Mode,(Int16)1);

            MotorControl motCon = new MotorControl(modCom);

            // test of event safety function
            int currentTorque = modCom.ReadModbus(Register.Torque, (ushort)1, false);
            Console.WriteLine("current Torque:");
            Console.WriteLine(currentTorque);

            motCon.CreateEvent((ushort)0,
                                (Int16)(0B000000000100000), //bitmask to get torque from status register
                                (Int16)(Register.Status),
                                (ushort)0XF007, // and between bitmask and status register
                                (Int16)(Register.Mode),
                                (ushort)0,
                                (Int16)0); //no source register

            modCom.RunModbus(Register.MotorTorqueMax, (Int16)100);

            int dummieRead;
            Double[] RecordedTimes1 = new Double[100];
            Double[] RecordedTimes2 = new Double[100];
            Double[] RecordedTimesRead = new Double[100];
            Double[] RecordedTimesWrite = new Double[100];
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i = 0; i < 99; i++)
            {
                RecordedTimes1[i] = stopWatch.Elapsed.TotalSeconds;
                dummieRead = modCom.ReadModbus(Register.Position, 2, true);
                RecordedTimes2[i] = stopWatch.Elapsed.TotalSeconds;
            }
            for (int i = 0; i < 100; i++)
            {
                RecordedTimesRead[i] = RecordedTimes2[i] - RecordedTimes1[i];
            }
            for (int i = 0; i < 100; i++)
            {
                RecordedTimes1[i] = stopWatch.Elapsed.TotalSeconds;
                modCom.RunModbus((ushort)450, (int)0);
                RecordedTimes2[i] = stopWatch.Elapsed.TotalSeconds;
            }
            for (int i = 0; i < 100; i++)
            {
                RecordedTimesWrite[i] = RecordedTimes2[i] - RecordedTimes1[i];
            }
            Console.WriteLine("Read: Max: {0}, Min: {1}, Avr: {2}", RecordedTimesRead.Max(), RecordedTimesRead.Min(), RecordedTimesRead.Average());
            Console.WriteLine("Write: Max: {0}, Min: {1}, Avr: {2}", RecordedTimesWrite.Max(), RecordedTimesWrite.Min(), RecordedTimesWrite.Average());

            List<Int32> ticks = new List<Int32>() { 0, 100, 1000, 2000, 3000, 2000, 1000, 100, 0 };
            //List<Int32> ticks = new List<Int32>() {0,2000,4000,8000,4000,500,-2000,-2000,0};
            List<double> times = new List<double>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

            motCon.RunTickSequence(ticks, times, Mode.PositionRamp);
            //motCon.ManualControl();
            //Console.ReadLine();
            modCom.EndModbus();

            Console.ReadLine();

        }
    }
}
