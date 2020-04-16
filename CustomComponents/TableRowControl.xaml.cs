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
            get => (string)_table.TableTitle.ToolTip;
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

        public void AddColumn(string columnName, Func<T, object> columnMatcher) {
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
        
        public Func<IList<T>, T> OnAdd { get; set; }
        
        public GenericTableRowControl() {
            _table.ItemList.ItemsSource = _values;
            
            _table.AddButton.Click += (sender, args) => {
                var addedElement = OnAdd.Invoke(_values);
                if (addedElement == null) return;

                _values.Add(addedElement);
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