﻿using System;
using System.Linq;
using System.Windows;
using LiteDB;
using TransportGraphApp.Actions;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Models;

namespace TransportGraphApp.Dialogs {
    public partial class TaskSpecificationDialog : Window {
        private AlgorithmConfig _config;

        private GenericTableRowControl<TransportSystem> _transportSystemsControl;
        private ComboBoxRowControl _algorithmTypeControl;
        private ComboBoxRowControl _methodTypeControl;
        private GenericTableRowControl<CityTag> _cityTagsControl;
        private GenericTableRowControl<RoadType> _roadTypesControl;
        
        public TaskSpecificationDialog() {
            InitializeComponent();
            Owner = App.Window;
            Icon = AppResources.GetAppIcon;
            
            _config = App.DataBase.GetCollection<AlgorithmConfig>().FindOne(a => a.IsPrimary);
            var transportSystems = App.DataBase.GetCollection<TransportSystem>().FindAll().ToList();
            var cityTags = App.DataBase.GetCollection<CityTags>().FindOne(ct => ct.IsPrimary).Values;
            var roadTypes = App.DataBase.GetCollection<RoadTypes>().FindOne(rt => rt.IsPrimary).Values;
            
            _transportSystemsControl = new GenericTableRowControl<TransportSystem>() {
                TitleValue = "Транспортные системы",
                TitleToolTip = "Представляет собой набор транпортных систем по которым будет работать алгоритм",
                OnAdd = transportSystemsInList => {
                    var d = new StringFieldDialog {
                        Title = "Добавить транспортную систему в список",
                        IsViable = transportSystemName => {
                            if (transportSystemName.Trim() == "") {
                                ComponentUtils.ShowMessage("Введите не пустое название", MessageBoxImage.Error);
                                return false;
                            }
                            
                            if (transportSystemsInList.Select(ts => ts.Name).Contains(transportSystemName.Trim())) {
                                ComponentUtils.ShowMessage("В списке уже есть данная транспортная система", MessageBoxImage.Error);
                                return false;
                            }

                            if (!transportSystems.Select(ts => ts.Name).Contains(transportSystemName.Trim())) {
                                ComponentUtils.ShowMessage("Название должно представлять собой одно из названий транспортной системы (предоставленных в выпадающем окошке)", MessageBoxImage.Error);
                                return false;
                            }

                            return true;
                        },
                        RowControl = {
                            TitleValue = "Введите название транспортной системы",
                            Value = "",
                            HelpingValues = transportSystems.Select(ts => ts.Name).ToList()
                        }
                    };
                    return d.ShowDialog() != true ? null : transportSystems.First(ts => ts.Name == d.RowControl.Value);
                },
                Value = _config.TransportSystems
            };
            _transportSystemsControl.AddColumn(
                "Название",
                ts => ts.Name);
            _transportSystemsControl.AddColumn(
                "Кол-во нас. пунктов",
                ts => App.DataBase.CountCitiesOfTransportSystem(ts));
            _transportSystemsControl.AddColumn(
                "Кол-во маршрутов",
                ts => App.DataBase.GetCollection<Road>().Count(r => r.TransportSystemId == ts.Id));
            
            _cityTagsControl = new GenericTableRowControl<CityTag>() {
                TitleValue = "Центральные нас. пункты",
                TitleToolTip = "Представляет собой набор тэгов, нас. пункт имеющий хотя-бы 1 тег из списка будет считаться центральным",
                OnAdd = cityTagsInList => {
                    var d = new StringFieldDialog {
                        Title = "Выбор центральных нас. пунктов",
                        IsViable = tagName => {
                            if (tagName.Trim() == "") {
                                ComponentUtils.ShowMessage("Введите не пустое название", MessageBoxImage.Error);
                                return false;
                            }
                            
                            if (cityTagsInList.Select(ct => ct.Name).Contains(tagName.Trim())) {
                                ComponentUtils.ShowMessage("В списке уже есть данный тэг", MessageBoxImage.Error);
                                return false;
                            }

                            if (!cityTags.Select(ct => ct.Name).Contains(tagName.Trim())) {
                                ComponentUtils.ShowMessage("Название должно представлять собой один из тэгов населенных пунктов (предоставленных в выпадающем окошке)", MessageBoxImage.Error);
                                return false;
                            }

                            return true;
                        },
                        RowControl = {
                            TitleValue = "Введите название тэга",
                            Value = "",
                            HelpingValues = cityTags.Select(ct => ct.Name).ToList()
                        }
                    };
                    return d.ShowDialog() != true ? null : cityTags.First(ct => ct.Name == d.RowControl.Value);;
                },
                Value = _config.CityTags
            };
            _cityTagsControl.AddColumn("Название", ct => ct.Name);
            
            _roadTypesControl = new GenericTableRowControl<RoadType>() {
                TitleValue = "Фильтр не используемых маршрутов",
                TitleToolTip = "Представляет собой набор типов маршрутов, маршрут имеющий тип из списка не будет использоваться в работе алгоритма",
                OnAdd = roadTypesInList => {
                    var d = new StringFieldDialog {
                        Title = "Выбор не используемых маршрутов",
                        IsViable = typeName => {
                            if (typeName.Trim() == "") {
                                ComponentUtils.ShowMessage("Введите не пустое название", MessageBoxImage.Error);
                                return false;
                            }
                            
                            if (roadTypesInList.Select(rt => rt.Name).Contains(typeName.Trim())) {
                                ComponentUtils.ShowMessage("В списке уже есть данный тэг", MessageBoxImage.Error);
                                return false;
                            }

                            if (!roadTypes.Select(rt => rt.Name).Contains(typeName.Trim())) {
                                ComponentUtils.ShowMessage("Название должно представлять собой один из типов маршрутов (предоставленных в выпадающем окошке)", MessageBoxImage.Error);
                                return false;
                            }

                            return true;
                        },
                        RowControl = {
                            TitleValue = "Введите название типа",
                            Value = "",
                            HelpingValues = roadTypes.Select(rt => rt.Name).ToList()
                        }
                    };
                    return d.ShowDialog() != true ? null : roadTypes.First(rt => rt.Name == d.RowControl.Value);;
                },
                Value = _config.RoadTypes
            };
            _roadTypesControl.AddColumn("Название", rt => rt.Name);


            _algorithmTypeControl = new ComboBoxRowControl() {
                TitleValue = "Тип алгоритма",
                TitleToolTip = "Представляет собой используемый тип алгоритма",
            };
            _algorithmTypeControl.AddItem(AlgorithmType.Cost.ToString(),
                "Стоимость проезда", 
                "Алгоритм основанный на затратах на перемещение, " +
                "в качестве веса ребра в итоговом графе будет представляться значение стоимости указанного маршрута");
            _algorithmTypeControl.AddItem(AlgorithmType.Length.ToString(),
                "Длинна маршрута", 
                "Алгоритм основанный на длинне маршрута, " +
                "в качестве веса ребра в итоговом графе будет представляться значение длинны указанного маршрута");
            _algorithmTypeControl.AddItem(AlgorithmType.Time.ToString(),
                "Время маршрута", 
                "Алгоритм основанный на длительности маршрута, " +
                "в качестве веса ребра в итоговом графе будет представляться значение времени поездки на данном маршруте плюс время ожидания отправления данного маршрута");
            _algorithmTypeControl.AddItem(AlgorithmType.ComplexCost.ToString(),
                "Комплексная стоимость маршрута", 
                "Алгоритм основанный на затратах на весь маршрут, " +
                "в качестве веса ребра в итоговом графе будет представляться значение стоимости указанного маршрута плюс " +
                "средняя стоимость проживания в данном нас. пункте умноженное на время ожидания отправления данного маршрута");
            _algorithmTypeControl.Selected = _config.AlgorithmType.ToString();
            
            _methodTypeControl = new ComboBoxRowControl() {
                TitleValue = "Тип метода",
                TitleToolTip = "Представляет собой используемый тип метода",
            };
            _methodTypeControl.AddItem(MethodType.Standard.ToString(),
                "Стандартный метод",
                "Метод где вся сеть предоставляется 1-им графом и каждый маршрут представляются 1-им ребром");
            _methodTypeControl.AddItem(MethodType.Local.ToString(),
                "Локальный метод",
                "Метод включает в себя работу алгоритма поиска кратчайшего пути локально в каждой системе, " +
                "а затем с помощью итеративного алгоритма найти ниулчшие значения для каждого нас. пункта");
            _methodTypeControl.AddItem(MethodType.Another.ToString(),
                "Сторонний метод",
                "Граф представляется в виде матрицы, и алгоритм передает управление стороннему програмному обеспечению");
            _methodTypeControl.Selected = _config.MethodType.ToString();
            

            PropertiesPanel.Children.Add(_transportSystemsControl.GetUiElement);
            PropertiesPanel.Children.Add(_algorithmTypeControl);
            PropertiesPanel.Children.Add(_methodTypeControl);
            PropertiesPanel.Children.Add(_cityTagsControl.GetUiElement);
            PropertiesPanel.Children.Add(_roadTypesControl.GetUiElement);
            
            
            Closed += (sender, args) => CancelClick();
        }

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

        private void UpdateConfig() {
            Enum.TryParse(_algorithmTypeControl.Selected, out AlgorithmType da);
            _config.AlgorithmType = da;
            Enum.TryParse(_methodTypeControl.Selected, out MethodType dm);
            _config.MethodType = dm;
            App.DataBase.GetCollection<AlgorithmConfig>().Update(_config);
        }
    }
}