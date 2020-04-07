using System;
using System.Windows.Media.Imaging;

namespace TransportGraphApp {
    internal static class AppResources {
        private const string ResourcesDir = "resources";
        private static Uri GetResourceUrl(string resourceName) => new Uri($"{ResourcesDir}/{resourceName}", UriKind.Relative);
        
        // App strings
        public static string GetAppTitle => "Transport graph app";
        public static string GetDefaultDataBasePath => "./application-data.db";
        
        // App icons
        public static BitmapImage GetAppIcon => new BitmapImage(GetResourceUrl("app-icon.png"));
        
        public static BitmapImage GetAddItemIcon => new BitmapImage(GetResourceUrl("util-add-item.png"));
        public static BitmapImage GetRemoveItemIcon => new BitmapImage(GetResourceUrl("util-remove-item.png"));

        public static BitmapImage GetTransportSystemsListIcon => new BitmapImage(GetResourceUrl("models-transport-systems.png"));
        public static BitmapImage GetCitiesListIcon => new BitmapImage(GetResourceUrl("models-cities.png"));
        public static BitmapImage GetRoadsListIcon => new BitmapImage(GetResourceUrl("models-roads.png"));
        
        public static BitmapImage GetGraphParametersIcon => new BitmapImage(GetResourceUrl("graph-parameters.png"));
        public static BitmapImage GetGraphStartAlgorithmIcon => new BitmapImage(GetResourceUrl("graph-start-algorithm.png"));
    }
}