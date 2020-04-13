using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Dialogs {
    public partial class ListTransportSystemsDialog : Window {
        private IList<TransportSystem> _currentSystemList;

        private GenericEntityListControl<TransportSystem> _entityList;


        private StackPanel _newPanel;
        private StringRowControl _newNameControl;
        private StringTableRowControl _newAvailableRoadTypesControl;

        private StackPanel _updatePanel;
        private StringRowControl _updateNameControl;
        private StringTableRowControl _updateAvailableRoadTypesControl;

        public ListTransportSystemsDialog() {
            InitializeComponent();
            Owner = App.Window;
            Icon = AppResources.GetAppIcon;

            // var propertyMatcher = new Dictionary<string, Func<TransportSystem, object>> {
            //     {"Название", ts => ts.Name}, {
            //         "Кол-во нас. пунктов",
            //         ts => AppDataBase.Instance.GetCollection<City>().Count(c => c.TransportSystemId == ts.Id)
            //     }, {
            //         "Кол-во маршрутов",
            //         ts => AppDataBase.Instance.GetCollection<Road>().Count(r => r.TransportSystemId == ts.Id)
            //     }
            // };

            // _entityList = new GenericEntityListControl<TransportSystem>(
            //     "Список доступных транспортных систем",
            //     propertyMatcher,
            //     DisplayNew,
            //     DisplayUpdate);

            SetUpNewPropertiesPanel();
            SetUpUpdatePropertiesPanel();
            PropertiesPanel.Visibility = Visibility.Collapsed;
            ListPanel.Children.Add(_entityList.GetUiElement());
            UpdateState();

            // var selected = AppGraph.Instance.GetSelectedSystem;
            // if (selected != null) {
            //     _entityList.Selected = _currentSystemList.First(t => t.Id == selected.Id);
            // }
        }


        private void SetUpNewPropertiesPanel() {
            _newPanel = new StackPanel();
            var label = new Label() {
                Margin = new Thickness(5, 5, 5, 5),
                Content = "Добавить транспортную систему"
            };
            _newNameControl = new StringRowControl() {
                TitleValue = "Название"
            };
            _newAvailableRoadTypesControl = new StringTableRowControl() {
                TitleValue = "Используемые типы дорог",
                IsViable = (adding, roadTypes) => {
                    if (adding.Trim() == "") {
                        ComponentUtils.ShowMessage("Введите название для нового типа дорог", MessageBoxImage.Error);
                        return false;
                    }

                    if (roadTypes.Contains(adding.Trim())) {
                        ComponentUtils.ShowMessage("Тип дороги с таким названием уже существует",
                            MessageBoxImage.Error);
                        return false;
                    }

                    return true;
                }
            };
            var addButton = new Button() {
                Margin = new Thickness(5,5,5,5),
                Content = "Добавить",
                HorizontalAlignment = HorizontalAlignment.Right
            };
            addButton.Click += (sender, args) => AddTransportSystem();

            _newPanel.Children.Add(label);
            _newPanel.Children.Add(_newNameControl);
            _newPanel.Children.Add(_newAvailableRoadTypesControl);
            _newPanel.Children.Add(addButton);
        }

        private void SetUpUpdatePropertiesPanel() {
            _updatePanel = new StackPanel();
            var label = new Label() {
                Margin = new Thickness(5, 5, 5, 5),
                Content = "Обновить транспортную систему"
            };
            _updateNameControl = new StringRowControl() {
                TitleValue = "Название"
            };
            _updateAvailableRoadTypesControl = new StringTableRowControl() {
                TitleValue = "Используемые типы дорог",
                IsViable = (adding, roadTypes) => {
                    if (adding.Trim() == "") {
                        ComponentUtils.ShowMessage("Введите название для нового типа дорог", MessageBoxImage.Error);
                        return false;
                    }

                    if (roadTypes.Contains(adding.Trim())) {
                        ComponentUtils.ShowMessage("Тип дороги с таким названием уже существует",
                            MessageBoxImage.Error);
                        return false;
                    }

                    return true;
                }
            };
            var buttonPanel = new WrapPanel() {
                Margin = new Thickness(5, 5, 5, 5),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            var removeButton = new Button() {
                Content = "Удалить",
                Margin = new Thickness(0, 0, 5, 0),
            };
            removeButton.Click += (sender, args) => RemoveTransportSystem();
            var updateButton = new Button() {
                Content = "Обновить"
            };
            updateButton.Click += (sender, args) => UpdateTransportSystem();
            buttonPanel.Children.Add(removeButton);
            buttonPanel.Children.Add(updateButton);
            
            _updatePanel.Children.Add(label);
            _updatePanel.Children.Add(_updateNameControl);
            _updatePanel.Children.Add(_updateAvailableRoadTypesControl);
            _updatePanel.Children.Add(buttonPanel);
        }
        
        private void DisplayNew() {
            _newNameControl.Value = "";
            _newAvailableRoadTypesControl.Value = new List<string>();
            _entityList.Selected = null;
            
            PropertiesPanel.Children.Clear();
            PropertiesPanel.Children.Add(_newPanel);
            PropertiesPanel.Visibility = Visibility.Visible;
        }

        private void DisplayUpdate(TransportSystem ts) {
            _updateNameControl.Value = ts.Name;
            _updateAvailableRoadTypesControl.Value = ts.Parameters.AvailableRoadTypes;
            
            PropertiesPanel.Children.Clear();
            PropertiesPanel.Children.Add(_updatePanel);
            PropertiesPanel.Visibility = Visibility.Visible;
        }

        private void AddTransportSystem() {
            if (_newNameControl.Value == "") {
                ComponentUtils.ShowMessage("Введите название транспортной системы", MessageBoxImage.Error);
                return;
            }

            if (_currentSystemList.Select(ts => ts.Name).Contains(_newNameControl.Value)) {
                ComponentUtils.ShowMessage("Система с таким названием уже существует", MessageBoxImage.Error);
                return;
            }
            
            // AppDataBase.Instance.GetCollection<TransportSystem>().Insert(new TransportSystem() {
            //     Name = _newNameControl.Value,
            //     Parameters = new TransportSystemParameters() {
            //         AvailableRoadTypes = _newAvailableRoadTypesControl.Value
            //     }
            // });
            UpdateState();
            DisplayNew();
        }

        private void UpdateTransportSystem() {
            var selected = _entityList.Selected;
            if (_updateNameControl.Value == "") {
                ComponentUtils.ShowMessage("Введите не пустое название транспортной системы", MessageBoxImage.Error);
                return;
            }

            if (selected.Name != _updateNameControl.Value && 
                _currentSystemList.Select(ts => ts.Name).Contains(_updateNameControl.Value)) {
                ComponentUtils.ShowMessage("Система с таким названием уже существует", MessageBoxImage.Error);
                return;
            }

            selected.Name = _updateNameControl.Value;
            selected.Parameters.AvailableRoadTypes = (_updateAvailableRoadTypesControl).Value;
            
            // AppDataBase.Instance.GetCollection<TransportSystem>().Update(selected);
            // UpdateState();
            // if (AppGraph.Instance.GetSelectedSystem != null && AppGraph.Instance.GetSelectedSystem.Id == selected.Id) {
            //     AppGraph.Instance.UpdateSystem();
            // }
            _entityList.Selected = _currentSystemList.First(t => t.Id == selected.Id);
            ComponentUtils.ShowMessage("Данная транспортная система была обновлена", MessageBoxImage.Information);
        }

        private void RemoveTransportSystem() {
            var selected = _entityList.Selected;

            // AppDataBase.Instance.GetCollection<TransportSystem>().Delete(selected.Id);
            // UpdateState();
            // if (AppGraph.Instance.GetSelectedSystem != null && AppGraph.Instance.GetSelectedSystem.Id == selected.Id) {
            //     AppGraph.Instance.SelectSystem(null);
            // }

            DisplayNew();
        }

        private void UpdateState() {
            // _currentSystemList = AppDataBase.Instance.GetCollection<TransportSystem>().FindAll().ToList();
            _entityList.SetSource(_currentSystemList);
        }

        private void SelectClick(object sender, RoutedEventArgs e) {
            if (_entityList.Selected == null) {
                ComponentUtils.ShowMessage("Выберите транспортную систему из списка", MessageBoxImage.Error);
                return;
            }

            // AppGraph.Instance.SelectSystem(_entityList.Selected);
            DialogResult = true;
        }
    }
}