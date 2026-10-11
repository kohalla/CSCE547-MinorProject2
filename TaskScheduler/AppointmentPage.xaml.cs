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
    /// Interaction logic for AppointmentPage.xaml
    /// </summary>
    public partial class AppointmentPage : Page
    {
        public AppointmentPage()
        {
			List<DataLayer.Appointment> userAppointments = ((App)Application.Current).db.GetUserAppointments(((App)Application.Current).userid);
			InitializeComponent();

            List<DataLayer.Appointment> UpcomingAppointmentsList = userAppointments.Where(a => a.StartDateTime >= DateTime.Now).OrderBy(a => a.StartDateTime).ToList();
			UpcomingAppointments.ItemsSource = UpcomingAppointmentsList;

			List<DataLayer.Appointment> PastAppointmentsList = userAppointments.Where(a => a.StartDateTime < DateTime.Now).OrderByDescending(a => a.StartDateTime).ToList();
            PastAppointments.ItemsSource = PastAppointmentsList;
			try
			{
				var cal = this.FindName("AppointmentsCalendar") as System.Windows.Controls.Calendar;
				if (cal != null)
				{
					cal.SelectedDates.Clear();
					var appointmentDates = UpcomingAppointmentsList.Where(a => a.StartDateTime.Date >= DateTime.Today).Select(a => a.StartDateTime.Date).Distinct();
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

}
		private void Click_AddAppointment(object sender, RoutedEventArgs e)
		{
			Appointment targetAppointment = new Appointment();
			AppointmentDetailsPage appointmentDetails = new AppointmentDetailsPage(targetAppointment);
			this.NavigationService?.Navigate(appointmentDetails);
		}
		private void UpcomingAppointments_SelectionChanged(Object sender, SelectionChangedEventArgs e)
		{
			if (UpcomingAppointments.SelectedItem != null)
			{
				Appointment selectedAppointment = (Appointment)UpcomingAppointments.SelectedItem;
				AppointmentDetailsPage appointmentDetails = new AppointmentDetailsPage(selectedAppointment);
				this.NavigationService?.Navigate(appointmentDetails);
			}
		}
		private void PastAppointments_SelectionChanged(Object sender, SelectionChangedEventArgs e)
		{
			if (PastAppointments.SelectedItem != null)
			{
				Appointment selectedAppointment = (Appointment)PastAppointments.SelectedItem;
				AppointmentDetailsPage appointmentDetails = new AppointmentDetailsPage(selectedAppointment);
				this.NavigationService?.Navigate(appointmentDetails);
			}
		}

	}
}
