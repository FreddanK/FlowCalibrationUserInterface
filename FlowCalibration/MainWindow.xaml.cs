using System.Windows;

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
