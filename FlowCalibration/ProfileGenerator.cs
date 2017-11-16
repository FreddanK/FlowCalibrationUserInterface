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
        public static LineSeries Sine(Double amplitude, Double frequency, Double samplingInterval)
        { //amplitude (flow), frequency (rad/s)
            Double period = 2 * Math.PI / frequency;    //period (s)

            LineSeries lineSeries = new LineSeries();
            lineSeries.Title = "Sine";
            if (samplingInterval == 0) return lineSeries;
            for (Double x = 0; x <= period; x += samplingInterval)
            {
                Double y = amplitude * Math.Sin(x*2*Math.PI/period);
                lineSeries.Points.Add(new DataPoint(x, y));
            }

            return lineSeries;
        }

        public static LineSeries Square(Double amplitude, Double frequency, Double samplingInterval)
        { //amplitude (flow), frequency (rad/s)
            Double period = 2 * Math.PI / frequency;    //period (s)

            LineSeries lineSeries = new LineSeries{ Title = "Square" };
            if (samplingInterval == 0) return lineSeries;
            for (Double x = 0; x <= period; x += samplingInterval)
            {
                Double y = amplitude * Math.Sign(Math.Sin(x*2 * Math.PI / period));
                lineSeries.Points.Add(new DataPoint(x, y));
            }

            return lineSeries;
        }

        public static LineSeries Triangle(Double amplitude, Double frequency, Double samplingInterval)
        {   //amplitude (flow), frequency (rad/s)
            // https://en.wikipedia.org/wiki/Waveform
            Double period = 2 * Math.PI / frequency;    //period (s)

            LineSeries lineSeries = new LineSeries { Title = "Triangle" };
            if (samplingInterval == 0) return lineSeries;
            for (double x = 0; x <= period; x+= samplingInterval){
                double y = (2 * amplitude / Math.PI) * Math.Asin(Math.Sin((2 * Math.PI * x) / period));
                lineSeries.Points.Add(new DataPoint(x, y));

            }
            return lineSeries;
        }

        public static LineSeries Peaks(Double amplitude, Double frequency, Double samplingInterval)
        {   //amplitude (flow), frequency (rad/s)
            
            Double period = 2 * Math.PI / frequency;    //period (s)

            LineSeries lineSeries = new LineSeries { Title = "Peaks" };
            double t1 = period / 4;
            if (samplingInterval == 0) return lineSeries;
            for (double x = 0; x <= period; x += samplingInterval)
            {
                if (x <= t1)
                {
                    double y = (2 * amplitude / Math.PI) * Math.Asin(Math.Sin((2 * Math.PI * x) / (period/2)));
                    lineSeries.Points.Add(new DataPoint(x, y));
                }
                else if (x > t1 && x <= t1*2)
                {
                    double y = 0;
                    lineSeries.Points.Add(new DataPoint(x, y));
                }
                else if (x > t1*2 && x <= t1 * 3)
                {
                    double y = -(2 * amplitude / Math.PI) * Math.Asin(Math.Sin((2 * Math.PI * x) / (period / 2)));
                    lineSeries.Points.Add(new DataPoint(x, y));
                }
                else if (x > t1*3)
                {
                    double y = 0;
                    lineSeries.Points.Add(new DataPoint(x, y));
                }
            }
            return lineSeries;
        }
    }
}
