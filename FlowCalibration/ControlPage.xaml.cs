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

namespace FlowCalibration
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class Page2 : Page
    {
        public Page2()
        {
            InitializeComponent();
            DataContext = this;
            Double dt = 0.1;
            FlowProfileGenerator flowProfileGenerator = new FlowProfileGenerator(dt);
            this.Points = flowProfileGenerator.SineProfile(1, 1);

          

            this.PlotTitle = "Example plot";
            //DataPointsListView.ItemsSource = this.Points;
        }

        private void BackToStartPageButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Page1());
        }

        public string PlotTitle { get; private set; }

        public IList<DataPoint> Points { get; private set; }
    }

    public class FlowProfileGenerator
    {
        public FlowProfileGenerator(Double samplingInterval)
        {
            this.SamplingInterval = samplingInterval;
        }

        public List<DataPoint> SineProfile(Double amplitude, Double frequency)
        { //amplitude (flow), frequency (rad/s)
            Double period = 2 * Math.PI / frequency;    //period (s)

            List<DataPoint> points = new List<DataPoint>();

            for( Double x = 0; x <= period; x += this.SamplingInterval)
            {
                Double y = amplitude * Math.Sin(x);
                points.Add(new DataPoint(x, y));
            } 
            
            return points;
        }

        public static List<DataPoint> SquareProfile(Double amplitude, Double frequency)
        {
            List<DataPoint> points = new List<DataPoint>();
            return points;
        }

        public static List<DataPoint> RampProfile(Double amplitude, Double frequency)
        {
            List<DataPoint> points = new List<DataPoint>();
            return points;
        }

        public static List<DataPoint> TriangleProfile(Double amplitude, Double frequency)
        {
            List<DataPoint> points = new List<DataPoint>();
            return points;
        }

        public static List<DataPoint> BreathingProfile(Double amplitude, Double frequency)
        {
            List<DataPoint> points = new List<DataPoint>();
            return points;
        }

        public Double SamplingInterval { get; set; } // Sampling interval (seconds)
    }
}
