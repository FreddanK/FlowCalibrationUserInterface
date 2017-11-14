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

namespace FlowCalibration
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
        }
        private void NavigateToControlPage(int profileIndex)
        {
            this.NavigationService.Navigate(new ControlPage(profileIndex));
        }

        private void Custom_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigateToControlPage(5);
        }

        private void Peaks_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigateToControlPage(4);
        }

        private void Triangle_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigateToControlPage(2);
        }

        private void Ramp_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigateToControlPage(3);
        }

        private void Square_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigateToControlPage(1);
        }

        private void Sine_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigateToControlPage(0);
        }
    }
}
