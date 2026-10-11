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

namespace TaskScheduler
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            // Initialize toggle state to reflect the current persisted theme
            try
            {
                ThemeToggle.IsChecked = TaskScheduler.Properties.Settings.Default.AppTheme == "Dark";
            }
            catch
            {
                // ignore if ThemeToggle is not available at design time
            }
        }

		private void ThemeToggle_Checked(object sender, RoutedEventArgs e)
		{
            var app = Application.Current as App;
            app?.ApplyTheme("Dark");
        }
        private void ThemeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            var app = Application.Current as App;
            app?.ApplyTheme("Light");
        }
	}
}
