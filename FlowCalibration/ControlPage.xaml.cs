﻿using System;
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

namespace FlowCalibration
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class ControlPage : Page
    {
        public ControlPage()
        {
            InitializeComponent();
            ViewModel = new ViewModel();
            DataContext = ViewModel;  
        }

        private void Profiles_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String SelectedProfileName = Profiles_ComboBox.SelectedItem.ToString();
            switch (SelectedProfileName)
            {
                case "Sine":
                    ViewModel.UpdatePlot(ProfileGenerator.Sine(1, 1, 0.1));
                    return;
                case "Square":  
                    ViewModel.UpdatePlot(ProfileGenerator.Square(1, 1, 0.1));
                    return;
            }
        }

        public ViewModel ViewModel { get; private set; }
    }

    public class ViewModel
    {
        public ViewModel()
        {
            FlowProfileNames = new ObservableCollection<string>
            {
                "Sine", "Square", "Triangle", "Ramp", "Breathing", "Custom"
            };

            FlowPlotModel = new PlotModel { Title = "Flow Profile" };

            Points = new ObservableCollection<DataPoint>();
        }

        public void UpdatePlot(LineSeries lineSeries)
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

        public ObservableCollection<string> FlowProfileNames { get; private set; }
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
