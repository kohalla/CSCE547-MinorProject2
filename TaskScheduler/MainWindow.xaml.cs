using DataLayer;
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
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			((App)Application.Current).userid = 2;
			((App)Application.Current).adminUser = false;

			InitializeComponent();
			MainFrame.Navigate(new HomePage());

			// Setup responsive font scaling
			this.Loaded += (_, __) => UpdateFontSizes();
			this.SizeChanged += (_, __) => UpdateFontSizes();

			// Initialize menu visibility based on current user
			UpdateMenuVisibility();
		}

		private void UpdateMenuVisibility()
		{
			// Ensure the admin page menu item visibility matches the app-level flag
			if (mnuPage_Admin != null)
			{
				mnuPage_Admin.Visibility = ((App)Application.Current).adminUser ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		private void UpdateFontSizes()
		{
			double width = Math.Max(300.0, this.ActualWidth);
			double scale = Math.Max(0.8, Math.Min(1.6, width / 1000.0));

			double heading = 24.0 * scale;
			double sub = 18.0 * scale;
			double body = 14.0 * scale;
			double small = 12.0 * scale;

		// Calendar sizing derived from the same scale so calendars resize with window
		double dayCell = 36.0 * scale;
		double calW = Math.Max(240.0, width * 0.32);
		double calH = Math.Max(200.0, this.ActualHeight * 0.4);
		double calPad = Math.Max(2.0, 4.0 * scale);

			// Try to update the active theme dictionary first so DynamicResource lookups pick up the new values
			var themeDict = Application.Current?.Resources?.MergedDictionaries?.FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Theme"));
			if (themeDict != null)
			{
				themeDict["HeadingFontSize"] = heading;
				themeDict["SubheadingFontSize"] = sub;
				themeDict["BodyFontSize"] = body;
				themeDict["SmallFontSize"] = small;
			// update calendar sizing tokens as well
				themeDict["CalendarDayCellSize"] = dayCell;
				themeDict["CalendarWidth"] = calW;
				themeDict["CalendarHeight"] = calH;
				themeDict["CalendarDayCellPadding"] = calPad;
				return;
			}

			// Fallback: update application resources
			Application.Current.Resources["HeadingFontSize"] = heading;
			Application.Current.Resources["SubheadingFontSize"] = sub;
			Application.Current.Resources["BodyFontSize"] = body;
			Application.Current.Resources["SmallFontSize"] = small;
		// fallback calendar sizing
		Application.Current.Resources["CalendarDayCellSize"] = dayCell;
		Application.Current.Resources["CalendarWidth"] = calW;
		Application.Current.Resources["CalendarHeight"] = calH;
		Application.Current.Resources["CalendarDayCellPadding"] = calPad;
		}

		/* USER MENU NAVIGATION */
		/* These functions handle the click events for the user menu items and update the global variables for page access and data display*/
		private void mnuUser_Click_Admin(object sender, RoutedEventArgs e)
		{
			//set global user id and navigate to user page
			((App)Application.Current).userid = 1;
			((App)Application.Current).adminUser = true;
			MainFrame.Navigate(new HomePage());
			UpdateMenuVisibility();
		}
		private void mnuUser_Click_JM(object sender, RoutedEventArgs e)
		{
			//set global user id and navigate to user page
			((App)Application.Current).userid = 2;
			((App)Application.Current).adminUser = false;
			MainFrame.Navigate(new HomePage());
			UpdateMenuVisibility();
		}
		private void mnuUser_Click_PK(object sender, RoutedEventArgs e)
		{
			//set global user id and navigate to user page
			((App)Application.Current).userid = 3;
			((App)Application.Current).adminUser = false;
			MainFrame.Navigate(new HomePage());
			UpdateMenuVisibility();
		}
		private void mnuUser_Click_TB(object sender, RoutedEventArgs e)
		{
			//set global user id and navigate to user page
			((App)Application.Current).userid = 4;
			((App)Application.Current).adminUser = false;
			MainFrame.Navigate(new HomePage());
			UpdateMenuVisibility();
		}
		private void mnuUser_Click_RP(object sender, RoutedEventArgs e)
		{
			//set global user id and navigate to user page
			((App)Application.Current).userid = 5;
			((App)Application.Current).adminUser = true;
			MainFrame.Navigate(new HomePage());
			UpdateMenuVisibility();
		}

		/*Page Menu Navigation--view*/
		/* These functions handle clicking events from the page menu that contain "view" */
		private void mnuPage_Click_ViewTasks(object sender, RoutedEventArgs e)
		{
			MainFrame.Navigate(new TaskPage());
		}
		private void mnuPage_Click_ViewGoals(object sender, RoutedEventArgs e)
		{
			MainFrame.Navigate(new GoalsPage());
		}
		private void mnuPage_Click_ViewAppointments(object sender, RoutedEventArgs e)
		{
			MainFrame.Navigate(new AppointmentPage());
		}

		/*Page Menu Navigation--add*/
		/* These functions handle clicking events from the page menu that contain "add" */
		private void mnuPage_Click_AddTask(object sender, RoutedEventArgs e)
		{
			MainFrame.Navigate(new TaskDetailPage(new DataLayer.Task()));
		}
		private void mnuPage_Click_AddGoals(object sender, RoutedEventArgs e)
		{
			MainFrame.Navigate(new GoalDetailPage(new DataLayer.Goal()));
		}
		private void mnuPage_Click_AddAppointments(object sender, RoutedEventArgs e)
		{
			MainFrame.Navigate(new AppointmentDetailsPage(new DataLayer.Appointment()));
		}

		/*Page Menu Navigation--update*/
		/* These functions handle clicking events from the page menu that contain "edit" */

		private void mnuPage_Click_EditTask(object sender, RoutedEventArgs e)
		{
			//get a task from the list
			DataLayer.Task targetTask = ((App)Application.Current).db.GetUserTaskbyID(((App)Application.Current).userid, 1);
			//pass to the menu
			MainFrame.Navigate(new TaskDetailPage(targetTask));
		}
		private void mnuPage_Click_EditGoals(object sender, RoutedEventArgs e)
		{
			// get the first goal  from the list
			Goal targetGoal = ((App)Application.Current).db.GetUserGoalbyID(((App)Application.Current).userid, 1);

			//pass to the menu
			MainFrame.Navigate(new GoalDetailPage(targetGoal));
		}
		private void mnuPage_Click_EditAppointments(object sender, RoutedEventArgs e)
		{
			// get the first appointment from the list
			Appointment targetAppointment = ((App)Application.Current).db.GetUserAppointmentbyID(((App)Application.Current).userid, 1);

			//pass to the menu
			MainFrame.Navigate(new AppointmentDetailsPage(targetAppointment));
		}

		/*Page Menu Navigation--settings*/
		private void mnuPage_Click_UserSettings(object sender, RoutedEventArgs e)
		{
			MainFrame.Navigate(new SettingsPage());
		}
		private void mnuPage_Click_AdminPage(object sender, RoutedEventArgs e)
		{
			if (((App)Application.Current).adminUser) {
				MainFrame.Navigate(new AdminPage());
			}
		}
	}
}
