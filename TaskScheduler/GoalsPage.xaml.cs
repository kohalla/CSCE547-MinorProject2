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
using DataLayer;

namespace TaskScheduler
{
    /// <summary>
    /// Interaction logic for GoalsPage.xaml
    /// </summary>
    public partial class GoalsPage : Page
    {
        public GoalsPage()
        {
			List<DataLayer.Goal> userGoals = ((App)Application.Current).db.GetUserGoals(((App)Application.Current).userid);
			InitializeComponent();
			
			UnstartedGoals.ItemsSource = userGoals.Where(g => g.Status == "Not Started").ToList();
			InProgressGoals.ItemsSource = userGoals.Where(g => g.Status == "In Progress").ToList();
			OnHoldGoals.ItemsSource = userGoals.Where(g => g.Status == "On Hold").ToList();
			CompletedGoals.ItemsSource = userGoals.Where(g => g.Status == "Completed").ToList();

        }
		/* data table click handlers */
		private void UnstartedGoals_SelectionChanged(Object sender, SelectionChangedEventArgs e)
		{
			if (UnstartedGoals.SelectedItem != null)
			{
				Goal selectedGoal = (Goal)UnstartedGoals.SelectedItem;
				GoalDetailPage goalDetails = new GoalDetailPage(selectedGoal);
				this.NavigationService?.Navigate(goalDetails);
			}
		}
		private void InProgressGoals_SelectionChanged(Object sender, SelectionChangedEventArgs e)
		{
			if (InProgressGoals.SelectedItem != null)
			{
				Goal selectedGoal = (Goal)InProgressGoals.SelectedItem;
				GoalDetailPage goalDetails = new GoalDetailPage(selectedGoal);
				this.NavigationService?.Navigate(goalDetails);
			}
		}
		private void OnHoldGoals_SelectionChanged(Object sender, SelectionChangedEventArgs e)
		{
			if (OnHoldGoals.SelectedItem != null)
			{
				Goal selectedGoal = (Goal)OnHoldGoals.SelectedItem;
				GoalDetailPage goalDetails = new GoalDetailPage(selectedGoal);
				this.NavigationService?.Navigate(goalDetails);
			}
		}
		private void CompletedGoals_SelectionChanged(Object sender, SelectionChangedEventArgs e)
		{
			if (CompletedGoals.SelectedItem != null)
			{
				Goal selectedGoal = (Goal)CompletedGoals.SelectedItem;
				GoalDetailPage goalDetails = new GoalDetailPage(selectedGoal);
				this.NavigationService?.Navigate(goalDetails);
			}
		}

		/*button click handlers */

		private void Click_AddGoals(object sender, RoutedEventArgs e)
		{
			Goal targetGoal = new Goal();
			GoalDetailPage goalDetails = new GoalDetailPage(targetGoal);
			this.NavigationService?.Navigate(goalDetails);

		}

	}
}
