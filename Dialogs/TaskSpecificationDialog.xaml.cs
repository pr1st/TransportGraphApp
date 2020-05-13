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
                    "Алгоритм основанный на длине пройденного пути использует алгоритм Дейкстры, а вес ребра использующийся в алгоритме вычисляется по формуле:\n" +
                    "W=L\n" +
                    "Где, W- вес ребра использующийся в алгоритме,\n" +
                    "L- параметр длины маршрута."
                }, {
                    AlgorithmType.Cost,
                    "Алгоритм основанный на денежных затратах на передвижение использует алгоритм Дейкстры, а вес ребра использующийся в алгоритме вычисляется по формуле:\n" +
                    "W=С\n" +
                    "Где, W- вес ребра использующийся в алгоритме,\n" +
                    "С- параметр стоимости маршрута."
                }, {
                    AlgorithmType.Time,
                    "Алгоритм основанный на временных затратах на передвижение использует алгоритм Беллмана-Форда, а вес ребра использующийся в алгоритме вычисляется по формуле:\n" +
                    "W=To - Ta + T\n" +
                    "Где, W - вес ребра использующийся в алгоритме,\n" +
                    "To- время отправления данного маршрута,\n" +
                    "Ta - время прибытия в населенный пункт отправления (для “центральных” населенных пунктов Ta = To),\n" +
                    "T- параметр времени движения маршрута."
                }, {
                    AlgorithmType.ComplexCost,
                    "Алгоритм основанный на денежных затратах на протяжении всего маршрута использует алгоритм Беллмана-Форда, а вес ребра использующийся в алгоритме вычисляется по формуле:\n" +
                    "W=С + (To - Ta) * Сc\n" +
                    "Где, W- вес ребра использующийся в алгоритме,\n" +
                    "С- параметр стоимости маршрута,\n" +
                    "To- время отправления данного маршрута,\n" +
                    "Ta- время прибытия в населенный пункт отправления (для “центральных” населенных пунктов Ta= To),\n" +
                    "Сc- параметр средних денежных затрат на проживание в день в населенном пункте отправления."
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
                {MethodType.Standard, "Метод где все транспортные сети объединяются в одну и задача решается стандартным алгоритмами Дейкстры или Беллмана-Форда "},
                {MethodType.Local, "Метод включает в себя работу на нескольких этапах:\n" +
                                   "1. Алгоритм поиска кратчайшего пути работает локально в каждой транспортной системе, помимо кратчайшего пути к центральным населенным пунктам, находится расстояние к транзитным населенным (пунктам присутствующих в нескольких транспортных системах)\n" +
                                   "2. Алгоритм поиска кратчайших путей работает между всеми транзитными и центральными населенными пунктами\n" +
                                   "3. Значения для остальных населенных пунктов складываются из значений полученных на 1-ой и 2-ой стадиях"}
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