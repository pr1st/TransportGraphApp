using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Dialogs.TransportSystemDialogs {
    public partial class ListTransportSystemsDialog : Window {
        private IList<TransportSystem> _currentSystemList;

        private EntityListComponent _entityListComponent;

        private StringTextBox _nameBox;

        public ListTransportSystemsDialog() {
            InitializeComponent();
            Owner = AppWindow.Instance;
            Icon = AppResources.GetAppIcon;

            _currentSystemList = AppDataBase.Instance.GetCollection<TransportSystem>().FindAll().ToList();

            _entityListComponent = new EntityListComponent() {
                OnAdd = () => {
                    // if (SystemNameBox.Text == "") {
                    //     ComponentUtils.ShowMessage("Введите название транспортной системы", MessageBoxImage.Error);
                    //     return;
                    // }
                    //
                    // if (_currentSystemList.Select(ts => ts.Name).Contains(SystemNameBox.Text)) {
                    //     ComponentUtils.ShowMessage("Система с таким названием уже существует", MessageBoxImage.Error);
                    //     return;
                    // }
                    //
                    // AppDataBase.Instance.GetCollection<TransportSystem>().Insert(new TransportSystem() {
                    //     Name = SystemNameBox.Text
                    // });
                    // _entityListComponent.UpdateList();
                    PropertiesPanel.Children.Clear();
                    _nameBox = new StringTextBox() {
                        ValueTitle = "Название"
                    };
                    PropertiesPanel.Children.Add(_nameBox);
                    PropertiesPanel.Visibility = Visibility.Visible;
                },
                OnUpdate = () => {
                    // var selectedItem = _entityListComponent.List.SelectedItem;
                    // if (selectedItem == null) {
                    //     ComponentUtils.ShowMessage("Выберите транспортную систему из списка чтобы ее отредактировать", MessageBoxImage.Error);
                    //     return;
                    // }
                    //
                    // PropertiesPanel.Children.Clear();
                    // _nameBox = new StringTextBox() {
                    //     ValueTitle = "Название",
                    //     Value = ((TransportSystem)selectedItem).Name
                    // };
                    // var button = new Button() {
                    //     Content = "Обновить",
                    //     HorizontalAlignment = HorizontalAlignment.Right
                    // };
                    // button.Click += (sender, args) => {
                    //     if (IsViable()) {
                    //         AppDataBase.Instance.GetCollection<TransportSystem>().Update(((TransportSystem) selectedItem).Id);
                    //     }
                    // }
                    // PropertiesPanel.Children.Add(_nameBox);
                    // PropertiesPanel.Children.Add(button);
                    // PropertiesPanel.Visibility = Visibility.Visible;
                },
                OnRemove = () => {
                    var selectedItem = _entityListComponent.List.SelectedItem;
                    if (selectedItem == null) {
                        ComponentUtils.ShowMessage("Выберите транспортную систему из списка чтобы ее удалить", MessageBoxImage.Error);
                        return;
                    }

                    AppDataBase.Instance.GetCollection<TransportSystem>().Delete(((TransportSystem) selectedItem).Id);
                }
            };
            var map = new Dictionary<string, Func<TransportSystem, object>> {
                {"Название", ts => ts.Name},
                {"Кол-во нас. пунктов", ts => 0},
                {"Кол-во маршрутов", ts => "ASD"}
            };

            _entityListComponent.SetUp("Список доступных транспортных систем", map);
            _entityListComponent.UpdateList(_currentSystemList);
            ListPanel.Children.Add(_entityListComponent);

            PropertiesPanel.Visibility = Visibility.Hidden;
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

            DialogResult = true;
        }
    }
}