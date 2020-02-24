using System;
using System.Windows;
using System.Windows.Input;
using TransportGraphApp.Actions;

namespace TransportGraphApp.Singletons {
    public partial class AppWindow : Window {
        private static AppWindow _instance;
        public static AppWindow Instance => _instance ??= new AppWindow();


        private AppWindow() {
            InitializeComponent();
            Title = AppResources.GetAppTitle;
            Icon = AppResources.GetAppIcon;
        }

        private void SelectDataBaseFile(object sender, RoutedEventArgs e) => SelectDataBaseFileAction.Invoke();

        private void CreateAndOpenDataBaseFile(object sender, RoutedEventArgs e) => CreateAndOpenDataBaseFileAction.Invoke();

        private void Close(object sender, RoutedEventArgs e) => CloseAction.Invoke();

        private void NewGraph(object sender, RoutedEventArgs e) => NewGraphAction.Invoke();

        private void SelectGraph(object sender, RoutedEventArgs e) => SelectGraphAction.Invoke();

        private void ChangeGraphAttributes(object sender, RoutedEventArgs e) => ChangeGraphAttributesAction.Invoke();

        private void NewNode(object sender, RoutedEventArgs e) => NewNodeAction.Invoke();

        private void NodeList(object sender, RoutedEventArgs e) => NodeListAction.Invoke();

        private void NewEdge(object sender, RoutedEventArgs e) => NewEdgeAction.Invoke();

        private void EdgeList(object sender, RoutedEventArgs e) => EdgeListAction.Invoke();

        private void About(object sender, RoutedEventArgs e) => AboutAction.Invoke();

        public void AppStateChanged(AppState newState) {
            switch (newState) {
                case AppState.Initial: {
                    GraphMenu.IsEnabled = false;
                    NodeMenu.IsEnabled = false;
                    EdgeMenu.IsEnabled = false;
                    break;
                }
                case AppState.ConnectedToDatabase: {
                    GraphMenu.IsEnabled = true;
                    ChangeGraphAttributesMenu.IsEnabled = false;
                    NodeMenu.IsEnabled = false;
                    EdgeMenu.IsEnabled = false;
                    break;
                }
                case AppState.GraphSelected: {
                    GraphMenu.IsEnabled = true;
                    ChangeGraphAttributesMenu.IsEnabled = true;
                    NodeMenu.IsEnabled = true;
                    EdgeMenu.IsEnabled = true;
                    break;
                }
                default:
                    throw new Exception("Invalid state");
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e) {
            var position = e.GetPosition(relativeTo: this);
            StatusText.Text = $"X: {position.X} Y: {position.Y}";
        }
    }
}