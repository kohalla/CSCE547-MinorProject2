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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskScheduler
{
    /// <summary>
    /// Interaction logic for TaskPage.xaml
    /// </summary>
    public partial class TaskPage : Page
    {
		// When a row button is clicked we set this to true on preview mouse down
		// so the subsequent SelectionChanged handler can suppress navigation.
		private bool _suppressNavigation = false;

        public TaskPage()
        {
            List<DataLayer.Task> userTasks = ((App)Application.Current).db.GetUserTasks(((App)Application.Current).userid);

			InitializeComponent();

			userName.Content = ((App)Application.Current).db.GetUsernameByID(((App)Application.Current).userid);

			//daily tasks
			var userDailyTasks = ((App)Application.Current).db.SearchAllUserTaskbyCategory(((App)Application.Current).userid, "DailyTask");//no dates on task, so use default order
			
			AllDailyTasks.ItemsSource = userDailyTasks;

			//chores
			var userChores = ((App)Application.Current).db.SearchAllUserTaskbyCategory(((App)Application.Current).userid, "Chore");//no dates on task, so use default order

			AllChores.ItemsSource = userChores;

			//fun activities
			var userFunActivities = ((App)Application.Current).db.SearchAllUserTaskbyCategory(((App)Application.Current).userid, "FunActivity");//no dates on task, so use default order

			AllFunActivities.ItemsSource = userFunActivities;

			//completed tasks
			var userCompletedTasks = ((App)Application.Current).db.GetUserTaskCompletions(((App)Application.Current).userid);

			CompletedTasks.ItemsSource = userCompletedTasks;
		}
		/*Task Click Options*/
		private void AllDailyTasks_SelectionChanged(Object sender, SelectionChangedEventArgs e)
		{
			if (_suppressNavigation)
			{
				// clear flag and do not navigate when the click was on a button
				_suppressNavigation = false;
				return;
			}
			if (AllDailyTasks.SelectedItem != null)
			{
				DataLayer.Task selectedTask = (DataLayer.Task)AllDailyTasks.SelectedItem;
				TaskDetailPage taskDetails = new TaskDetailPage(selectedTask);
				this.NavigationService?.Navigate(taskDetails);
			}
		}
		private void AllChores_SelectionChanged(Object sender, SelectionChangedEventArgs e)
		{
			if (_suppressNavigation)
			{
				_suppressNavigation = false;
				return;
			}
			if (AllChores.SelectedItem != null)
			{
				DataLayer.Task selectedTask = (DataLayer.Task)AllChores.SelectedItem;
				TaskDetailPage taskDetails = new TaskDetailPage(selectedTask);
				this.NavigationService?.Navigate(taskDetails);
			}
		}
		private void AllFunActivities_SelectionChanged(Object sender, SelectionChangedEventArgs e)
		{
			if (_suppressNavigation)
			{
				_suppressNavigation = false;
				return;
			}
			if (AllFunActivities.SelectedItem != null)
			{
				DataLayer.Task selectedTask = (DataLayer.Task)AllFunActivities.SelectedItem;
				TaskDetailPage taskDetails = new TaskDetailPage(selectedTask);
				this.NavigationService?.Navigate(taskDetails);
			}
		}
		private void Click_AddTask(object sender, RoutedEventArgs e)
		{
			DataLayer.Task targetTask = new DataLayer.Task();
			TaskDetailPage taskDetails = new TaskDetailPage(targetTask);
			this.NavigationService?.Navigate(taskDetails);
		}
		private void CompleteButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			// set flag so SelectionChanged knows to skip navigation
			_suppressNavigation = true;
			
		}

		private void CompleteButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var btn = sender as Button;
				if (btn == null) return;
				var task = btn.DataContext as DataLayer.Task;
				if (task == null) return;
				TaskCompletion newCompletion = new TaskCompletion
				{
					TaskID = task.TaskID,
					CompletedAt = DateTime.Now,
					Notes = "Completion Note!"
				};
				((App)Application.Current).db.AddTaskCompletion(newCompletion);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Complete Error: {ex.Message}");
			}

			// Refresh the completed tasks list
			var userCompletedTasks = ((App)Application.Current).db.GetUserTaskCompletions(((App)Application.Current).userid);
			CompletedTasks.ItemsSource = userCompletedTasks;
			
		}
		private void CompletedTask_SelectionChanged(object sender, RoutedEventArgs e)
		{
			//show the task details
		}
	}
}
