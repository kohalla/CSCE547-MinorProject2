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
	/// Interaction logic for CategoryDetailPage.xaml
	/// </summary>
	public partial class CategoryDetailPage : Page
	{
		int pageCategoryID = 0;
		public CategoryDetailPage(DataLayer.Category targetCategory)
		{
			InitializeComponent();
			this.DataContext = targetCategory;
			pageCategoryID = targetCategory.CategoryID;


			txtCategoryName.Text = targetCategory.CategoryName?? "New Category";

			cmbCategoryGroup.ItemsSource = new List<string> { "DailyTask", "Chore", "FunActivity", "Goal", "Appointment" };
			if (pageCategoryID != 0)
			{
				cmbCategoryGroup.SelectedItem = targetCategory.CategoryGroup;
			}
			else
			{
				cmbCategoryGroup.SelectedIndex = 0;
			}
			cmbCategoryGroup.SelectionChanged += (s, e) => { if (this.IsLoaded) SaveButton.IsEnabled = true; };

			txtDescription.Text = targetCategory.Description;

			txtColor.Text = targetCategory.ColorHex ?? "#ffffff";

		}
		private void OnTextboxTextChanged(object sender, TextChangedEventArgs e)
		{
			if (!this.IsLoaded) return;
			SaveButton.IsEnabled = true;
		}
		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			var category = (DataLayer.Category)this.DataContext;
			category.CategoryName = txtCategoryName.Text;
			category.CategoryGroup = cmbCategoryGroup.SelectedItem.ToString();
			category.Description = txtDescription.Text;
			category.ColorHex = txtColor.Text;
			if (pageCategoryID == 0)
			{
				// New category
				((App)Application.Current).db.AddCategory(category);
			}
			else
			{
				// Existing category
				((App)Application.Current).db.UpdateCategory(category);
			}
			this.NavigationService?.GoBack();
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
				((App)Application.Current).db.DeleteCategorybyID(pageCategoryID);

				//reset the button
				SaveButton.IsEnabled = false;

				//navigate back to the category page
				this.NavigationService?.Navigate(new AdminPage());
			}
			catch (Exception except)
			{
				MessageBox.Show($"Delete Error:\n{FormatException(except)}");
			}
		}
	}
	
}
