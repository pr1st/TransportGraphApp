using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Models;

namespace TransportGraphApp.Dialogs {
    public partial class TaskResultsDialog : Window {
        public TaskResultsDialog() {
            InitializeComponent();
            Owner = App.Window;
            Icon = AppResources.GetAppIcon;
            
            var propertyMatcher = new Dictionary<string, Func<AlgorithmResult, object>> {
                {
                    "Дата выполнения", 
                    res => res.RunDate
                },
                {
                   "Метод",
                   res => res.AlgorithmConfig.MethodType
                },
                {
                  "Алгоритм",
                  res => res.AlgorithmConfig.AlgorithmType
                },
                {
                    "Кол-во. нас. пунктов",
                    res => res.Cities.Count
                }
            };
            var entityList = new GenericEntityListControl<AlgorithmResult>(
                "Результаты",
                propertyMatcher,
                DisplayProperties);
            
            ListPanel.Children.Add(entityList.GetUiElement());
            entityList.SetSource(App.DataBase.GetCollection<AlgorithmResult>().FindAll());
            
            ComponentUtils.InsertIconToButton(RemoveButton, AppResources.GetRemoveItemIcon, "Удалить результат из списка");
            RemoveButton.Click += (sender, args) => {
                if (entityList.Selected == null) return;
                App.DataBase.GetCollection<AlgorithmResult>().Delete(entityList.Selected.Id);
                entityList.SetSource(App.DataBase.GetCollection<AlgorithmResult>().FindAll());
                VisibilityPanel.Visibility = Visibility.Collapsed;
            };
        }

        public void DisplayProperties(AlgorithmResult res) {
            PropertiesPanel.Children.Clear();
            
            var cfgAlgType = new ConstantStringRowControl() {
                TitleValue = "Тип алгоритма",
                Value = res.AlgorithmConfig.AlgorithmType.ToString()
            };
            var cfgMetType = new ConstantStringRowControl() {
                TitleValue = "Тип метода",
                Value = res.AlgorithmConfig.MethodType.ToString()
            };
            var cfgTransportSystems = new ConstantStringRowControl() {
                TitleValue = "Использованные транспортные системы",
                Value = string.Join(", ", res.AlgorithmConfig.TransportSystems.Select(ts => ts.Name))
            };
            var cfgCityTags = new ConstantStringRowControl() {
                TitleValue = "Центральные города",
                Value = string.Join(", ", res.AlgorithmConfig.CityTags.Select(ct => ct.Name))
            };
            var cfgRoadTypes = new ConstantStringRowControl() {
                TitleValue = "Исключенные дороги",
                Value = string.Join(", ", res.AlgorithmConfig.RoadTypes.Select(rt => rt.Name))
            };
            
            var propertyMatcher = new Dictionary<string, Func<City, object>> {
                {
                    "Название", 
                    c => c.Name 
                },
                {
                    "Итоговое значение",
                    c => res.Values[res.Cities.IndexOf(c)]
                }
            };
            var entityList = new GenericEntityListControl<City>(
                "Таблица результатов",
                propertyMatcher,
                city => { });
            
            entityList.SetSource(res.Cities);

            PropertiesPanel.Children.Add(cfgAlgType);
            PropertiesPanel.Children.Add(cfgMetType);
            PropertiesPanel.Children.Add(cfgTransportSystems);
            PropertiesPanel.Children.Add(cfgCityTags);
            PropertiesPanel.Children.Add(cfgRoadTypes);
            PropertiesPanel.Children.Add(entityList.GetUiElement());
            
            VisibilityPanel.Visibility = Visibility.Visible;
        }
    }
}