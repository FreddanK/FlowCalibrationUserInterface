using OxyPlot;
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
        public ControlPageViewModel()
        {
            FlowProfileNames = new ObservableCollection<string>
            {
                "Sine", "Square", "Triangle", "Ramp", "Peaks", "Custom"
            };

            FlowPlotModel = new PlotModel { Title = "Flow Profile" };
            LoggerPlotModel = new PlotModel { Title = "Logged values" };


            LoggerPlotModel.Series.Add(new FunctionSeries(Math.Cosh, 0, 3, 0.1, "Flow (ml/s)"));
            LoggerPlotModel.Series.Add(new FunctionSeries(Math.Sinh, 0, 3, 0.1, "Volume (ml)"));


            Points = new ObservableCollection<DataPoint>();

            Amplitude = 1;

            Frequency = 1;

            SamplingInterval = 0.2;
        }

        public void UpdateProfile()
        {
            switch (CurrentProfileName)
            {
                case "Sine":
                    UpdatePlot(ProfileGenerator.Sine(Amplitude, Frequency, SamplingInterval));
                    return;
                case "Square":
                    UpdatePlot(ProfileGenerator.Square(Amplitude, Frequency, SamplingInterval));
                    return;
                case "Triangle":
                    UpdatePlot(ProfileGenerator.Triangle(Amplitude, Frequency, SamplingInterval));
                    return;
                case "Peaks":
                    UpdatePlot(ProfileGenerator.Peaks(Amplitude, Frequency, SamplingInterval));
                    return;
            }
        }

        private void UpdatePlot(LineSeries lineSeries)
        {
            FlowPlotModel.Series.Clear();
            FlowPlotModel.Series.Add(lineSeries);
            FlowPlotModel.InvalidatePlot(true);
            UpdateObservableCollectionFromIList(Points, lineSeries.Points);

        }

        private void UpdateObservableCollectionFromIList(ObservableCollection<DataPoint> observablePoints, IList<DataPoint> pointList)
        {
            observablePoints.Clear();
            foreach(DataPoint point in pointList)
            {
                observablePoints.Add(point);
            }
        }

        public PlotModel FlowPlotModel { get; private set; }

        public PlotModel LoggerPlotModel { get; private set; }

        public ObservableCollection<DataPoint> Points { get; private set; }

        public ObservableCollection<String> FlowProfileNames { get; private set; }

        public String CurrentProfileName { get; set; }

        public Double Amplitude { get; set; }

        public Double Frequency { get; set; }

        public Double SamplingInterval { get; set; }
    }
}
