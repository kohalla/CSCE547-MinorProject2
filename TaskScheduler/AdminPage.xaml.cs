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
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
		private bool _suppressNavigation = false;
		public AdminPage()
        {
			List<DataLayer.Category> allCategories = ((App)Application.Current).db.GetAllCategories();

			InitializeComponent();

			var searchOptions= new List<String>{"ID", "Name", "Group"};

            searchCategoryComboBox.ItemsSource = searchOptions;
			searchCategoryComboBox.SelectedIndex = 0;
			searchCategoryComboBox.SelectionChanged += (s, e) =>
			{
				// Clear the search text box when the search option changes
				//searchCategoryTextBox.Text = string.Empty;
			};

			categoryDataGrid.ItemsSource = allCategories;


		}
		private const int MaxAutoGenColumns = 5;
		private void CategoryDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
		{
			var grid = (DataGrid)sender; // grid.Columns contains columns already generated for this ItemsSource, so compare against it
			if (grid.Columns.Count >= MaxAutoGenColumns) { e.Cancel = true; } 
		
		}

		private void Click_AddCategory(object sender, RoutedEventArgs e)
		{			
			DataLayer.Category targetCategory = new DataLayer.Category();
			CategoryDetailPage categoryDetails = new CategoryDetailPage(targetCategory);
			this.NavigationService?.Navigate(categoryDetails);
		}
		private void Category_SelectionChanged(Object sender, SelectionChangedEventArgs e)
		{
			if (_suppressNavigation)
			{
				_suppressNavigation = false;
				return;
			}
			if (categoryDataGrid.SelectedItem != null)
			{
				DataLayer.Category selectedCategory = (DataLayer.Category)categoryDataGrid.SelectedItem;
				CategoryDetailPage categoryDetails = new CategoryDetailPage(selectedCategory);
				this.NavigationService?.Navigate(categoryDetails);
			}
		}

		private void Click_SearchCategoryGo(object sender, RoutedEventArgs e)
		{
			string selected = searchCategoryComboBox.SelectedItem?.ToString() ?? string.Empty;
			string searchWhat;
			if (selected.IndexOf("id", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				searchWhat = "ID";
			}
			else if (selected.IndexOf("name", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				searchWhat = "Name";
			}
			else if (selected.IndexOf("group", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				searchWhat = "Group";
			}
			else
			{
				searchWhat = "Name";
			}

			string searchField = searchCategoryTextBox.Text ?? string.Empty;
			List<DataLayer.Category> searchResults;
			if (string.IsNullOrWhiteSpace(searchField))
			{
				searchResults = ((App)Application.Current).db.GetAllCategories();
			}
			else
			{
				searchResults = ((App)Application.Current).db.SearchCategoriesByName(searchWhat, searchField);
			}

			categoryDataGrid.ItemsSource = searchResults;

		}

		private void Click_ClearSearch(object sender, RoutedEventArgs e)
		{
			categoryDataGrid.ItemsSource = ((App)Application.Current).db.GetAllCategories();
		}
	}
}
