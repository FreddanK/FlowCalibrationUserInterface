using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OxyPlot;
using OxyPlot.Series;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FlowCalibration
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class ControlPage : Page
    {
        public ControlPage(int profileIndex)
        {
            InitializeComponent();
            ViewModel = new ViewModel();
            DataContext = ViewModel;

            ViewModel.CurrentProfileName = ViewModel.FlowProfileNames[profileIndex];
            Profiles_ComboBox.SelectedIndex = profileIndex;
            ViewModel.UpdateProfile();
        }

        private void Profiles_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.CurrentProfileName = Profiles_ComboBox.SelectedItem.ToString();
            ViewModel.UpdateProfile();     
        }

        public ViewModel ViewModel { get; private set; }

        private void Parameter_TextBox_PreviewKeyUp(object sender, RoutedEventArgs e)
        {
            Double amplitude;
            Double frequency;
            Double samplingInterval;
            bool amplitudeOK = Double.TryParse(Amplitude_TextBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out amplitude);
            bool frequencyOK = Double.TryParse(Frequency_TextBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out frequency);
            bool samplingIntervalOK = Double.TryParse(SamplingInterval_TextBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out samplingInterval);
 
            if (amplitudeOK) ViewModel.Amplitude = amplitude;
            if (frequencyOK) ViewModel.Frequency = frequency;
            if (samplingIntervalOK && samplingInterval > 0) ViewModel.SamplingInterval = samplingInterval;

            ViewModel.UpdateProfile();
        }
    }

    public class ViewModel
    {
        public ViewModel()
        {
            FlowProfileNames = new ObservableCollection<string>
            {
                "Sine", "Square", "Triangle", "Ramp", "Peaks", "Custom"
            };

            FlowPlotModel = new PlotModel { Title = "Flow Profile" };

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

        public ObservableCollection<DataPoint> Points { get; private set; }

        public ObservableCollection<String> FlowProfileNames { get; private set; }

        public String CurrentProfileName { get; set; }

        public Double Amplitude { get; set; }

        public Double Frequency { get; set; }

        public Double SamplingInterval { get; set; }
    }

    public class ProfileGenerator
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
