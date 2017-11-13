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
            this.DataContext = this;

            this.PlotTitle = "Flow Profile";

            this.FlowProfilePlot = new PlotModel { Title = this.PlotTitle };
            LineSeries lineSeries = ProfileGenerator.Square(1, 1, 0.1);
            this.FlowProfilePlot.Series.Add(lineSeries);
            this.FlowProfilePlot.Series.Add(ProfileGenerator.Sine(1,1,0.5));

            this.Points = lineSeries.Points;




            //DataPointsListView.ItemsSource = this.Points;
        }

        private void BackToStartPageButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Page1());
        }

        public string PlotTitle { get; private set; }

        public PlotModel FlowProfilePlot { get; private set; }

        public IList<DataPoint> Points { get; private set; }
    }

    public class ProfileGenerator
    {
        public static LineSeries Sine(Double amplitude, Double frequency, Double samplingInterval)
        { //amplitude (flow), frequency (rad/s)
            Double period = 2 * Math.PI / frequency;    //period (s)

            LineSeries lineSeries = new LineSeries();
            lineSeries.Title = "Sine";

            for (Double x = 0; x <= period; x += samplingInterval)
            {
                Double y = amplitude * Math.Sin(x);
                lineSeries.Points.Add(new DataPoint(x, y));
            }

            return lineSeries;
        }

        public static LineSeries Square(Double amplitude, Double frequency, Double samplingInterval)
        { //amplitude (flow), frequency (rad/s)
            Double period = 2 * Math.PI / frequency;    //period (s)

            LineSeries lineSeries = new LineSeries{ Title = "Square" };

            for (Double x = 0; x <= period; x += samplingInterval)
            {
                Double y = amplitude * Math.Sign(Math.Sin(x));
                lineSeries.Points.Add(new DataPoint(x, y));
            }

            return lineSeries;
        }
    }
}
