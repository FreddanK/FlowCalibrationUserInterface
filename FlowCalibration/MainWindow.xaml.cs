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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += NavigateToStartPage;
        }

        private void NavigateToStartPage(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(new ControlPage(0));
        }

        private void Exit_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void StartPage_MenuItem_Click(object sender, RoutedEventArgs e) => NavigateToStartPage(sender, e);
    }
}
