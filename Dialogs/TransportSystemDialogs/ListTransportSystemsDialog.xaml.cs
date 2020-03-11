using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TransportGraphApp.Actions.TransportSystemActions;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Dialogs.TransportSystemDialogs {
    public partial class ListTransportSystemsDialog : Window {
        public TransportSystem SelectedSystem => (TransportSystem) TransportSystemList.SelectedItem;

        private IList<TransportSystem> _currentSystemList;
        private readonly Func<IEnumerable<TransportSystem>> _systemsSupplier;
        private readonly Func<TransportSystem, int> _numberOfCitiesSupplier;

        public ListTransportSystemsDialog(
            Func<IEnumerable<TransportSystem>> systemsSupplier,
            Func<TransportSystem, int> numberOfCitiesSupplier) {
            InitializeComponent();
            Owner = AppWindow.Instance;
            Icon = AppResources.GetAppIcon;
            _systemsSupplier = systemsSupplier;
            _numberOfCitiesSupplier = numberOfCitiesSupplier;

            var numberOfCitiesColumn = new GridViewColumn() {
                Header = "Кол-во нас. пунктов",
                DisplayMemberBinding = new Binding() {
                    Converter = new NumberOfCities() {
                        Supplier = _numberOfCitiesSupplier
                    }
                }
            };
            ((GridView) TransportSystemList.View).Columns.Add(numberOfCitiesColumn);

            ConfigureButtons();
            UpdateState();
        }

        private void ConfigureButtons() {
            var addButton = new IconButton(AppResources.GetAddItemIcon, () => {
                    if (SystemNameBox.Text == "") {
                        ComponentUtils.ShowMessage("Введите имя транспортной системы", MessageBoxImage.Error);
                        return;
                    }

                    if (_currentSystemList.Select(ts => ts.Name).Contains(SystemNameBox.Text)) {
                        ComponentUtils.ShowMessage("Система с таким именем уже существует", MessageBoxImage.Error);
                        return;
                    }

                    AppActions.Instance.GetAction<CreateTransportSystemAction>().Invoke(new TransportSystem() {
                        Name = SystemNameBox.Text
                    });
                    UpdateState();
                })
                {ToolTip = "Добавить транспортную систему"};
            AddButtonPanel.Children.Add(addButton);

            var deleteButton = new IconButton(AppResources.GetRemoveItemIcon, () => {
                    var selected = TransportSystemList.SelectedItem;
                    if (selected == null) {
                        return;
                    }

                    foreach (var ts in TransportSystemList.SelectedItems) {
                        AppActions.Instance.GetAction<DeleteTransportSystemAction>().Invoke((TransportSystem) ts);
                    }

                    UpdateState();
                })
                {ToolTip = "Удалить транспортную систему"};
            RemoveButtonPanel.Children.Add(deleteButton);
        }

        private void UpdateState() {
            SystemNameBox.Text = "";
            _currentSystemList = _systemsSupplier.Invoke().ToList();
            TransportSystemList.ItemsSource = _currentSystemList;
            CollectionViewSource.GetDefaultView(TransportSystemList.ItemsSource).Refresh();
        }

        private void SelectClick(object sender, RoutedEventArgs e) {
            if (TransportSystemList.SelectedItem == null) {
                ComponentUtils.ShowMessage("Выберите систему из списка", MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
        }
    }

    public class NumberOfCities : IValueConverter {
        public Func<TransportSystem, int> Supplier { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is TransportSystem ts) {
                return Supplier.Invoke(ts);
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}