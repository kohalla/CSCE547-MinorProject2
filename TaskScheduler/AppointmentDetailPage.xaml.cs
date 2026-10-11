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
    /// Interaction logic for AppointmentDetailsPage.xaml
    /// </summary>
    public partial class AppointmentDetailsPage : Page
    {
		int pageAppointmentID = 0;
		int pageAppointmentCategory = 0;
		public AppointmentDetailsPage(Appointment targetAppointment)
        {
            InitializeComponent();
            this.DataContext = targetAppointment;

			txtAppointmentTitle.Text = targetAppointment.Title;
			pageAppointmentID = targetAppointment.AppointmentID;
			pageAppointmentCategory = targetAppointment.CategoryID;

			// Get categories for the ComboBox
			try
			{
				using (var ctx = new DataLayerContext())
				{
					var categories = ctx.Categories.Where(c => c.CategoryGroup == "Appointment").ToList();
					cmbAppointmentCategory.ItemsSource = categories;
					// select existing category if present
					if (pageAppointmentCategory != 0)
						cmbAppointmentCategory.SelectedValue = pageAppointmentCategory;
				}


			}
			catch
			{
				// ignore load errors; ComboBox will be empty
			}

			txtLocation.Text = targetAppointment.Location;

			//make default date  today and make sure end will not cause save errors
			if (targetAppointment.AppointmentID == 0) {
				DateTime time= DateTime.Now;
				txtStart.Text = time.ToString();
				DateTime endTime = time.AddHours(1); // default to 1 hour later
				txtEnd.Text = endTime.ToString();
			}
			else
			{
				txtStart.Text = targetAppointment.StartDateTime.ToString();

				// make default end not cause save errors
				txtEnd.Text = targetAppointment.EndDateTime.ToString();
			}
			txtReminder.Text = targetAppointment.ReminderMinutesBefore.ToString();
			txtNotes.Text = targetAppointment.Notes;

			// enable Save when category selection changes
			cmbAppointmentCategory.SelectionChanged += (s, e) => { if (this.IsLoaded) SaveButton.IsEnabled = true; };
		}

		private void OnTextboxTextChanged(object sender, TextChangedEventArgs e)
		{
			if (!this.IsLoaded) return;
			SaveButton.IsEnabled = true;
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
				((App)Application.Current).db.DeleteAppointmentbyID(pageAppointmentID);

				//reset the button
				SaveButton.IsEnabled = false;

				//navigate back to the task page
				this.NavigationService?.Navigate(new AppointmentPage());
			}
			catch (Exception except)
			{
				MessageBox.Show($"Delete Error:\n{FormatException(except)}");
			}
		}
		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				//get the task from DataContext (or create a new one for add)
				var currentAppointment = this.DataContext as DataLayer.Appointment ?? new DataLayer.Appointment();

				//populate from UI fields
				currentAppointment.UserID = ((App)Application.Current).userid;

				// Category comes from selected ComboBox item if present
				if (cmbAppointmentCategory.SelectedValue != null)
				{
					currentAppointment.CategoryID = (int)cmbAppointmentCategory.SelectedValue;
				}
				else
				{
					currentAppointment.CategoryID = pageAppointmentCategory;
				}

				currentAppointment.Title = txtAppointmentTitle.Text;
				
				currentAppointment.Location = txtLocation.Text;

				//start and end date/time parsing
				if (!string.IsNullOrWhiteSpace(txtStart.Text))
				{
					if (DateTime.TryParse(txtStart.Text, out DateTime sdt))
						currentAppointment.StartDateTime = sdt;
					else
						MessageBox.Show("Preferred Time Input Error: invalid format");
				}
				if (!string.IsNullOrWhiteSpace(txtEnd.Text))
				{
					if (DateTime.TryParse(txtEnd.Text, out DateTime edt))
						currentAppointment.EndDateTime = edt;
					else
						MessageBox.Show("Preferred Time Input Error: invalid format");
				}

				//notes and reminder
				currentAppointment.Notes = txtNotes.Text;
				currentAppointment.ReminderMinutesBefore = int.TryParse(txtReminder.Text, out int reminder) ? reminder : 0;

				//update or add the appointment
				if (pageAppointmentID > 0)
				{
					//update the appointment
					((App)Application.Current).db.UpdateAppointmentbyID(((App)Application.Current).userid, pageAppointmentID, currentAppointment);
				}
				else
				{
					//add the appointment and get the new id
					int newId = ((App)Application.Current).db.AddAppointment(currentAppointment);
					if (newId > 0)
					{
						pageAppointmentID = newId;
						currentAppointment.AppointmentID = newId;
					}
				}


				//reset the button
				SaveButton.IsEnabled = false;

				//navigate back to the appointment page
				this.NavigationService?.Navigate(new AppointmentPage());
			}
			catch (Exception except)
			{
				MessageBox.Show($"Save Error:\n{FormatException(except)}");
			}
		}

	}
}
