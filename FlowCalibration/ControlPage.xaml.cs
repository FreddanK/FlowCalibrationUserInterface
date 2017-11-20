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
 
            if (amplitudeOK) ViewModel.Amplitude = amplitude;
            if (frequencyOK && frequency > 0) ViewModel.Frequency = frequency;
            if (samplingIntervalOK && samplingInterval > 0.005) ViewModel.SamplingInterval = samplingInterval;
            if (repeatOK && repeat <= 10)  ViewModel.Repeat = repeat;

            ViewModel.UpdateProfile();
        }

        private void BaudRates_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Run_Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
