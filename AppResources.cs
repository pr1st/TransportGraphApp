using System;
using System.Windows.Media.Imaging;

namespace TransportGraphApp {
    internal static class AppResources {
        private const string ResourcesDir = "resources";

        private static Uri GetResourceUrl(string resourceName) => new Uri($"{ResourcesDir}/{resourceName}", UriKind.Relative);

        public static BitmapImage GetAppIcon => new BitmapImage(GetResourceUrl("app-icon.png"));

        public static BitmapImage GetPlusSignIcon => new BitmapImage(GetResourceUrl("plus-sign.png"));

        public static BitmapImage GetCloseSignIcon => new BitmapImage(GetResourceUrl("close-sign.png"));

        public static BitmapImage GetUpdateSignIcon => new BitmapImage(GetResourceUrl("update-sign.png"));

        public static BitmapImage GetRemoveItemIcon => new BitmapImage(GetResourceUrl("remove-item.png"));

        public static BitmapImage GetAddItemIcon => new BitmapImage(GetResourceUrl("add-item.png"));

        public static string GetAppTitle => "Transport graph app";

        public static string GetDefaultDataBasePath => "./application-data.db";
    }
}