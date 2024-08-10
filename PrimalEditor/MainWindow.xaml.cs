using PrimalEditor.GameProject;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrimalEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += onMainWindowLoaded;
            Closing += onMainWindowClosing;
        }

        private void onMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= onMainWindowLoaded;
            OpenProjectBrowserDialog();
        }

        private void onMainWindowClosing(object? sender, CancelEventArgs e)
        {
            Closing -= onMainWindowClosing;
            Project.Current?.Unload();
        }

        private void OpenProjectBrowserDialog()
        {
            var projectBrowser = new ProjectBrowserDialog();
            if (projectBrowser.ShowDialog() == false || projectBrowser.DataContext == null)
            {
                Application.Current.Shutdown();

            }
            else
            {
                Project.Current?.Unload();
                DataContext = projectBrowser.DataContext;

            }
        }
    }
}