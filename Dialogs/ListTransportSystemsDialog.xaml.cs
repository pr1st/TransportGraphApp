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

        private Label _propertiesTitleLabel;
        private StringRowControl _nameControl;
        private StringTableRowControl _availableRoadTypesControl;
        private Button _actionButton1;
        private Button _actionButton2;

        public ListTransportSystemsDialog() {
            InitializeComponent();
            Owner = AppWindow.Instance;

            var propertyMatcher = new Dictionary<string, Func<TransportSystem, object>> {
                {"Название", ts => ts.Name}, {
                    "Кол-во нас. пунктов",
                    ts => AppDataBase.Instance.GetCollection<City>().Count(c => c.TransportSystemId == ts.Id)
                }, {
                    "Кол-во маршрутов",
                    ts => AppDataBase.Instance.GetCollection<Road>().Count(r => r.TransportSystemId == ts.Id)
                }
            };

            _entityList = new GenericEntityListControl<TransportSystem>(
                "Список доступных транспортных систем",
                propertyMatcher,
                DisplayNew,
                DisplayUpdate);


            _propertiesTitleLabel = new Label() {
                Margin = new Thickness(5, 5, 5, 5)
            };
            _nameControl = new StringRowControl() {
                TitleValue = "Название"
            };
            _availableRoadTypesControl = new StringTableRowControl() {
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
            _actionButton1 = new Button() {
                Margin = new Thickness(0, 0, 5, 0),
            };
            _actionButton2 = new Button();
            buttonPanel.Children.Add(_actionButton1);
            buttonPanel.Children.Add(_actionButton2);

            PropertiesPanel.Children.Add(_propertiesTitleLabel);
            PropertiesPanel.Children.Add(_nameControl);
            PropertiesPanel.Children.Add(_availableRoadTypesControl);
            PropertiesPanel.Children.Add(buttonPanel);
            PropertiesPanel.Visibility = Visibility.Collapsed;

            ListPanel.Children.Add(_entityList.GetUiElement());
            UpdateState();

            var selected = AppGraph.Instance.GetSelectedSystem;
            if (selected != null) {
                _entityList.Selected = _currentSystemList.First(t => t.Id == selected.Id);
            }
        }

        private void DisplayNew() {
            _propertiesTitleLabel.Content = "Добавить транспортную систему";
            _nameControl.Value = "";
            _availableRoadTypesControl.Value = new List<string>();

            _actionButton1.Visibility = Visibility.Hidden;
            _actionButton2.Content = "Добавить";
            ClearFromEvents(_actionButton2);
            _actionButton2.Click += AddTransportSystem;

            _entityList.Selected = null;
            PropertiesPanel.Visibility = Visibility.Visible;
        }

        private void DisplayUpdate(TransportSystem ts) {
            _propertiesTitleLabel.Content = "Обновить транспортную систему";
            _nameControl.Value = ts.Name;
            _availableRoadTypesControl.Value = ts.Parameters.AvailableRoadTypes;
            
            _actionButton1.Visibility = Visibility.Visible;
            _actionButton1.Content = "Удалить";
            ClearFromEvents(_actionButton1);
            _actionButton1.Click += RemoveTransportSystem;
            
            _actionButton2.Content = "Обновить";
            ClearFromEvents(_actionButton2);
            _actionButton2.Click += UpdateTransportSystem;
            
            PropertiesPanel.Visibility = Visibility.Visible;
        }

        private void AddTransportSystem(object sender, RoutedEventArgs args) {
            if (!IsViable()) return;
            AppDataBase.Instance.GetCollection<TransportSystem>().Insert(new TransportSystem() {
                Name = _nameControl.Value,
                Parameters = new TransportSystemParameters() {
                    AvailableRoadTypes = _availableRoadTypesControl.Value
                }
            });
            UpdateState();
            DisplayNew();
        }

        private void UpdateTransportSystem(object sender, RoutedEventArgs args) {
            var selected = _entityList.Selected;
            if (!IsViable(selected)) {
                return;
            }

            selected.Name = _nameControl.Value;
            selected.Parameters.AvailableRoadTypes = _availableRoadTypesControl.Value;
            
            AppDataBase.Instance.GetCollection<TransportSystem>().Update(selected);
            UpdateState();
            if (AppGraph.Instance.GetSelectedSystem != null && AppGraph.Instance.GetSelectedSystem.Id == selected.Id) {
                AppGraph.Instance.UpdateSystem();
            }
            _entityList.Selected = _currentSystemList.First(t => t.Id == selected.Id);
            ComponentUtils.ShowMessage("Данная транспортная система была обновлена", MessageBoxImage.Information);
        }

        private void RemoveTransportSystem(object sender, RoutedEventArgs args) {
            var selected = _entityList.Selected;

            AppDataBase.Instance.GetCollection<TransportSystem>().Delete(selected.Id);
            UpdateState();
            if (AppGraph.Instance.GetSelectedSystem != null && AppGraph.Instance.GetSelectedSystem.Id == selected.Id) {
                AppGraph.Instance.SelectSystem(null);
            }

            DisplayNew();
        }

        private void ClearFromEvents(Button button) {
            button.Click -= UpdateTransportSystem;
            button.Click -= AddTransportSystem;
            button.Click -= RemoveTransportSystem;
        }
        
        private void UpdateState() {
            _currentSystemList = AppDataBase.Instance.GetCollection<TransportSystem>().FindAll().ToList();
            _entityList.SetSource(_currentSystemList);
        }

        private bool IsViable(TransportSystem previousSystem = null) {
            if (_nameControl.Value == "") {
                ComponentUtils.ShowMessage("Введите название транспортной системы", MessageBoxImage.Error);
                return false;
            }

            if (previousSystem != null && 
                previousSystem.Name != _nameControl.Value && 
                _currentSystemList.Select(ts => ts.Name).Contains(_nameControl.Value)) {
                ComponentUtils.ShowMessage("Система с таким названием уже существует", MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void SelectClick(object sender, RoutedEventArgs e) {
            if (_entityList.Selected == null) {
                ComponentUtils.ShowMessage("Выберите транспортную систему из списка", MessageBoxImage.Error);
                return;
            }

            AppGraph.Instance.SelectSystem(_entityList.Selected);
            DialogResult = true;
        }
    }
}