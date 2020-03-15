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

        public class Database {
            public static BitmapImage GetDataBaseCreateIcon => new BitmapImage(GetResourceUrl("database-create.png"));
            public static BitmapImage GetDataBaseOpenIcon => new BitmapImage(GetResourceUrl("database-open.png"));
            public static BitmapImage GetDataBaseCloseIcon => new BitmapImage(GetResourceUrl("database-close.png"));
        }

        public static BitmapImage GetTransportSystemsListIcon => new BitmapImage(GetResourceUrl("transport-systems-list.png"));
        public static BitmapImage GetCitiesListIcon => new BitmapImage(GetResourceUrl("cities-list.png"));
    }
}