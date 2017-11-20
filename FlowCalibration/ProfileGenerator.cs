using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCalibration
{
    static class ProfileGenerator
    {
        public static List<DataPoint> GetPeriodic(String funcName, Double amplitude, Double frequency, Double samplingInterval, Double repeat)
        {
            //amplitude (flow), frequency (rad/s), repeat (times)
            Func<Double, Double, Double, Double> mathFunction = Zero;
            switch (funcName)
            {
                case "Sine":
                    mathFunction = Sine;
                    break;
                case "Square":
                    mathFunction = Square;
                    break;
                case "Triangle":
                    mathFunction = Triangle;
                    break;
                case "Peaks":
                    mathFunction = Peaks;
                    break;
            }


            Double period = 2 * Math.PI / frequency;    //period (s)

            List<DataPoint> points = new List<DataPoint>();

            if (samplingInterval == 0) return points;
            if (frequency == 0) return points;

            for (Double x = 0; x <= period*repeat; x += samplingInterval)
            {
                Double y = mathFunction(x, amplitude, period);
                points.Add(new DataPoint(x, y));
            }

            List<DataPoint> allPeriods = new List<DataPoint>();

            return points;
        }

        public static Double Sine(Double x, Double amplitude, Double period)
        { 
            return amplitude * Math.Sin(x*2*Math.PI/period);
        }

        public static Double Square(Double x, Double amplitude, Double period)
        { 
            return amplitude * Math.Sign(Math.Sin(x*2 * Math.PI / period));
        }

        public static Double Triangle(Double x, Double amplitude, Double period)
        {   
            return (2 * amplitude / Math.PI) * Math.Asin(Math.Sin((2 * Math.PI * x) / period));
        }

        public static Double Peaks(Double x, Double amplitude, Double period)
        {   
            Double t1 = period / 4;
            Double y=0;
            if ((x%period) <= t1)
            {
                y = (2 * amplitude / Math.PI) * Math.Asin(Math.Sin((2 * Math.PI * x) / (period/2)));
            }
            else if ((x%period) > t1 && (x%period) <= t1*2)
            {
                y = 0;
            }
            else if ((x%period) > t1*2 && (x%period) <= t1 * 3)
            {
                y = -(2 * amplitude / Math.PI) * Math.Asin(Math.Sin((2 * Math.PI * x) / (period / 2)));
            }
            else if ((x%period) > t1*3)
            {
                y = 0;
            }
            return y;
        }

        public static Double Zero(Double x, Double amplitude, Double period)
        {
            return 0;
        }
    }
}
