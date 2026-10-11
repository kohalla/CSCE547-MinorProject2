using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DataLayer;

//global settings go here
namespace TaskScheduler
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public int userid { get; set; } = 2; //default to admin user
        public bool adminUser { get; set; } = false; //default to admin user

        // Lazily create the data layer so the application startup doesn't eagerly open
        // database connections or perform work that may trigger network activity.
        private readonly Lazy<TaskSchedulerDataLayer> _lazyDb = new Lazy<TaskSchedulerDataLayer>(() => new TaskSchedulerDataLayer());

        public TaskSchedulerDataLayer db => _lazyDb.Value;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Load persisted theme setting (defaults to "Light" in Settings.designer)
            var theme = TaskScheduler.Properties.Settings.Default.AppTheme;
            if (string.IsNullOrEmpty(theme))
            {
                theme = "Light";
            }

            ApplyTheme(theme, save: false);
        }

        /// <summary>
        /// Apply a theme by replacing any existing theme ResourceDictionary with the requested one.
        /// The theme files are expected at Themes/&lt;Name&gt;Theme.xaml (e.g. Themes/LightTheme.xaml).
        /// </summary>
        public void ApplyTheme(string themeName, bool save = true)
        {
            if (Application.Current == null)
                return;

            var dictionaries = Application.Current.Resources.MergedDictionaries;

            // Remove any existing theme dictionaries (those that contain "Theme.xaml" in their source)
            var existing = dictionaries.Where(d => d.Source != null && d.Source.OriginalString.Contains("Theme.xaml")).ToList();
            foreach (var d in existing)
            {
                dictionaries.Remove(d);
            }

            // Add the requested theme dictionary
            try
            {
                var uri = new Uri($"Themes/{themeName}Theme.xaml", UriKind.Relative);
                var newDict = new ResourceDictionary() { Source = uri };
                dictionaries.Add(newDict);
            }
            catch (Exception)
            {
                // If loading fails, do nothing - keep the app running. In a real app consider logging this.
            }

            if (save)
            {
                TaskScheduler.Properties.Settings.Default.AppTheme = themeName;
                TaskScheduler.Properties.Settings.Default.Save();
            }
        }
    }
}
