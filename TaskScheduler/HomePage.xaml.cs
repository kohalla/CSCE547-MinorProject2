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
	/// Interaction logic for HomePage.xaml
	/// </summary>
	public partial class HomePage : Page
	{
		public HomePage()
		{
			InitializeComponent();
			// Defer data loading until the page is loaded to avoid triggering database/network
			// activity during application startup. Load data on a background thread and
			// marshal results back to the UI thread.
			this.Loaded += HomePage_Loaded;
		}

		private async void HomePage_Loaded(object sender, RoutedEventArgs e)
		{
			// unsubscribe so this runs only once
			this.Loaded -= HomePage_Loaded;

			try
			{
				var app = (App)Application.Current;

				// run DB queries off the UI thread
				var data = await System.Threading.Tasks.Task.Run(() =>
				{
					var result = new
					{
						Username = app.db.GetUsernameByID(app.userid),
						Goals = app.db.GetUserGoals(app.userid).OrderBy(g => g.StartDate).ToList(),
						Appointments = app.db.GetUserAppointments(app.userid).OrderBy(a => a.StartDateTime).ToList(),
						DailyTasks = app.db.SearchAllUserTaskbyCategory(app.userid, "DailyTask"),
						Chores = app.db.SearchAllUserTaskbyCategory(app.userid, "Chore"),
						FunActivities = app.db.SearchAllUserTaskbyCategory(app.userid, "FunActivity")
					};

					return result;
				});

				// update UI safely
				username.Content = data.Username ?? string.Empty;

				int targetGoalLength = 10;
				int safeGoalLength = Math.Min(targetGoalLength, data.Goals.Count);
				Top10Goals.ItemsSource = data.Goals.GetRange(0, safeGoalLength);

				int targetAppointmentLength = 3;
				var futureAppointments = data.Appointments.Where(a => a.StartDateTime >= DateTime.Now).ToList();
				int safeAppointmentLength = Math.Min(targetAppointmentLength, futureAppointments.Count);
				Top3Appointments.ItemsSource = futureAppointments.GetRange(0, safeAppointmentLength);

			// Highlight appointment dates on the calendar 
			try
			{
				var cal = this.FindName("AppointmentsCalendar") as System.Windows.Controls.Calendar;
				if (cal != null)
				{
					cal.SelectedDates.Clear();
					var appointmentDates = futureAppointments.Where(a => a.StartDateTime.Date >= DateTime.Today).Select(a => a.StartDateTime.Date).Distinct();
					foreach (var date in appointmentDates)
					{
						cal.SelectedDates.Add(date);
					}
				}
			}
			catch
			{
				// ignore calendar highlighting errors to avoid breaking the rest of the page
			}

				int targetTaskLength = 3;
				TopDailyTasks.ItemsSource = data.DailyTasks.Take(targetTaskLength).ToList();
				TopChores.ItemsSource = data.Chores.Take(targetTaskLength).ToList();
				TopFunActivities.ItemsSource = data.FunActivities.Take(targetTaskLength).ToList();
			}
			catch (Exception ex)
			{
				// Log or show a friendly message; avoid throwing during page load.
				System.Diagnostics.Debug.WriteLine("HomePage load error: " + ex.Message);
				// Leave controls empty on error so the rest of the UI can function.
			}
		}
		/*Goal Click Functions*/
		private void Top10Goals_SelectionChanged(Object sender, SelectionChangedEventArgs e)
		{
			if(Top10Goals.SelectedItem !=null)
			{
				Goal selectedGoal = (Goal)Top10Goals.SelectedItem;
				GoalDetailPage goalDetails = new GoalDetailPage(selectedGoal);
				this.NavigationService?.Navigate(goalDetails);
			}
		}
		
		private void Click_MoreGoals(object sender, RoutedEventArgs e)
		{
			GoalsPage goalsPage = new GoalsPage();
			this.NavigationService?.Navigate(goalsPage);
		}

		private void Click_AddGoals(object sender, RoutedEventArgs e)
		{
			Goal targetGoal = new Goal();
			GoalDetailPage goalDetails = new GoalDetailPage(targetGoal);
			this.NavigationService?.Navigate(goalDetails);

		}
		/*Appointment Click Options*/
		private void Top3Appointments_SelectionChanged(Object sender, SelectionChangedEventArgs e)
		{
			if (Top3Appointments.SelectedItem != null)
			{
				Appointment selectedAppointment = (Appointment)Top3Appointments.SelectedItem;
				AppointmentDetailsPage appointmentDetails = new AppointmentDetailsPage(selectedAppointment);
				this.NavigationService?.Navigate(appointmentDetails);
			}
		}

		private void Click_MoreAppointments(object sender, RoutedEventArgs e)
		{
			AppointmentPage appointmentPage = new AppointmentPage();
			this.NavigationService?.Navigate(appointmentPage);
		}

		private void Click_AddAppointment(object sender, RoutedEventArgs e)
		{
			Appointment targetAppointment = new Appointment();
			AppointmentDetailsPage appointmentDetails = new AppointmentDetailsPage(targetAppointment);
			this.NavigationService?.Navigate(appointmentDetails);
		}

		/*Task Click Options*/
		private void Top3DailyTasks_SelectionChanged(Object sender, SelectionChangedEventArgs e)
		{
			if (TopDailyTasks.SelectedItem != null)
			{
				DataLayer.Task selectedTask = (DataLayer.Task)TopDailyTasks.SelectedItem;
				TaskDetailPage taskDetails = new TaskDetailPage(selectedTask);
				this.NavigationService?.Navigate(taskDetails);
			}
		}
		private void TopChores_SelectionChanged(Object sender, SelectionChangedEventArgs e)
		{
			if (TopChores.SelectedItem != null)
			{
				DataLayer.Task selectedTask = (DataLayer.Task)TopChores.SelectedItem;
				TaskDetailPage taskDetails = new TaskDetailPage(selectedTask);
				this.NavigationService?.Navigate(taskDetails);
			}
		}
		private void TopFunActivities_SelectionChanged(Object sender, SelectionChangedEventArgs e)
		{
			if (TopFunActivities.SelectedItem != null)
			{
				DataLayer.Task selectedTask = (DataLayer.Task)TopFunActivities.SelectedItem;
				TaskDetailPage taskDetails = new TaskDetailPage(selectedTask);
				this.NavigationService?.Navigate(taskDetails);
			}
		}


		private void Click_MoreTasks(object sender, RoutedEventArgs e)
		{
			TaskPage taskPage = new TaskPage();
			this.NavigationService?.Navigate(taskPage);
		}

		private void Click_AddTask(object sender, RoutedEventArgs e)
		{
			DataLayer.Task targetTask = new DataLayer.Task();
			TaskDetailPage taskDetails = new TaskDetailPage(targetTask);
			this.NavigationService?.Navigate(taskDetails);
		}
	}
}
