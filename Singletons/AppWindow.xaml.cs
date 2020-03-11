using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TransportGraphApp.Actions;
using TransportGraphApp.Actions.DataBaseActions;
using TransportGraphApp.Actions.TransportSystemActions;
using TransportGraphApp.Actions.UtilActions;
using TransportGraphApp.CustomComponents;

namespace TransportGraphApp.Singletons {
    public partial class AppWindow : Window {
        private static AppWindow _instance;
        public static AppWindow Instance => _instance ??= new AppWindow();

        private AppWindow() {
            InitializeComponent();
            Title = AppResources.GetAppTitle;
            Icon = AppResources.GetAppIcon;


            AppActions.Instance.AddElementToAction<ExitAction>(MenuFileExit);
            SetUpDataBaseActions();
            SetUpTransportSystemActions();
        }

        private void SetUpDataBaseActions() {
            var actions = AppActions.Instance;

            var createButton = new IconButton(AppResources.Database.GetDataBaseCreateIcon, () => { }) {
                Margin = new Thickness(0, 0, 0, 0),
                ToolTip = "Создать файл базы данных"
            };
            actions.AddElementToAction<CreateDataBaseAction>(MenuFileCreateDataBase);
            actions.AddElementToAction<CreateDataBaseAction>(createButton.Button);

            var openButton = new IconButton(AppResources.Database.GetDataBaseOpenIcon, () => { }) {
                Margin = new Thickness(0, 0, 0, 0),
                ToolTip = "Открыть базу данных"
            };
            actions.AddElementToAction<OpenDataBaseAction>(MenuFileOpenDataBase);
            actions.AddElementToAction<OpenDataBaseAction>(openButton.Button);

            var closeButton = new IconButton(AppResources.Database.GetDataBaseCloseIcon, () => { }) {
                Margin = new Thickness(0, 0, 0, 0),
                ToolTip = "Закрыть текущую базу данных"
            };
            actions.AddElementToAction<CloseDataBaseAction>(MenuFileCloseDataBase);
            actions.AddElementToAction<CloseDataBaseAction>(closeButton.Button);


            var list = new List<UIElement> {createButton, openButton, closeButton};
            DataBaseToolBar.ItemsSource = list;
        }

        private void SetUpTransportSystemActions() {
            var actions = AppActions.Instance;

            var listButton = new IconButton(AppResources.GetTransportSystemsListIcon, () => { }) {
                Margin = new Thickness(0, 0, 0, 0),
                ToolTip = "Список транспортных систем"
            };
            actions.AddElementToAction<ListTransportSystemsAction>(MenuTransportSystemList);
            actions.AddElementToAction<ListTransportSystemsAction>(listButton.Button);

            var list = new List<UIElement> {listButton};
            TransportSystemsToolBar.ItemsSource = list;
        }

        public void DrawGraph() {
            var ts = AppGraph.Instance.TransportSystem;
            AppGraphPanel.Children.Clear();
            if (ts == null) {
                var label = new Label() {
                    Content = "Никакая система еще не выбрана"
                };
                AppGraphPanel.Children.Add(label);
                return;
            }

            var prev = new Label() {
                Content = "Выбранная система"
            };
            var name = new Label() {
                Content = $"Name: {ts.Name}"
            };
            AppGraphPanel.Children.Add(prev);
            AppGraphPanel.Children.Add(name);
        }


        //private void NewGraph(object sender, RoutedEventArgs e) => NewGraphAction.Invoke();

        //private void SelectGraph(object sender, RoutedEventArgs e) => SelectGraphAction.Invoke();

        //private void ChangeGraphAttributes(object sender, RoutedEventArgs e) => ChangeGraphAttributesAction.Invoke();

        //private void NewNode(object sender, RoutedEventArgs e) => NewNodeAction.Invoke();

        //private void NodeList(object sender, RoutedEventArgs e) => NodeListAction.Invoke();

        //private void NewEdge(object sender, RoutedEventArgs e) => NewEdgeAction.Invoke();

        //private void EdgeList(object sender, RoutedEventArgs e) => EdgeListAction.Invoke();

        //private void About(object sender, RoutedEventArgs e) => AboutAction.Invoke();


        private void OnMouseMove(object sender, MouseEventArgs e) {
            var position = e.GetPosition(relativeTo: this);


            StatusText.Text = $"X: {position.X} Y: {position.Y}";
        }

        private void Help_Component_NumberInputField(object sender, RoutedEventArgs e) {
            ComponentUtils.ShowMessage(DoubleTextBox.DescriptionInfo, MessageBoxImage.Information);
        }

        private void Help_Component_StringInputField(object sender, RoutedEventArgs e) {
            ComponentUtils.ShowMessage(StringTextBox.DescriptionInfo, MessageBoxImage.Information);
        }

        private void Help_Component_BooleanInputField(object sender, RoutedEventArgs e) {
            ComponentUtils.ShowMessage(TrueFalseBox.DescriptionInfo, MessageBoxImage.Information);
        }
    }
}