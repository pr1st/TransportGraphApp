using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TransportGraphApp.CustomComponents {
    public partial class TableRowControl : UserControl {
        public TableRowControl() {
            InitializeComponent();
            ComponentUtils.InsertIconToButton(AddButton, AppResources.GetAddItemIcon, "Добавить");
            ComponentUtils.InsertIconToButton(RemoveButton, AppResources.GetRemoveItemIcon, "Удалить");
        }
    }

    public class GenericTableRowControl<T> {
        private readonly TableRowControl _table = new TableRowControl();

        private IList<T> _values = new List<T>();

        public string TitleValue {
            get => _table.TableTitle.Text;
            set => _table.TableTitle.Text = value;
        }

        public string TitleToolTip {
            get => (string) _table.TableTitle.ToolTip;
            set => _table.TableTitle.ToolTip = value;
        }

        public IList<T> Value {
            get => _values;
            set {
                _values = value;
                _table.ItemList.ItemsSource = _values;
                CollectionViewSource.GetDefaultView(_table.ItemList.ItemsSource).Refresh();
            }
        }

        public void AddColumns(IDictionary<string, Func<T, object>> columns) {
            foreach (var (columnName, columnMatcher) in columns) {
                var column = new GridViewColumn() {
                    Header = columnName,
                    DisplayMemberBinding = new Binding() {
                        Converter = new PropertyConverter<T>() {
                            Supplier = columnMatcher
                        }
                    }
                };
                ((GridView) _table.ItemList.View).Columns.Add(column);
            }
        }

        private Func<IList<T>, IList<T>> _onAdd;
        public Func<IList<T>, IList<T>> OnAdd {
            get => _onAdd;
            set {
                if (value == null) {
                    _table.AddButton.Visibility = Visibility.Collapsed;
                    _table.RemoveButton.Visibility = Visibility.Collapsed;
                }
                else {
                    _table.AddButton.Visibility = Visibility.Visible;
                    _table.RemoveButton.Visibility = Visibility.Visible;
                }

                _onAdd = value;
            }
        }

        public GenericTableRowControl() {
            _table.ItemList.ItemsSource = _values;

            OnAdd = null;
            _table.AddButton.Click += (sender, args) => {
                var addedElements = OnAdd.Invoke(_values);
                if (addedElements == null) return;

                foreach (var element in addedElements) {
                    _values.Add(element);
                }

                CollectionViewSource.GetDefaultView(_table.ItemList.ItemsSource).Refresh();
            };

            _table.RemoveButton.Click += (sender, args) => {
                if (_table.ItemList.SelectedItem == null) return;

                foreach (var selectedItem in _table.ItemList.SelectedItems) {
                    _values.Remove((T) selectedItem);
                }

                CollectionViewSource.GetDefaultView(_table.ItemList.ItemsSource).Refresh();
            };
        }

        public UIElement GetUiElement => _table;
    }
}