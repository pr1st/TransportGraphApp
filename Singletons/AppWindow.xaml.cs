using System.Windows;
using System.Windows.Controls;
using TransportGraphApp.Actions;

namespace TransportGraphApp.Singletons {
    public partial class AppWindow : Window {
        public AppWindow() {
            InitializeComponent();
            Title = AppResources.GetAppTitle;
            Icon = AppResources.GetAppIcon;
            
            SetUpModelsActions();
            SetUpTaskActions();
            SetUpHelpActions();
        }

        private void SetUpModelsActions() {
            ComponentUtils.InsertIconToButton(ButtonGlobalParameters, AppResources.GetGlobalParametersIcon, "Глобальные параметры сети");
            ButtonGlobalParameters.Click += (sender, args) => GlobalParametersAction.Invoke();
            MenuModelsGlobalParameters.Click += (sender, args) => GlobalParametersAction.Invoke();
            
            ComponentUtils.InsertIconToButton(ButtonTransportSystemList, AppResources.GetTransportSystemsListIcon, "Транспортные системы");
            ButtonTransportSystemList.Click += (sender, args) => ListTransportSystemsAction.Invoke();
            MenuModelsTransportSystems.Click += (sender, args) => ListTransportSystemsAction.Invoke();

            ComponentUtils.InsertIconToButton(ButtonCitiesList, AppResources.GetCitiesListIcon, "Населенные пункты");
            ButtonCitiesList.Click += (sender, args) => ListCitiesAction.Invoke();
            MenuModelsCities.Click += (sender, args) => ListCitiesAction.Invoke();
            
            ComponentUtils.InsertIconToButton(ButtonRoadsList, AppResources.GetRoadsListIcon, "Маршруты");
            ButtonRoadsList.Click += (sender, args) => ListRoadsAction.Invoke();
            MenuModelsRoads.Click += (sender, args) => ListRoadsAction.Invoke();
        }

        private void SetUpTaskActions() {
            ComponentUtils.InsertIconToButton(ButtonTaskSpecification, AppResources.GetTaskSpecificationIcon, "Спецификация поставленной задачи");
            ButtonTaskSpecification.Click += (sender, args) => TaskSpecificationAction.Invoke();
            MenuTaskSpecification.Click += (sender, args) => TaskSpecificationAction.Invoke();

            ComponentUtils.InsertIconToButton(ButtonTaskCheckData, AppResources.GetTaskCheckIcon, "Проверка данных");
            ButtonTaskCheckData.Click += (sender, args) => TaskCheckDataAction.Invoke();
            MenuTaskCheckData.Click += (sender, args) => TaskCheckDataAction.Invoke();
            
            ComponentUtils.InsertIconToButton(ButtonTaskStart, AppResources.GetTaskStartIcon, "Запуск задачи");
            ButtonTaskStart.Click += (sender, args) => TaskStartAction.Invoke();
            MenuTaskStart.Click += (sender, args) => TaskStartAction.Invoke();
            
            ComponentUtils.InsertIconToButton(ButtonTaskResults, AppResources.GetTaskResultsIcon, "Результаты расчетов");
            ButtonTaskResults.Click += (sender, args) => TaskResultsAction.Invoke();
            MenuTaskResults.Click += (sender, args) => TaskResultsAction.Invoke();
        }

        private void SetUpHelpActions() {
            MenuHelpAbout.Click += (sender, args) => AboutAction.Invoke();
        }
    }
}