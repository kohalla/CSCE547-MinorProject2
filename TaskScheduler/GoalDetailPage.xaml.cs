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
    /// Interaction logic for GoalDetailPage.xaml
    /// </summary>
    public partial class GoalDetailPage : Page
    {
		int pageGoalID = 0;
		int pageGoalCategory = 0;

		public GoalDetailPage(Goal targetGoal)
        {
            InitializeComponent();
			this.DataContext = targetGoal;

			pageGoalID = targetGoal.GoalID;
			pageGoalCategory = targetGoal.CategoryID;

			/*top section*/
			txtGoalTitle.Text = targetGoal.Title;

			//category drop down
			try
				{
					using (var ctx = new DataLayerContext())
					{
						var categories = ctx.Categories.Where(c => c.CategoryGroup == "Goal").ToList();
						cmbGoalCategory.ItemsSource = categories;
						// select existing category if present
						if (pageGoalCategory != 0)
							cmbGoalCategory.SelectedValue = pageGoalCategory;
					}
				}
				catch
				{
					// ignore load errors; ComboBox will be empty
				}

			txtDetail.Text = targetGoal.Description;

            //status drop down
            List <string>statusOptions = new List<string> { "Not Started", "In Progress", "Completed", "On Hold" };
            cmbGoalStatus.ItemsSource = statusOptions;
            if (!string.IsNullOrEmpty(targetGoal.Status))
            {
                cmbGoalStatus.SelectedItem = targetGoal.Status;
            }

			/*time frame*/
			//make default date  today and make sure end will not cause save errors
			if (targetGoal.GoalID == 0)
			{
				DateTime time = DateTime.Now;
				txtStartDate.Text = time.ToString();
				DateTime endTime = time.AddHours(1); // default to 1 hour later
				txtEndDate.Text = endTime.ToString();
			}
			else
			{
				txtStartDate.Text = targetGoal.StartDate.ToString();

				// make default end not cause save errors
				txtEndDate.Text = targetGoal.TargetDate.ToString();
			}

			/*progress section*/
			txtUnit.Text = targetGoal.Unit;
			// show 0 when null, otherwise the decimal's ToString()
			txtPercentComplete.Text = targetGoal.ProgressPercent?.ToString() ?? "0";
		
			// convert nullable decimal to decimal with default 0, then cast to double
			progressBar.Value = (double)(targetGoal.ProgressPercent ?? 0m);
			txtTarget.Text = targetGoal.TargetValue.ToString();

		}
		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				//call the user update function with the updated task
				((App)Application.Current).db.DeleteGoalbyID(pageGoalID);

				//reset the button
				SaveButton.IsEnabled = false;

				//navigate back to the task page
				this.NavigationService?.Navigate(new GoalsPage());
			}
			catch (Exception except)
			{
				MessageBox.Show($"Delete Error:\n{FormatException(except)}");
			}
		}
		private void OnTextboxTextChanged(object sender, TextChangedEventArgs e)
		{
			if (!this.IsLoaded) return;
			SaveButton.IsEnabled = true;
			// update progress bar when percent complete changes
			var source = e.OriginalSource as TextBox;
			// If the percent textbox changed, update the progress value
			if (source == txtPercentComplete)
			{
				if (decimal.TryParse(txtPercentComplete.Text, out decimal percent))
				{
					progressBar.Value = (double)percent;
				}
				else
				{
					progressBar.Value = 0;
				}
			}
			// If the target textbox changed, update the progress bar maximum so the visual reflects the new target
			else if (source == txtTarget)
			{
				if (decimal.TryParse(txtTarget.Text, out decimal t))
				{
					// ensure maximum is at least 1 to avoid zero-division visuals
					progressBar.Maximum = (double)Math.Max(t, 1m);
				}
				else
				{
					// fall back to the DataContext target value if parsing fails
					if (this.DataContext is DataLayer.Goal g)
						progressBar.Maximum = (double)g.TargetValue;
				}
			}
		}
		private string FormatException(Exception ex)
		{
			if (ex == null) return string.Empty;
			var sb = new System.Text.StringBuilder();
			var cur = ex;
			int depth = 0;
			while (cur != null && depth < 10)
			{
				sb.AppendLine(cur.Message);
				cur = cur.InnerException;
				depth++;
			}
			// include stack trace of the top-level exception for context
			sb.AppendLine();
			sb.AppendLine(ex.StackTrace ?? string.Empty);
			return sb.ToString();
		}
		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				//get the task from DataContext (or create a new one for add)
				var currentGoal = this.DataContext as DataLayer.Goal ?? new DataLayer.Goal();

				//populate from UI fields
				currentGoal.GoalID = pageGoalID;
				currentGoal.UserID = ((App)Application.Current).userid;

				// Category comes from selected ComboBox item if present
				if (cmbGoalCategory.SelectedValue != null)
				{
					currentGoal.CategoryID = (int)cmbGoalCategory.SelectedValue;
				}
				else
				{
					currentGoal.CategoryID = pageGoalCategory;
				}

				currentGoal.Title = txtGoalTitle.Text;

				currentGoal.Description = txtDetail.Text;

				currentGoal.Status = cmbGoalStatus.SelectedItem?.ToString() ?? "Not Started";

				//start and end date/time parsing
				if (!string.IsNullOrWhiteSpace(txtStartDate.Text))
				{
					if (DateTime.TryParse(txtStartDate.Text, out DateTime sdt))
						currentGoal.StartDate = sdt;
					else
						MessageBox.Show("Preferred Time Input Error: invalid format");
				}
				if (!string.IsNullOrWhiteSpace(txtEndDate.Text))
				{
					if (DateTime.TryParse(txtEndDate.Text, out DateTime edt))
						currentGoal.TargetDate = edt;
					else
						MessageBox.Show("Preferred Time Input Error: invalid format");
				}

			//progress
			currentGoal.Unit = txtUnit.Text;
			// parse target first so we can calculate CurrentValue from percent (ProgressPercent is computed in the DB)
			decimal target = 0m;
			if (decimal.TryParse(txtTarget.Text, out decimal parsedTarget))
			{
				target = parsedTarget;
				currentGoal.TargetValue = target;
			}
			else
			{
				MessageBox.Show("Target Value Input Error: invalid format");
			}

			if (decimal.TryParse(txtPercentComplete.Text, out decimal percent))
			{
				// ProgressPercent is marked computed in the model. Persist the equivalent CurrentValue
				// assuming percent is a percentage (0-100). CurrentValue = TargetValue * percent/100
				if (target == 0m)
				{
					// if target is zero or not parsed, fall back to using existing TargetValue on the DataContext
					if (this.DataContext is DataLayer.Goal g)
						target = g.TargetValue;
				}
				currentGoal.CurrentValue = (target * percent) / 100m;
				// also keep the textbox value in the in-memory object so UI remains consistent
				currentGoal.ProgressPercent = percent;
			}
			else
			{
				MessageBox.Show("Progress Percent Input Error: invalid format");
			}

				//update or add the goal
				if (pageGoalID > 0)
				{
					//update the goal
					((App)Application.Current).db.UpdateGoalbyID(((App)Application.Current).userid, pageGoalID, currentGoal);
				}
				else
				{
					//add the goal and get the new id
					int newId = ((App)Application.Current).db.AddGoal(currentGoal);
					if (newId > 0)
					{
						pageGoalID = newId;
						currentGoal.GoalID = newId;
					}
				}


				//reset the button
				SaveButton.IsEnabled = false;

				//navigate back to the goal page
				this.NavigationService?.Navigate(new GoalsPage());
			}
			catch (Exception except)
			{
				MessageBox.Show($"Save Error:\n{FormatException(except)}");
			}
		}
	}
}
