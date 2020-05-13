using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Graph;
using TransportGraphApp.Models;

namespace TransportGraphApp.Dialogs {
    public partial class TaskResultsDialog : Window {
        // updateable properties
        private AlgorithmResult _selectedResult;

        // ui components
        private ConstantStringRowControl _configAlgTypeControl;
        private ConstantStringRowControl _configMethodTypeControl;
        private GenericTableRowControl<TransportSystem> _configTransportSystemsControl;
        private GenericTableRowControl<CityTag> _configCentralCitiesTagsControl;
        private GenericTableRowControl<RoadType> _configUnusedRoadTypesControl;
        private GenericEntityListControl<Node> _resultTableControl;

        public TaskResultsDialog() {
            InitializeComponent();
            Owner = App.Window;
            Icon = AppResources.GetAppIcon;

            InitConfigProperty();
            InitResultTableProperty();
            InitReportMethods();

            var entityList = new GenericEntityListControl<AlgorithmResult>(
                "Результаты",
                AlgorithmResult.PropertyMatcher(),
                DisplayProperties);

            ListPanel.Children.Add(entityList.GetUiElement());
            entityList.SetSource(App.DataBase.GetCollection<AlgorithmResult>().FindAll());

            ComponentUtils.InsertIconToButton(RemoveButton, AppResources.GetRemoveItemIcon,
                "Удалить результат из списка");
            RemoveButton.Click += (sender, args) => {
                if (entityList.Selected == null) return;
                foreach (var result in entityList.SelectedAll()) {
                    App.DataBase.GetCollection<AlgorithmResult>().Delete(result.Id);
                }

                entityList.SetSource(App.DataBase.GetCollection<AlgorithmResult>().FindAll());
                VisibilityPanel.Visibility = Visibility.Collapsed;
            };
        }

        // init methods
        private void InitConfigProperty() {
            _configAlgTypeControl = new ConstantStringRowControl() {
                TitleValue = "Тип алгоритма"
            };
            ConfigPropertiesPanel.Children.Add(_configAlgTypeControl);

            _configMethodTypeControl = new ConstantStringRowControl() {
                TitleValue = "Тип метода"
            };
            ConfigPropertiesPanel.Children.Add(_configMethodTypeControl);

            _configTransportSystemsControl = new GenericTableRowControl<TransportSystem>() {
                TitleValue = "Использованные транспортные системы",
            };
            _configTransportSystemsControl.AddColumns(TransportSystem.PropertyMatcher());
            ConfigPropertiesPanel.Children.Add(_configTransportSystemsControl.GetUiElement);

            _configCentralCitiesTagsControl = new GenericTableRowControl<CityTag>() {
                TitleValue = "Центральные города"
            };
            _configCentralCitiesTagsControl.AddColumns(CityTag.PropertyMatcher());
            ConfigPropertiesPanel.Children.Add(_configCentralCitiesTagsControl.GetUiElement);

            _configUnusedRoadTypesControl = new GenericTableRowControl<RoadType>() {
                TitleValue = "Исключенные дороги"
            };
            _configUnusedRoadTypesControl.AddColumns(RoadType.PropertyMatcher());
            ConfigPropertiesPanel.Children.Add(_configUnusedRoadTypesControl.GetUiElement);
        }

        private void InitResultTableProperty() {
            var propertyMatcher = new Dictionary<string, Func<Node, object>> {
                {
                    "Название",
                    n => n.Name
                }, {
                    "Итоговое значение",
                    n => {
                        if (_selectedResult == null) return null;
                        var weight = n.MinWeight().Weight.Value;
                        switch (_selectedResult.AlgorithmConfig.AlgorithmType) {
                            case AlgorithmType.Time when n.IsCentral:
                                return "0 д. 0 ч. 0 м.";
                            case AlgorithmType.Time: {
                                var time = (int) weight;
                                var d = time / (60 * 24);
                                time -= d * 60 * 24;
                                var h = time / 60;
                                time -= h * 60;
                                var m = time;
                                return $"{d} д. {h} ч. {m} м.";
                            }
                            case AlgorithmType.Length:
                            case AlgorithmType.ComplexCost:
                            case AlgorithmType.Cost:
                                return $"{weight:F2} у.е.";
                            default: throw new NotImplementedException();
                        }
                    }
                }
            };
            _resultTableControl = new GenericEntityListControl<Node>(
                "Таблица результатов",
                propertyMatcher,
                n => { });
            PropertiesPanel.Children.Add(_resultTableControl.GetUiElement());
        }
        
        private void InitReportMethods() {
            MinReport.Click += (sender, args) => App.ReportSaver.MinimalReport(_selectedResult);
            MaxReport.Click += (sender, args) => App.ReportSaver.FullReport(_selectedResult);
            JsonReport.Click += (sender, args) => App.ReportSaver.JsonReport(_selectedResult);
            MatrixReport.Click += (sender, args) => App.ReportSaver.MatrixReport(_selectedResult);
        }

        // callback methods
        private void DisplayProperties(AlgorithmResult res) {
            _selectedResult = res;

            _configAlgTypeControl.Value = res.AlgorithmConfig.AlgorithmType.GetDescription();
            _configMethodTypeControl.Value = res.AlgorithmConfig.MethodType.GetDescription();
            _configTransportSystemsControl.Value = res.AlgorithmConfig.TransportSystems;
            _configCentralCitiesTagsControl.Value = res.AlgorithmConfig.CityTags;
            _configUnusedRoadTypesControl.Value = res.AlgorithmConfig.RoadTypes;

            _resultTableControl.SetSource(res.Nodes);

            VisibilityPanel.Visibility = Visibility.Visible;
        }

        private void DisplayNodeInfo(Node n) {
            var res = _selectedResult;
            Console.WriteLine($"Print Node: {n.Name}");
            Console.WriteLine($"Центральный: {n.IsCentral}");
            var minWeight = new GraphWeight();
            foreach (var weight in n.Weights) {
                Console.WriteLine(
                    $"Dep time: {weight.Time} || From: {weight.From.Name} || Weight: {weight.Weight}");
                if (weight.Weight < minWeight.Weight) {
                    minWeight = weight;
                }
            }

            if (n.IsCentral) return;

            Console.WriteLine($"Min value: {minWeight.Weight}");
            Console.WriteLine($"Last node: {minWeight.From.Name}");

            Console.WriteLine("Path");
            var a = minWeight;
            Console.Write($"{n.Name}");
            while (!a.From.IsCentral) {
                var next = a.From.Weights.First(w => a.FromTime == w.Time);
                Console.Write($" ->(+{(a.Weight - next.Weight).Value}) {a.From.Name}");
                a = next;
            }

            Console.Write($" ->(+{(a.Weight).Value}) {a.From.Name}");

            Console.WriteLine();
            Console.WriteLine();
        }
    }
}