using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TransportGraphApp.Actions;
using TransportGraphApp.Actions.CityActions;
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
            _toolBarItems.Add(new Separator());
            SetUpTransportSystemActions();
            _toolBarItems.Add(new Separator());
            SetUpCitiesActions();
            _toolBarItems.Add(new Separator());
            SetUpRoadsActions();
            ToolBar.ItemsSource = _toolBarItems;
        }

        private readonly List<UIElement> _toolBarItems = new List<UIElement>();

        private void SetUpDataBaseActions() {
            var actions = AppActions.Instance;

            var createButton = new IconButton(AppResources.Database.GetDataBaseCreateIcon, () => { }) {
                Margin = new Thickness(0, 0, 0, 0),
                ToolTip = "Создать файл базы данных"
            };
            actions.AddElementToAction<CreateDataBaseAction>(createButton.Button);
            actions.AddElementToAction<CreateDataBaseAction>(MenuFileCreateDataBase);

            var openButton = new IconButton(AppResources.Database.GetDataBaseOpenIcon, () => { }) {
                Margin = new Thickness(0, 0, 0, 0),
                ToolTip = "Открыть базу данных"
            };
            actions.AddElementToAction<OpenDataBaseAction>(openButton.Button);
            actions.AddElementToAction<OpenDataBaseAction>(MenuFileOpenDataBase);

            var closeButton = new IconButton(AppResources.Database.GetDataBaseCloseIcon, () => { }) {
                Margin = new Thickness(0, 0, 0, 0),
                ToolTip = "Закрыть текущую базу данных"
            };
            actions.AddElementToAction<CloseDataBaseAction>(closeButton.Button);
            actions.AddElementToAction<CloseDataBaseAction>(MenuFileCloseDataBase);

            _toolBarItems.AddRange(new[] {createButton, openButton, closeButton});
        }

        private void SetUpTransportSystemActions() {
            var actions = AppActions.Instance;

            var systemsListButton = new IconButton(AppResources.GetTransportSystemsListIcon, () => { }) {
                Margin = new Thickness(0, 0, 0, 0),
                ToolTip = "Список транспортных систем"
            };
            actions.AddElementToAction<ListTransportSystemsAction>(MenuTransportSystemList);
            actions.AddElementToAction<ListTransportSystemsAction>(systemsListButton.Button);

            //TODO add params buttons

            _toolBarItems.AddRange(new[] {systemsListButton});
        }

        private void SetUpCitiesActions() {
            var actions = AppActions.Instance;

            var citiesListButton = new IconButton(AppResources.GetCitiesListIcon, () => { }) {
                Margin = new Thickness(0, 0, 0, 0),
                ToolTip = "Список городов в системе"
            };
            actions.AddElementToAction<ListCitiesAction>(MenuCitiesList);
            actions.AddElementToAction<ListCitiesAction>(citiesListButton.Button);

            _toolBarItems.AddRange(new[] {citiesListButton});
        }

        private void SetUpRoadsActions() {
            //TODO
        }

        public void DrawGraph() {
            AppGraphPanel.Children.Clear();
            if (App.CurrentStates[AppStates.TransportSystemSelected]) {
                var ts = AppCurrentSystem.Instance.GetTransportSystem();
                var cities = AppCurrentSystem.Instance.GetCities();
                var roads = AppCurrentSystem.Instance.GetRoads();
                var prev = new Label() {
                    Content = "Скоро здесь будет рисоваться граф, а пока"
                };
                var nameLabel = new Label() {
                    Content = $"Название: {ts.Name}"
                };
                var cityLabel = new Label() {
                    Content = $"В сети присутсвует {cities.Count} различных населенных пунктов"
                };
                var roadLabel = new Label() {
                    Content = $"В сети присутсвует {roads.Count} различных маршрутов"
                };

                AppGraphPanel.Children.Add(prev);
                AppGraphPanel.Children.Add(nameLabel);
                AppGraphPanel.Children.Add(cityLabel);
                AppGraphPanel.Children.Add(roadLabel);
            }
            else {
                var label = new Label() {
                    Content = "Никакая транспортная система еще не выбрана"
                };
                AppGraphPanel.Children.Add(label);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e) {
            var position = e.GetPosition(relativeTo: this);


            StatusText.Text = $"X: {position.X} Y: {position.Y}";
        }
    }
}