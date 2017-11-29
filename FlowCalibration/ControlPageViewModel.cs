using OxyPlot;
using Model;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Data;

namespace FlowCalibration
{
    class ControlPageViewModel
    {
        public ObservableCollection<DataPoint> LogFlowPoints { get; private set; }

        public ObservableCollection<DataPoint> LogVolumePoints { get; private set; }

        public ObservableCollection<DataPoint> ControlFlowPoints { get; private set; }
        public ObservableCollection<Point> Points { get; private set; }

        public ObservableCollection<String> FlowProfileNames { get; private set; }

        public String CurrentProfileName { get; set; }

        public Double Amplitude { get; set; }
        public Double Frequency { get; set; }
        public Double SamplingInterval { get; set; }
        public Double Repeat { get; set; }

        
        public String RecordedProfile { get; set; }
        public String RecordedDateTime { get; set; }
        public Double RecordedMaxTime { get; set; }
        public Double RecordedMinFlow { get; set; }
        public Double RecordedMaxFlow { get; set; }
        public Double RecordedMinVolume { get; set; }
        public Double RecordedMaxVolume { get; set; }

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
            Points = new ObservableCollection<Point>();
            ControlFlowPoints = new ObservableCollection<DataPoint>();
            LogFlowPoints = new ObservableCollection<DataPoint>();
            LogVolumePoints = new ObservableCollection<DataPoint>();

            Amplitude = 1;
            Frequency = 1;
            SamplingInterval = 0.1;
            Repeat = 1;

            RecordedProfile = "2012";
            RecordedDateTime = "2012";
            RecordedMaxTime = 0;
            RecordedMinFlow = 0;
            RecordedMaxFlow = 0;
            RecordedMinVolume = 0;
            RecordedMaxVolume = 0;


        }

        public void UpdateProfile()
        {
            List<DataPoint> points = ProfileGenerator.GetPeriodic(CurrentProfileName, Amplitude, Frequency, SamplingInterval, Repeat);
            //UpdateObservableCollectionFromIList(ControlFlowPoints, points);
            UpdateFlowProfileFromIList(points);
        }

        private void UpdateObservableCollectionFromIList(ObservableCollection<DataPoint> observablePoints, IList<DataPoint> pointList)
        {
            observablePoints.Clear();
            foreach(DataPoint point in pointList)
            {
                observablePoints.Add(point);
            }
        }

        private void UpdateFlowProfileFromIList(IList<DataPoint> pointList)
        {
            ControlFlowPoints.Clear();
            Points.Clear();
            int i = 0;
            foreach(DataPoint point in pointList)
            {
                ControlFlowPoints.Add(point);
                Points.Add(new Point(point.X, point.Y, i, ControlFlowPoints));
                i++;
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

            RecordedProfile = CurrentProfileName;
        }

        public void InitializeMotor()
        {
            modCom = new ModbusCommunication();
            motorControl = new MotorControl(modCom);
        }

        public void SaveProfile(String filePath)
        {
            List<Double> times = new List<Double>();
            List<Double> values = new List<Double>();

            foreach (DataPoint point in ControlFlowPoints)
            {
                times.Add(point.X);
                values.Add(point.Y);
            }
            DataExporter.SaveTimeAndValuesToCsv(times, values, filePath);
        }

        public void LoadProfile(String filePath)
        {
            List<Double> times = new List<Double>();
            List<Double> values = new List<Double>();

            DataExporter.LoadTimeAndValuesFromCsv(times, values, filePath);

            List<DataPoint> dataPoints = new List<DataPoint>();

            for(int i=0; i<values.Count(); i++)
            {
                dataPoints.Add(new DataPoint(times[i], values[i]));
            }

            UpdateObservableCollectionFromIList(ControlFlowPoints, dataPoints);
        }
    }

    public class Point
    {
        public ObservableCollection<DataPoint> TrackedCollection { get; set; }
        double y;
        double x;
        public Double X
        {
            get { return x; }
            set
            {
                x = value;
                TrackedCollection[Index] = new DataPoint(x, y);
            }
        }
        public Double Y {
            get { return y; }
            set
            {
                y = value;
                TrackedCollection[Index] = new DataPoint(x, y);
            }
        }
        public int Index { get; set; }

        public Point(Double x, Double y, int i, ObservableCollection<DataPoint> t)
        {
            this.x = x;
            this.y = y;
            Index = i;
            TrackedCollection = t;
        }
    }

}
