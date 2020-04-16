using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using LiteDB;
using Microsoft.Win32;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TransportGraphApp.Dialogs {
    public partial class ListCitiesDialog : Window {
        private IList<City> _currentCitiesList;
        private TransportSystem _selectedTransportSystem;

        private GenericEntityListControl<City> _entityList;

        private StackPanel _newPanel;
        private StringRowControl _newNameControl;
        private LatitudeLongitudeRowControl _newCooridnatesControl;
        private PositiveDoubleRowControl _newCostOfStayingControl;

        private StackPanel _updatePanel;
        private StringRowControl _updateNameControl;
        private LatitudeLongitudeRowControl _updateCooridnatesControl;
        private PositiveDoubleRowControl _updateCostOfStayingControl;
        
        public ListCitiesDialog(TransportSystem transportSystem) {
            InitializeComponent();
            Owner = App.Window;
            Icon = AppResources.GetAppIcon;
            _selectedTransportSystem = transportSystem;

            var propertyMatcher = new Dictionary<string, Func<City, object>> {
                {"Название", c => c.Name}, 
                // {"Широта", c => c.Latitude}, 
                // {"Долгота", c => c.Longitude},
                {"Стоимость проживания", c => c.CostOfStaying},
            };

            _entityList = new GenericEntityListControl<City>(
                "Доступные населенные пункты",
                propertyMatcher,
                DisplayUpdate);

            SetUpNewPropertiesPanel();
            SetUpUpdatePropertiesPanel();
            PropertiesPanel.Visibility = Visibility.Collapsed;
            ListPanel.Children.Add(_entityList.GetUiElement());
            
            ComponentUtils.InsertIconToButton(AddButton, AppResources.GetAddItemIcon, "Открыть окно для добавления транспортной системы");
            AddButton.Click += (sender, args) => DisplayNew();
            
            UpdateState();
        }
        
        private void SetUpNewPropertiesPanel() {
            _newPanel = new StackPanel();
            var labelPanel = new WrapPanel() {
                Margin = new Thickness(5, 5, 5, 5)
            };
            var label = new Label() {
                Margin = new Thickness(0, 0, 5, 0),
                Content = "Добавить населенный пункт"
            };
            var importButton = new Button() {
                VerticalAlignment = VerticalAlignment.Center,
                Content = "Добавить из файла"
            };
            importButton.Click += (sender, args) => ImportFromFileClick();
            labelPanel.Children.Add(label);
            labelPanel.Children.Add(importButton);
            _newNameControl = new StringRowControl() {
                TitleValue = "Название"
            };
            _newCooridnatesControl = new LatitudeLongitudeRowControl();
            _newCostOfStayingControl = new PositiveDoubleRowControl() {
                TitleValue = "Стоимость проживания в данном населенном пункте (условных единиц в день)"
            };
            var addButton = new Button() {
                Margin = new Thickness(5,5,5,5),
                Content = "Добавить",
                HorizontalAlignment = HorizontalAlignment.Right
            };
            addButton.Click += (sender, args) => AddCity();

            _newPanel.Children.Add(labelPanel);
            _newPanel.Children.Add(_newNameControl);
            _newPanel.Children.Add(_newCooridnatesControl);
            _newPanel.Children.Add(_newCostOfStayingControl);
            _newPanel.Children.Add(new Separator() {
                Margin = new Thickness(5,5,5,5),
            });
            _newPanel.Children.Add(addButton);
        }
        
        private void SetUpUpdatePropertiesPanel() {
            _updatePanel = new StackPanel();
            var label = new Label() {
                Margin = new Thickness(5, 5, 5, 5),
                Content = "Обновить населенный пункт"
            };
            _updateNameControl = new StringRowControl() {
                TitleValue = "Название"
            };
            _updateCooridnatesControl = new LatitudeLongitudeRowControl();
            _updateCostOfStayingControl = new PositiveDoubleRowControl() {
                TitleValue = "Стоимость проживания в данном населенном пункте (условных единиц в день)"
            };
            var buttonPanel = new WrapPanel() {
                Margin = new Thickness(5, 5, 5, 5),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            var removeButton = new Button() {
                Content = "Удалить",
                Margin = new Thickness(0, 0, 5, 0),
            };
            removeButton.Click += (sender, args) => RemoveCity();
            var updateButton = new Button() {
                Content = "Обновить"
            };
            updateButton.Click += (sender, args) => UpdateCity();
            buttonPanel.Children.Add(removeButton);
            buttonPanel.Children.Add(updateButton);

            _updatePanel.Children.Add(label);
            _updatePanel.Children.Add(_updateNameControl);
            _updatePanel.Children.Add(_updateCooridnatesControl);
            _updatePanel.Children.Add(_updateCostOfStayingControl);
            _updatePanel.Children.Add(new Separator() {
                Margin = new Thickness(5,5,5,5),
            });
            _updatePanel.Children.Add(buttonPanel);
        }
        
        private void DisplayNew() {
            _newNameControl.Value = "";
            _newCooridnatesControl.Latitude = 0;
            _newCooridnatesControl.Longitude = 0;
            _newCostOfStayingControl.Value = 0;
            _entityList.Selected = null;
            
            PropertiesPanel.Children.Clear();
            PropertiesPanel.Children.Add(_newPanel);
            PropertiesPanel.Visibility = Visibility.Visible;
        }
        
        private void DisplayUpdate(City c) {
            _updateNameControl.Value = c.Name;
            // _updateCooridnatesControl.Latitude = c.Latitude;
            // _updateCooridnatesControl.Longitude = c.Longitude;
            _updateCostOfStayingControl.Value = c.CostOfStaying;
            
            PropertiesPanel.Children.Clear();
            PropertiesPanel.Children.Add(_updatePanel);
            PropertiesPanel.Visibility = Visibility.Visible;
        }
        
        private void AddCity() {
            if (_newNameControl.Value == "") {
                ComponentUtils.ShowMessage("Введите название населенного пункта", MessageBoxImage.Error);
                return;
            }

            if (_currentCitiesList.Select(с => с.Name).Contains(_newNameControl.Value)) {
                ComponentUtils.ShowMessage("Населенный пункт с таким названием уже существует", MessageBoxImage.Error);
                return;
            }
            
            App.DataBase.GetCollection<City>().Insert(new City() {
                Name = _newNameControl.Value,
                // Latitude = _newCooridnatesControl.Latitude,
                // Longitude = _newCooridnatesControl.Longitude,
                CostOfStaying = _newCostOfStayingControl.Value,
                // TransportSystemId = _selectedTransportSystem.Id
            });
            UpdateState();
            DisplayNew();
        }
        
        private void UpdateCity() {
            var selected = _entityList.Selected;
            if (_updateNameControl.Value == "") {
                ComponentUtils.ShowMessage("Введите не пустое название населенного пункта", MessageBoxImage.Error);
                return;
            }

            if (selected.Name != _updateNameControl.Value && 
                _currentCitiesList.Select(с => с.Name).Contains(_updateNameControl.Value)) {
                ComponentUtils.ShowMessage("Населенный пункт с таким названием уже существует", MessageBoxImage.Error);
                return;
            }

            selected.Name = _updateNameControl.Value;
            // selected.Latitude = _updateCooridnatesControl.Latitude;
            // selected.Longitude = _updateCooridnatesControl.Longitude;
            selected.CostOfStaying = _updateCostOfStayingControl.Value;
            
            App.DataBase.GetCollection<City>().Update(selected);
            UpdateState();
            
            _entityList.Selected = _currentCitiesList.First(c => c.Id == selected.Id);
            ComponentUtils.ShowMessage("Данный населенный пункт был обновлен", MessageBoxImage.Information);
        }
        
        private void RemoveCity() {
            var selected = _entityList.Selected;
            App.DataBase.GetCollection<City>().Delete(selected.Id);
            UpdateState();
            DisplayNew();
        }
        
        private void UpdateState() {
            // _currentCitiesList = App.DataBase
            //     .GetCollection<City>()
            //     .Find(c => c.TransportSystemId == _selectedTransportSystem.Id)
            //     .ToList();
            _entityList.SetSource(_currentCitiesList);
        }
        
        private void ImportFromFileClick() {
            var sb = new StringBuilder();
            var msg1 = "Файл должен быть в json формате";
            var msg2 = "В файле корневой элемент должен быть массив состоящий из нас. пунктов";
            var msg3 = "Поля которые используются:";
            var msg4 = "Name - нименование нас. пункта";
            var msg5 = "Latitude - широта";
            var msg6 = "Longitude - долгота";
            var msg7 = "CostOfStaying - Стоимость проживания в городе";
            var msg8 = "Пример корректного файла:";
            var msg9 =
                "[\n" +
                "    {\n" +
                "        \"Name\":\"Адыгейск\",\n" +
                "        \"Latitude\":44.878414,\n" +
                "        \"Longitude\":39.190289,\n" +
                "        \"CostOfStaying\":123\n" +
                "    },\n" +
                "    {\n" +
                "        \"Name\":\"Майкоп\",\n" +
                "        \"Latitude\":44.6098268,\n" +
                "        \"Longitude\":40.1006527,\n" +
                "        \"CostOfStaying\":234\n" +
                "    }\n" +
                "]";
            sb.AppendLine(msg1);
            sb.AppendLine(msg2);
            sb.AppendLine(msg3);
            sb.AppendLine(msg4);
            sb.AppendLine(msg5);
            sb.AppendLine(msg6);
            sb.AppendLine(msg7);
            sb.AppendLine(msg8);
            sb.AppendLine(msg9);
            ComponentUtils.ShowMessage(sb.ToString(), MessageBoxImage.Information);

            var openFileDialog = new OpenFileDialog() {
                Filter = "json files (*.json)|*.json",
                InitialDirectory = Directory.GetCurrentDirectory()
            };
            
            if (openFileDialog.ShowDialog() != true) return;

            try {
                var cities = JsonSerializer.Deserialize<IList<City>>(File.ReadAllText(openFileDialog.FileName));
                var allNames = cities
                    .Select(c => c.Name)
                    .Concat(_currentCitiesList.Select(c => c.Name))
                    .ToList();
                if (allNames.Distinct().Count() != allNames.Count()) {
                    ComponentUtils.ShowMessage("Выбранной файл содержит названия нас. пунктов которые уже есть в списке " +
                                               "или сам содержит дубликаты названий нас. пунктов", MessageBoxImage.Error);
                    return;
                }
                foreach (var c in cities) {
                    // c.TransportSystemId = _selectedTransportSystem.Id;
                    App.DataBase.GetCollection<City>().Insert(c);   
                }
                UpdateState();
                DisplayNew();
            }
            catch (JsonException) {
                ComponentUtils.ShowMessage("Выбранной файл представлен в неверном формате", MessageBoxImage.Error);
            } 
        }
    }
}