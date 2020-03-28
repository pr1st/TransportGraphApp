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


        private EntityListComponent _entityListComponent;

        private Label _propertiesTitleBox;
        private StringRowControl _nameBox;
        private Button _actionButton;

        public ListTransportSystemsDialog() {
            InitializeComponent();
            Owner = AppWindow.Instance;
            _entityListComponent = new EntityListComponent() {
                OnAdd = AddNewPanelAction,
                OnRemove = RemoveObjectAction
            };
            var propertyMap = new Dictionary<string, Func<TransportSystem, object>> {
                {"Название", ts => ts.Name}, {
                    "Кол-во нас. пунктов",
                    ts => AppDataBase.Instance.GetCollection<City>().Count(c => c.TransportSystemId == ts.Id)
                }, {
                    "Кол-во маршрутов",
                    ts => AppDataBase.Instance.GetCollection<Road>().Count(r => r.TransportSystemId == ts.Id)
                }
            };
            _entityListComponent.SetUp("Список доступных транспортных систем", propertyMap);
            _entityListComponent.List.SelectionChanged += (sender, args) => UpdatePanelAction();

            _propertiesTitleBox = new Label();
            _nameBox = new StringRowControl() {
                Margin = new Thickness(0, 5, 0, 0),
                ValueTitle = "Название"
            };
            _actionButton = new Button() {
                Margin = new Thickness(0, 5, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            PropertiesPanel.Children.Add(_propertiesTitleBox);
            PropertiesPanel.Children.Add(_nameBox);
            PropertiesPanel.Children.Add(_actionButton);
            PropertiesPanel.Visibility = Visibility.Collapsed;

            ListPanel.Children.Add(_entityListComponent);
            UpdateState();
            if (AppGraph.Instance.GetSelectedSystem != null) {
                _entityListComponent.List.SelectedItem =
                    _currentSystemList.First(t => t.Id == AppGraph.Instance.GetSelectedSystem.Id);
            }
        }

        private void AddNewPanelAction() {
            _propertiesTitleBox.Content = "Добавить транспортную систему";
            _nameBox.Value = "";
            _actionButton.Content = "Добавить";
            _actionButton.Click -= UpdateTransportSystem;
            _actionButton.Click -= AddTransportSystem;
            _actionButton.Click += AddTransportSystem;
            _entityListComponent.List.SelectedItem = null;
            PropertiesPanel.Visibility = Visibility.Visible;
        }

        private void UpdatePanelAction() {
            if (_entityListComponent.List.SelectedItem == null) {
                return;
            }

            _propertiesTitleBox.Content = "Обновить транспортную систему";
            _nameBox.Value = ((TransportSystem) _entityListComponent.List.SelectedItem).Name;
            _actionButton.Content = "Обновить";
            _actionButton.Click -= UpdateTransportSystem;
            _actionButton.Click -= AddTransportSystem;
            _actionButton.Click += UpdateTransportSystem;
            PropertiesPanel.Visibility = Visibility.Visible;
        }

        private void AddTransportSystem(object sender, RoutedEventArgs args) {
            if (!IsViable()) return;
            AppDataBase.Instance.GetCollection<TransportSystem>().Insert(new TransportSystem() {
                Name = _nameBox.Value,
                Parameters = new TransportSystemParameters()
            });
            _nameBox.Value = "";
            UpdateState();
        }

        private void UpdateTransportSystem(object sender, RoutedEventArgs args) {
            var selected = (TransportSystem) _entityListComponent.List.SelectedItem;
            if (selected.Name == _nameBox.Value || !IsViable()) {
                return;
            }

            selected.Name = _nameBox.Value;
            AppDataBase.Instance.GetCollection<TransportSystem>().Update(selected);
            UpdateState();
            if (AppGraph.Instance.GetSelectedSystem != null && AppGraph.Instance.GetSelectedSystem.Id == selected.Id) {
                AppGraph.Instance.UpdateSystem();
            }
            _entityListComponent.List.SelectedItem = _currentSystemList.First(t => t.Id == selected.Id);
        }

        private void RemoveObjectAction() {
            var selected = (TransportSystem) _entityListComponent.List.SelectedItem;
            if (selected == null) {
                ComponentUtils.ShowMessage("Выберите транспортную систему из списка чтобы ее удалить",
                    MessageBoxImage.Error);
                return;
            }
            
            AppDataBase.Instance.GetCollection<TransportSystem>()
                .Delete(selected.Id);
            UpdateState();
            if (AppGraph.Instance.GetSelectedSystem != null && AppGraph.Instance.GetSelectedSystem.Id == selected.Id) {
                AppGraph.Instance.SelectSystem(null);
            }
            AddNewPanelAction();
        }

        private void UpdateState() {
            _currentSystemList = AppDataBase.Instance.GetCollection<TransportSystem>().FindAll().ToList();
            _entityListComponent.UpdateList(_currentSystemList);
        }

        private bool IsViable() {
            if (_nameBox.Value == "") {
                ComponentUtils.ShowMessage("Введите название транспортной системы", MessageBoxImage.Error);
                return false;
            }

            if (_currentSystemList.Select(ts => ts.Name).Contains(_nameBox.Value)) {
                ComponentUtils.ShowMessage("Система с таким названием уже существует", MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void SelectClick(object sender, RoutedEventArgs e) {
            if (_entityListComponent.List.SelectedItem == null) {
                ComponentUtils.ShowMessage("Выберите транспортную систему из списка", MessageBoxImage.Error);
                return;
            }

            AppGraph.Instance.SelectSystem((TransportSystem) _entityListComponent.List.SelectedItem);
            DialogResult = true;
        }
    }
}