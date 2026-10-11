using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace TaskScheduler.Converters
{
	// MultiValueConverter that returns a display string for a ComboBox based on SelectedItem and DisplayMemberPath
	public class ComboBoxSelectionTextConverter : IMultiValueConverter
	{
		// Shared instance to allow XAML usage via x:Static without requiring a resource entry
		public static readonly ComboBoxSelectionTextConverter Instance = new ComboBoxSelectionTextConverter();

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values == null || values.Length < 2)
				return string.Empty;

			var selectedItem = values[0];
			var displayMemberPath = values[1] as string;

			if (selectedItem == null)
				return string.Empty;

			if (!string.IsNullOrEmpty(displayMemberPath))
			{
				try
				{
					var prop = selectedItem.GetType().GetProperty(displayMemberPath, BindingFlags.Public | BindingFlags.Instance);
					if (prop != null)
					{
						var val = prop.GetValue(selectedItem);
						return val?.ToString() ?? string.Empty;
					}
				}
				catch
				{
					// fall through to ToString()
				}
			}

			return selectedItem.ToString();
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
