using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TransportGraphApp.Actions;

namespace TransportGraphApp.Singletons {
    public partial class AppWindow : Window {
        private static AppWindow _instance;
        
        public static AppWindow Instance => _instance ??= new AppWindow();
        
        
        private IList<UIElement> _systemDependentElements = new List<UIElement>();
        
        private AppWindow() {
            InitializeComponent();
            Title = AppResources.GetAppTitle;
            Icon = AppResources.GetAppIcon;

            SetUpModelsActions();
            SetUpGraphActions();
            MenuHelpAbout.Click += (sender, args) => AboutAction.Invoke();
            MenuHelpOverview.Click += (sender, args) => OverviewAction.Invoke();
            
            MainPanel.Children.Add(AppGraph.Instance);
        }

        private void SetUpModelsActions() {
            ComponentUtils.InsertIconToButton(ButtonTransportSystemList, AppResources.GetTransportSystemsListIcon, "Список транспортных систем");
            ButtonTransportSystemList.Click += (sender, args) => ListTransportSystemsAction.Invoke();
            MenuModelsTransportSystems.Click += (sender, args) => ListTransportSystemsAction.Invoke();

            ComponentUtils.InsertIconToButton(ButtonCitiesList, AppResources.GetCitiesListIcon, "Список населенных пунктов в системе");
            ButtonCitiesList.Click += (sender, args) => ListCitiesAction.Invoke();
            MenuModelsCities.Click += (sender, args) => ListCitiesAction.Invoke();
            _systemDependentElements.Add(ButtonCitiesList);
            _systemDependentElements.Add(MenuModelsCities);
            
            ComponentUtils.InsertIconToButton(ButtonRoadsList, AppResources.GetRoadsListIcon, "Список маршрутов в системе");
            ButtonRoadsList.Click += (sender, args) => ListRoadsAction.Invoke();
            MenuModelsRoads.Click += (sender, args) => ListRoadsAction.Invoke();
            _systemDependentElements.Add(ButtonRoadsList);
            _systemDependentElements.Add(MenuModelsRoads);
        }

        private void SetUpGraphActions() {
            ComponentUtils.InsertIconToButton(ButtonGraphParameters, AppResources.GetGraphParametersIcon, "Параметры визуализации графа");
            ButtonGraphParameters.Click += (sender, args) => GraphParametersAction.Invoke();
            MenuGraphParameters.Click += (sender, args) => GraphParametersAction.Invoke();
            _systemDependentElements.Add(ButtonGraphParameters);
            _systemDependentElements.Add(MenuGraphParameters);
            
            ComponentUtils.InsertIconToButton(ButtonStartAlgorithm, AppResources.GetGraphStartAlgorithmIcon, "Стартовать алгоритм");
            ButtonStartAlgorithm.Click += (sender, args) => StartAlgorithmAction.Invoke();
            MenuGraphStartAlgorithm.Click += (sender, args) => StartAlgorithmAction.Invoke();
            _systemDependentElements.Add(ButtonStartAlgorithm);
            _systemDependentElements.Add(MenuGraphStartAlgorithm);
        }
        
        public void SystemSelected(bool isSystemSelected) {
            foreach (var element in _systemDependentElements) {
                element.IsEnabled = isSystemSelected;
            }
        }
        
        private void ResizeEvent(object sender, SizeChangedEventArgs e) {
            AppGraph.Instance.DrawGraph();
        }
    }
}