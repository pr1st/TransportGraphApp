using System;
using System.Windows.Media.Imaging;

namespace TransportGraphApp {
    internal static class AppResources {
        private const string ResourcesDir = "resources";

        private static Uri GetResourceUrl(string resourceName) => new Uri($"{ResourcesDir}/{resourceName}", UriKind.Relative);
        
        public static BitmapImage GetAppIcon => new BitmapImage(GetResourceUrl("app-icon.png"));
        
        public static BitmapImage GetAddItemIcon => new BitmapImage(GetResourceUrl("add-item.png"));
        public static BitmapImage GetUpdateItemIcon => new BitmapImage(GetResourceUrl("update-item.png"));
        public static BitmapImage GetRemoveItemIcon => new BitmapImage(GetResourceUrl("remove-item.png"));

        public static string GetAppTitle => "Transport graph app";
        public static string GetDefaultDataBasePath => "./application-data.db";

        public static BitmapImage GetTransportSystemsListIcon => new BitmapImage(GetResourceUrl("transport-systems-list.png"));
        public static BitmapImage GetTransportSystemsParametersIcon => new BitmapImage(GetResourceUrl("transport-systems-parameters.png"));
        public static BitmapImage GetCitiesListIcon => new BitmapImage(GetResourceUrl("cities-list.png"));
        public static BitmapImage GetRoadsListIcon => new BitmapImage(GetResourceUrl("roads-list.png"));
    }
}