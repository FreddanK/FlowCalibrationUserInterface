using System;

public class BareMinimum
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

    static Tuple<List<double>, List<double>> SendReceiveData(List<double> targetvalues, List<double> time, int writeRegister, int readRegister1, int readRegister2)
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

    static void WriteToRegister(int registerindex, double data)
    {
        // Writes data [data] in register [registerindex]
        //WRITE DATA TO INDEX
        Console.WriteLine("{0} written to register {1}", data, registerindex);
    }

    static double ReadFromRegister(int registerindex)
    {
        // READ FROM REGISTER INDEX
        double index = 1; //Dummie
        return index;
    }

    static List<double> FlowListToMotorSpeed(List<double> flow)
    {
        // Converting flow (ml/s) to motor speed data
        // Inputs:
        // flow (ml/s)
        for (int i = 1; i <= flow.Count; i++)
        {
            flow[i - 1] = FlowToMotorSpeed(flow[i - 1]);
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

    static double MotorSpeedToFlow(double motorSpeed)
    {
        // Converting speed data from motor to flow (ml/s)
        // Inputs:
        // motorSpeed (raw data from motor)

        // f = (motorSpeed * 60 / 256) / 60 * (pitch / 10) * area = (rev/min)*(min/s)*(cm/rev)*A = (cm/s * A)
        double f = motorSpeed * Hardware.Pitch * Hardware.Area / 2560;
        return f;
    }

    static double FlowToMotorSpeed(double flow)
    {
        // Converting flow (ml/s) to motor speed data
        // Inputs:
        // flow (ml/s)

        double motorSpeed = flow * 2560 / Hardware.Pitch / Hardware.Area;
        return motorSpeed;
    }

    static void Main(string[] args)
    {
        double a = 2;
        Console.Title = Console.ReadLine();
    }
}
