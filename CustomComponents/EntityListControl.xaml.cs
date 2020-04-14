using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TransportGraphApp.Models;

namespace TransportGraphApp.CustomComponents {
    public partial class EntityListControl : UserControl {
        public EntityListControl() {
            InitializeComponent();
        }
    }

    public class PropertyConverter<T> : IValueConverter {
        public Func<T, object> Supplier { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is T entity) {
                return Supplier.Invoke(entity);
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    
    
    public class GenericEntityListControl<T> where T : IAppModel {
        private readonly EntityListControl _entityListControl = new EntityListControl();

        public GenericEntityListControl(string title, IDictionary<string, Func<T, object>> propertyMatcher, Action<T> onSelectElement) {
            _entityListControl.Title.Content = title;

            foreach (var (columnName, supplier) in propertyMatcher) {
                var column = new GridViewColumn() {
                    Header = columnName,
                    DisplayMemberBinding = new Binding() {
                        Converter = new PropertyConverter<T>() {
                            Supplier = supplier
                        }
                    }
                };
                ((GridView) _entityListControl.List.View).Columns.Add(column);
            }

            _entityListControl.List.SelectionChanged += (sender, args) => {
                if (Selected != null) {
                    onSelectElement.Invoke(Selected);
                }
            };
        }

        public void SetSource(IEnumerable<T> source) {
            _entityListControl.List.ItemsSource = source;
            CollectionViewSource.GetDefaultView(_entityListControl.List.ItemsSource).Refresh();
        }

        public T Selected {
            get => (T) _entityListControl.List.SelectedItem;
            set => _entityListControl.List.SelectedItem = value;
        }

        public UIElement GetUiElement() => _entityListControl;
    }
}