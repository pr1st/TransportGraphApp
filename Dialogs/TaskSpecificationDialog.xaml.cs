using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LiteDB;
using TransportGraphApp.Actions;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Dialogs.ResultDialogs;
using TransportGraphApp.Models;

namespace TransportGraphApp.Dialogs {
    public partial class TaskSpecificationDialog : Window {
        // unmodifiable properties
        private static IList<TransportSystem> _availableTransportSystems;
        private static CityTags _availableCityTags;
        private static RoadTypes _availableRoadTypes;

        // updateable properties
        private AlgorithmConfig _config;


        // ui controls
        private GenericTableRowControl<TransportSystem> _transportSystemsControl;
        private GenericComboBoxRowControl<AlgorithmType> _algorithmTypeControl;
        private GenericComboBoxRowControl<MethodType> _methodTypeControl;
        private GenericTableRowControl<CityTag> _cityTagsControl;
        private GenericTableRowControl<RoadType> _roadUnusedTypesControl;

        public TaskSpecificationDialog() {
            InitializeComponent();
            Owner = App.Window;
            Icon = AppResources.GetAppIcon;

            _availableTransportSystems = App.DataBase.GetCollection<TransportSystem>().FindAll().ToList();
            _availableCityTags = App.DataBase.GetCollection<CityTags>().FindOne(ct => ct.IsPrimary);
            _availableRoadTypes = App.DataBase.GetCollection<RoadTypes>().FindOne(rt => rt.IsPrimary);

            _config = App.DataBase.GetCollection<AlgorithmConfig>().FindOne(a => a.IsPrimary);
            
            InitTransportSystemsProperty();
            InitAlgorithmTypeProperty();
            InitMethodTypeProperty();
            InitCityTagsProperty();
            InitUnusedRoadTypesProperty();

            Closed += (sender, args) => CancelClick();
        }

        // init methods
        private void InitTransportSystemsProperty() {
            _transportSystemsControl = new GenericTableRowControl<TransportSystem>() {
                TitleValue = "Транспортные системы",
                TitleToolTip = "Представляет собой набор транпортных систем по которым будет работать алгоритм",
                OnAdd = alreadyUsedTransportSystems => {
                    var dialog = new GenericSelectEntitiesDialog<TransportSystem>(
                        "Выберите транспортные системы учавствующие в работе алгоритма",
                        TransportSystem.PropertyMatcher(),
                        _availableTransportSystems.Where(ts => !alreadyUsedTransportSystems.Contains(ts)));
                    return dialog.Selected;
                },
                Value = _config.TransportSystems
            };
            _transportSystemsControl.AddColumns(TransportSystem.PropertyMatcher());
            
            PropertiesPanel.Children.Add(_transportSystemsControl.GetUiElement);
        }

        private void InitAlgorithmTypeProperty() {
            var algorithms = new Dictionary<AlgorithmType, string>() {
                {
                    AlgorithmType.Length,
                    "Алгоритм основанный на длинне маршрута, " +
                    "в качестве веса ребра в итоговом графе будет представляться значение длинны указанного маршрута"
                }, {
                    AlgorithmType.Cost,
                    "Алгоритм основанный на затратах на перемещение, " +
                    "в качестве веса ребра в итоговом графе будет представляться значение стоимости указанного маршрута"
                }, {
                    AlgorithmType.Time,
                    "Алгоритм основанный на длительности маршрута, " +
                    "в качестве веса ребра в итоговом графе будет представляться " +
                    "значение времени поездки на данном маршруте плюс время ожидания отправления данного маршрута"
                }, {
                    AlgorithmType.ComplexCost,
                    "Алгоритм основанный на затратах на весь маршрут, " +
                    "в качестве веса ребра в итоговом графе будет представляться " +
                    "значение стоимости указанного маршрута плюс средняя стоимость проживания в данном нас. пункте умноженное на время ожидания отправления данного маршрута"
                }
            };

            _algorithmTypeControl = new GenericComboBoxRowControl<AlgorithmType>(algorithms) {
                TitleValue = "Тип алгоритма",
                TitleToolTip = "Представляет собой используемый тип алгоритма",
                Selected = _config.AlgorithmType
            };

            PropertiesPanel.Children.Add(_algorithmTypeControl.GetUiElement);
        }

        private void InitMethodTypeProperty() {
            var methods = new Dictionary<MethodType, string>() {
                {MethodType.Standard, "Метод где вся сеть предоставляется 1-им графом"},
                {MethodType.Local, "Метод включает в себя работу алгоритма поиска кратчайшего пути локально в каждой системе, а затем ..."}
            };

            _methodTypeControl = new GenericComboBoxRowControl<MethodType>(methods) {
                TitleValue = "Тип метода",
                TitleToolTip = "Представляет собой используемый тип метода",
                Selected = _config.MethodType
            };
            
            PropertiesPanel.Children.Add(_methodTypeControl.GetUiElement);
        }


        private void InitCityTagsProperty() {
            _cityTagsControl = new GenericTableRowControl<CityTag>() {
                TitleValue = "Выбор центральных нас. пунктов",
                TitleToolTip =
                    "Представляет собой набор тэгов, нас. пункт имеющий хотя-бы 1 тег из списка будет считаться центральным",
                OnAdd = alreadyUsedCityTags => {
                    var dialog = new GenericSelectEntitiesDialog<CityTag>(
                        "Выберите типы центральных населенных пунктов",
                        CityTag.PropertyMatcher(),
                        _availableCityTags.Values.Where(ct => !alreadyUsedCityTags.Contains(ct)));
                    return dialog.Selected;
                },
                Value = _config.CityTags
            };
            _cityTagsControl.AddColumns(CityTag.PropertyMatcher());
            
            PropertiesPanel.Children.Add(_cityTagsControl.GetUiElement);
        }

        private void InitUnusedRoadTypesProperty() {
            _roadUnusedTypesControl = new GenericTableRowControl<RoadType>() {
                TitleValue = "Фильтр не используемых маршрутов",
                TitleToolTip =
                    "Представляет собой набор типов маршрутов, маршрут имеющий тип из списка не будет использоваться в работе алгоритма",
                OnAdd = alreadyUsedRoadTypes => {
                    var dialog = new GenericSelectEntitiesDialog<RoadType>(
                        "Выборите не используемые маршруты",
                        RoadType.PropertyMatcher(),
                        _availableRoadTypes.Values.Where(rt => !alreadyUsedRoadTypes.Contains(rt)));
                    return dialog.Selected;
                },
                Value = _config.RoadTypes
            };
            _roadUnusedTypesControl.AddColumns(RoadType.PropertyMatcher());
            
            PropertiesPanel.Children.Add(_roadUnusedTypesControl.GetUiElement);
        }
        
        // click buttons methods
        private void RunClick(object sender, RoutedEventArgs e) {
            UpdateConfig();
            TaskStartAction.Invoke();
        }

        private void CheckClick(object sender, RoutedEventArgs e) {
            UpdateConfig();
            TaskCheckDataAction.Invoke();
        }

        private void CancelClick() {
            UpdateConfig();
        }

        // update state method
        private void UpdateConfig() {
            _config.TransportSystems = _transportSystemsControl.Value;
            _config.AlgorithmType = _algorithmTypeControl.Selected;
            _config.MethodType = _methodTypeControl.Selected;
            _config.CityTags = _cityTagsControl.Value;
            _config.RoadTypes = _roadUnusedTypesControl.Value;
            App.DataBase.GetCollection<AlgorithmConfig>().Update(_config);
        }
    }
}