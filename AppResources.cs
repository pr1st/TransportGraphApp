using System;
using System.Windows.Media.Imaging;

namespace TransportGraphApp {
    internal static class AppResources {
        private const string ResourcesDir = "resources";

        private static Uri GetResourceUrl(string resourceName) => new Uri($"{ResourcesDir}/{resourceName}", UriKind.Relative);

        public static BitmapImage GetAppIcon => new BitmapImage(GetResourceUrl("app-icon.png"));

        public static BitmapImage GetRemoveItemIcon => new BitmapImage(GetResourceUrl("remove-item.png"));

        public static BitmapImage GetAddItemIcon => new BitmapImage(GetResourceUrl("add-item.png"));

        public static BitmapImage GetUpdateItemIcon => new BitmapImage(GetResourceUrl("update-item.png"));
        public static BitmapImage GetUpdateDItemIcon => new BitmapImage(GetResourceUrl("update-item-disabled.png"));
        public static BitmapImage GetDatabaseItemIcon => new BitmapImage(GetResourceUrl("database-item.png"));
        public static BitmapImage GetDescriptionItemIcon => new BitmapImage(GetResourceUrl("description-item.png"));

        public static string GetAppTitle => "Transport graph app";

        public static string GetDefaultDataBasePath => "./application-data.db";
    }
}