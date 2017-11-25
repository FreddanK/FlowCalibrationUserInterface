using OxyPlot;
using Model;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCalibration
{
    class ControlPageViewModel
    {
        public ObservableCollection<DataPoint> LogFlowPoints { get; private set; }

        public ObservableCollection<DataPoint> LogVolumePoints { get; private set; }

        public ObservableCollection<DataPoint> ControlFlowPoints { get; private set; }

        public ObservableCollection<String> FlowProfileNames { get; private set; }
        
        public String CurrentProfileName { get; set; }

        public Double Amplitude { get; set; }
        public Double Frequency { get; set; }
        public Double SamplingInterval { get; set; }
        public Double Repeat { get; set; }
        public int R { get; set; }

        ModbusCommunication modCom;
        MotorControl motorControl;

        public ControlPageViewModel()
        {
            FlowProfileNames = new ObservableCollection<string>
            {
                "Sine", "Square", "Triangle", "Ramp", "Peaks", "Custom"
            };


            FunctionSeries points1 = new FunctionSeries(Math.Cosh, 0, 3, 0.1, "Flow (ml/s)");
            FunctionSeries points2 = new FunctionSeries(Math.Sinh, 0, 3, 0.1, "Volume (ml)");

            ControlFlowPoints = new ObservableCollection<DataPoint>();
            LogFlowPoints = new ObservableCollection<DataPoint>();
            LogVolumePoints = new ObservableCollection<DataPoint>();

            UpdateObservableCollectionFromIList(LogFlowPoints, points1.Points);
            UpdateObservableCollectionFromIList(LogVolumePoints, points2.Points);

            Amplitude = 1;
            Frequency = 1;
            SamplingInterval = 0.1;
            Repeat = 1;
            R = 1;

            modCom = new ModbusCommunication();
            motorControl = new MotorControl(modCom);
        }

        public void UpdateProfile()
        {
            List<DataPoint> points = ProfileGenerator.GetPeriodic(CurrentProfileName, Amplitude, Frequency, SamplingInterval, Repeat);
            UpdateObservableCollectionFromIList(ControlFlowPoints, points);
        }

        private void UpdateObservableCollectionFromIList(ObservableCollection<DataPoint> observablePoints, IList<DataPoint> pointList)
        {
            observablePoints.Clear();
            foreach(DataPoint point in pointList)
            {
                observablePoints.Add(point);
            }
        }

        public void RunFlowProfile()
        {
            List<Double> times = new List<Double>();
            List<Double> values = new List<Double>();

            foreach (DataPoint point in ControlFlowPoints)
            {
                times.Add(point.X);
                values.Add(point.Y);
            }

            motorControl.RunWithVelocity(values, times);

            // Backend.RunFlowValues(times,values);
        }
    }
}
