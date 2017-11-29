using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using Microsoft.Win32;

namespace FlowCalibration
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class ControlPage : Page
    {
        ControlPageViewModel ViewModel { get; set; }

        public ControlPage(int profileIndex)
        {
            InitializeComponent();
            ViewModel = new ControlPageViewModel();
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


        private void Parameter_TextBox_PreviewKeyUp(object sender, RoutedEventArgs e)
        {
            Double amplitude;
            Double frequency;
            Double samplingInterval;
            Double repeat;
            bool amplitudeOK = Double.TryParse(Amplitude_TextBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out amplitude);
            bool frequencyOK = Double.TryParse(Frequency_TextBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out frequency);
            bool samplingIntervalOK = Double.TryParse(SamplingInterval_TextBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out samplingInterval);
            bool repeatOK = Double.TryParse(Repeat_TextBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out repeat);

            if (amplitudeOK && frequencyOK && samplingIntervalOK && repeatOK && frequency > 0 && samplingInterval > 0.005 && repeat <= 10)
            {
                ViewModel.Amplitude = amplitude;
                ViewModel.Frequency = frequency;
                ViewModel.SamplingInterval = samplingInterval;
                ViewModel.Repeat = repeat;

                ViewModel.UpdateProfile();
            }

        }

        private void Run_Button_Click(object sender, RoutedEventArgs e)
        {
            //Thread runThread = new Thread(ViewModel.RunFlowProfile);
            //runThread.Name = "Run servo thread";
            //runThread.Start();
            ViewModel.RunFlowProfile();

        }

        private void LoadProfile_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            if( openDialog.ShowDialog() == true)
            {
                ViewModel.LoadProfile(openDialog.FileName);
            }

        }

        private void SaveProfile_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            if( saveDialog.ShowDialog() == true )
            {
                ViewModel.SaveProfile(saveDialog.FileName, ViewModel.ControlFlowPoints);
            }
        }

        private void Connect_Button_Click(object sender, RoutedEventArgs e)
        {
            String portName = COM_port_TextBox.Text;
            if(!ViewModel.USBConnected)
            {
                ViewModel.InitializeMotor(portName);
            }
        }

        private void SaveLoggedVolume_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            if( saveDialog.ShowDialog() == true )
            {
                ViewModel.SaveProfile(saveDialog.FileName, ViewModel.LogVolumePoints);
            }
        }

        private void SaveLoggedFlow_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            if( saveDialog.ShowDialog() == true )
            {
                ViewModel.SaveProfile(saveDialog.FileName, ViewModel.LogFlowPoints);
            }
        }
    }
}
