using System;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using Microsoft.Win32;

namespace FlowCalibration
{
    /// <summary>
    /// Interaction logic for ControlPage.xaml
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

            if (amplitudeOK && frequencyOK && samplingIntervalOK && repeatOK && frequency >= 0.1 && samplingInterval >= 0.04 && repeat <= 10)
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
            openDialog.Filter = "CSV file (*.csv)|*.csv";
            if ( openDialog.ShowDialog() == true)
            {
                Profiles_ComboBox.SelectedItem = "Custom";
                ViewModel.LoadProfile(openDialog.FileName);
            }

        }

        private void SaveProfile_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "CSV file (*.csv)|*.csv";
            saveDialog.DefaultExt = "csv";
            saveDialog.AddExtension = true;
            if ( saveDialog.ShowDialog() == true )
            {
                ViewModel.SaveProfile(saveDialog.FileName, ViewModel.ControlFlowPoints);
            }
        }

        private void Connect_Button_Click(object sender, RoutedEventArgs e)
        {
            String portName = COM_port_TextBox.Text;
            if (!ViewModel.USBConnected)
            {
                try
                {
                    ViewModel.InitializeMotor(portName);
                }
                catch (System.IO.IOException ex)
                {
                    MessageBox.Show(ex.Message,"Error message");
                }
            }
                
        }

        private void SaveLoggedVolume_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "CSV file (*.csv)|*.csv";
            saveDialog.DefaultExt = "csv";
            saveDialog.AddExtension = true;
            if ( saveDialog.ShowDialog() == true )
            {
                ViewModel.SaveProfile(saveDialog.FileName, ViewModel.LogVolumePoints);
            }
        }

        private void SaveLoggedFlow_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "CSV file (*.csv)|*.csv";
            saveDialog.DefaultExt = "csv";
            saveDialog.AddExtension = true;
            if ( saveDialog.ShowDialog() == true )
            {
                ViewModel.SaveProfile(saveDialog.FileName, ViewModel.LogFlowPoints);
            }
        }
    }
}
