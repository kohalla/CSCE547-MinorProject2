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
    /// Interaction logic for TaskDetailPage.xaml
    /// </summary>
    public partial class TaskDetailPage : Page
    {
        int pageTaskID =0;
        int pageTaskCategory = 0;

        public TaskDetailPage(DataLayer.Task targetTask)
        {
            InitializeComponent();
            this.DataContext = targetTask;
            pageTaskID = targetTask.TaskID;
            pageTaskCategory = targetTask.CategoryID;

            // Get categories for the ComboBox
            try
            {
                using (var ctx = new DataLayerContext())
                {
                    var categories = ctx.Categories.Where(c => c.CategoryGroup == "Daily Task" || c.CategoryGroup == "Chore" || c.CategoryGroup == "FunActivity").ToList();
					cmbTaskCategory.ItemsSource = categories;
                    // select existing category if present
                    if (pageTaskCategory != 0)
                        cmbTaskCategory.SelectedValue = pageTaskCategory;
                }

        
            }
            catch
            {
                // ignore load errors; ComboBox will be empty
            }

            // disable Complete button for new (unsaved) tasks
            CompleteButton.IsEnabled = pageTaskID > 0;

            // enable Save when category selection changes
            cmbTaskCategory.SelectionChanged += (s, e) => { if (this.IsLoaded) SaveButton.IsEnabled = true; };

            //task priority
            if (pageTaskID == 0) { 
                txtPriority.Text = "1"; 
            } else { 
                txtPriority.Text = targetTask.Priority.ToString(); }
            

            //task
            txtTaskTitle.Text = targetTask.Title;

            //task details
            txtDetail.Text = targetTask.Description;

			//task frequency
			//txtFequency.Text = targetTask.Frequency;
			// Populate categories into the ComboBox
            var frequencyOptions = new List<string> { "Daily", "Weekly", "Monthly", "Weekdays", "Biweekly", "Once" };
			cmbTaskFrequency.ItemsSource = frequencyOptions;
			// select existing frequency if present
			if (!string.IsNullOrEmpty(targetTask.Frequency))
				cmbTaskFrequency.SelectedItem = targetTask.Frequency;
			// enable Save when frequency selection changes
			cmbTaskFrequency.SelectionChanged += (s, e) => { if (this.IsLoaded) SaveButton.IsEnabled = true; };


            // Populate user's goals into the ComboBox and select existing goal if present
            try
            {
                var goals = ((App)Application.Current).db.GetUserGoals(((App)Application.Current).userid);
                cmbGoal.ItemsSource = goals;
                if (targetTask.GoalID != null)
                {
                    cmbGoal.SelectedValue = targetTask.GoalID.Value;
                }
            }
            catch
            {
                // ignore load errors; ComboBox will be empty
            }
            cmbGoal.SelectionChanged += (s, e) => { if (this.IsLoaded) SaveButton.IsEnabled = true; };

            //duration
            txtDuration.Text = targetTask.DurationMinutes.ToString();

			//preffered day
			//txtDay.Text = targetTask.PreferredDay;
			var prefDayOptions = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
			cmbTaskPreferredDay.ItemsSource = prefDayOptions;
			// select existing preferred day if present
			if (!string.IsNullOrEmpty(targetTask.PreferredDay))
				cmbTaskPreferredDay.SelectedItem = targetTask.PreferredDay;
			// enable Save when preferred day selection changes
			cmbTaskPreferredDay.SelectionChanged += (s, e) => { if (this.IsLoaded) SaveButton.IsEnabled = true; };

			//preffered time
			if (targetTask.PreferredTime !=null){
                txtTime.Text = targetTask.PreferredTime.ToString();
             } else {
                txtTime.Text = "";
             }

            // flexibility and status (True/False dropdowns)
            try
            {
                // use string options instead of ComboBoxItem elements
                var boolOptions = new List<string> { "True", "False" };
                cmbFlexibility.ItemsSource = boolOptions;
                cmbStatus.ItemsSource = boolOptions;

                // set selected value for flexibility
                var flexValue = targetTask.IsFlexible ? "True" : "False";
                cmbFlexibility.SelectedItem = flexValue;

                // set selected value for status
                var statusValue = targetTask.IsActive ? "True" : "False";
                cmbStatus.SelectedItem = statusValue;
            }
            catch
            {
                // ignore if UI elements not present or casting fails
            }

            // enable save when these selections change
            cmbFlexibility.SelectionChanged += (s, e) => { if (this.IsLoaded) SaveButton.IsEnabled = true; };
            cmbStatus.SelectionChanged += (s, e) => { if (this.IsLoaded) SaveButton.IsEnabled = true; };
        }
        private void OnTextboxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.IsLoaded) return;
            SaveButton.IsEnabled = true;
        }

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
            try
            {
                //get the task from DataContext (or create a new one for add)
                var currentTask = this.DataContext as DataLayer.Task ?? new DataLayer.Task();

                //populate from UI fields
                currentTask.UserID = ((App)Application.Current).userid;

                // Category comes from selected ComboBox item if present
                if (cmbTaskCategory.SelectedValue != null)
                {
                    currentTask.CategoryID = (int)cmbTaskCategory.SelectedValue;
                }
                else
                {
                    currentTask.CategoryID = pageTaskCategory;
                }

                // Goal selected from ComboBox
                if (cmbGoal.SelectedValue != null)
                {
                    currentTask.GoalID = (int)cmbGoal.SelectedValue;
                }
                else
                {
                    currentTask.GoalID = null;
                }

                //get the goal id 
                currentTask.Title = txtTaskTitle.Text;
                currentTask.Description = txtDetail.Text;
				//currentTask.Frequency = txtFequency.Text;
				if (cmbTaskFrequency.SelectedValue != null)
				{
					currentTask.Frequency = cmbTaskFrequency.SelectedValue.ToString();
				}
				if (cmbTaskPreferredDay.SelectedValue != null)
                {
                    currentTask.PreferredDay = cmbTaskPreferredDay.SelectedValue.ToString();
                }
                if (!string.IsNullOrWhiteSpace(txtTime.Text))
                {
                    if (TimeSpan.TryParse(txtTime.Text, out TimeSpan pts))
                        currentTask.PreferredTime = pts;
                    else
                        MessageBox.Show("Preferred Time Input Error: invalid format");
                }
                if (int.TryParse(txtDuration.Text, out int dur)) currentTask.DurationMinutes = dur;
                if (byte.TryParse(txtPriority.Text, out byte pri)) currentTask.Priority = pri;
                // read flexibility and status from ComboBoxes
                if (cmbFlexibility.SelectedItem != null)
                {
                    var flexStr = (cmbFlexibility.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? cmbFlexibility.SelectedItem.ToString();
                    if (bool.TryParse(flexStr, out bool flex)) currentTask.IsFlexible = flex;
                }
                if (cmbStatus.SelectedItem != null)
                {
                    var actStr = (cmbStatus.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? cmbStatus.SelectedItem.ToString();
                    if (bool.TryParse(actStr, out bool act)) currentTask.IsActive = act;
                }

                //update or add the task
                if (pageTaskID > 0)
                {
                    //update the task
                    ((App)Application.Current).db.UpdateTaskbyID(((App)Application.Current).userid, pageTaskID, currentTask);
                }
                else
                {
                    //add the task and get the new id
                    int newId = ((App)Application.Current).db.AddTask(currentTask);
                    if (newId > 0)
                    {
                        pageTaskID = newId;
                        currentTask.TaskID = newId;
                        // enable Complete now that task is saved
                        CompleteButton.IsEnabled = true;
                    }
                }
                

                //reset the button
                SaveButton.IsEnabled = false;

				//navigate back to the task page
				this.NavigationService?.Navigate(new TaskPage());
			}
			catch (Exception except)
            {
                MessageBox.Show($"Save Error:\n{FormatException(except)}");
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
		private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
			try
			{
                //call the user update function with the updated task
				((App)Application.Current).db.DeleteTaskbyID(pageTaskID);

				//reset the button
				SaveButton.IsEnabled = false;

				//navigate back to the task page
				this.NavigationService?.Navigate(new TaskPage());
			}
            catch (Exception except)
            {
                MessageBox.Show($"Delete Error:\n{FormatException(except)}");
            }
            this.NavigationService?.Navigate(new TaskPage());
		}
        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (pageTaskID <= 0)
                {
                    MessageBox.Show("Cannot complete unsaved task.");
                    return;
                }

                TaskCompletion newCompletion = new TaskCompletion
                {
                    TaskID = pageTaskID,
                    CompletedAt = DateTime.Now,
                    Notes = "Completion Note!"
                };

                ((App)Application.Current).db.AddTaskCompletion(newCompletion);

				//navigate back to the task page
				this.NavigationService?.Navigate(new TaskPage());
			}
			catch (Exception ex)
            {
                MessageBox.Show($"Completion Error:\n{FormatException(ex)}");
            }

		}

		
    }
}
