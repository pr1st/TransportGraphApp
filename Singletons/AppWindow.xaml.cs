using System;
using System.Collections.Generic;
using System.Windows;
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

            MenuFileExit.Click += (sender, args) => ExitAction.Invoke();
            SetUpTransportSystemActions();
            SetUpCitiesActions();
            SetUpRoadsActions();
            MenuHelpAbout.Click += (sender, args) => AboutAction.Invoke();
            MenuHelpOverview.Click += (sender, args) => OverviewAction.Invoke();
            
            MainPanel.Children.Add(AppGraph.Instance);
        }
        
        private void SetUpTransportSystemActions() {
            ComponentUtils.InsertIconToButton(ButtonTransportSystemList, AppResources.GetTransportSystemsListIcon, "Список транспортных систем");
            ButtonTransportSystemList.Click += (sender, args) => ListTransportSystemsAction.Invoke();
            MenuTransportSystemList.Click += (sender, args) => ListTransportSystemsAction.Invoke();

            ComponentUtils.InsertIconToButton(ButtonTransportSystemParameters, AppResources.GetTransportSystemsParametersIcon, "Параметры транспортной системы");
            ButtonTransportSystemParameters.Click += (sender, args) => TransportSystemParametersAction.Invoke();
            MenuTransportSystemParameters.Click += (sender, args) => TransportSystemParametersAction.Invoke();
            
            _systemDependentElements.Add(ButtonTransportSystemParameters);
            _systemDependentElements.Add(MenuTransportSystemParameters);
        }

        private void SetUpCitiesActions() {
            ComponentUtils.InsertIconToButton(ButtonCitiesList, AppResources.GetCitiesListIcon, "Список населенных пунктов в системе");
            ButtonCitiesList.Click += (sender, args) => ListCitiesAction.Invoke();
            MenuCitiesList.Click += (sender, args) => ListCitiesAction.Invoke();
            
            _systemDependentElements.Add(ButtonCitiesList);
            _systemDependentElements.Add(MenuCitiesList);
        }

        private void SetUpRoadsActions() {
            ComponentUtils.InsertIconToButton(ButtonRoadsList, AppResources.GetRoadsListIcon, "Список маршрутов в системе");
            ButtonRoadsList.Click += (sender, args) => ListRoadsAction.Invoke();
            MenuRoadsList.Click += (sender, args) => ListRoadsAction.Invoke();
            
            _systemDependentElements.Add(ButtonRoadsList);
            _systemDependentElements.Add(MenuRoadsList);
        }

        
        public void SystemSelected(bool isSystemSelected) {
            foreach (var element in _systemDependentElements) {
                element.IsEnabled = isSystemSelected;
            }
        }
    }
}