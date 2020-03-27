using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using TransportGraphApp.Models;

namespace TransportGraphApp.CustomComponents {
    public partial class EntityListComponent : UserControl {
        public EntityListComponent() {
            InitializeComponent();
        }

        public ThreadStart OnAdd { get; set; }
        public ThreadStart OnUpdate { get; set; }
        public ThreadStart OnRemove { get; set; }

        public void SetUp<T>(string title,
            IDictionary<string, Func<T, object>> propertyGetter) where T: IAppModel {
            Title.Content = title;
            foreach (var (key, value) in propertyGetter) {
                var column = new GridViewColumn() {
                    Header = key,
                    DisplayMemberBinding = new Binding() {
                        Converter = new PropertyConverter<T>() {
                            Supplier = value
                        }
                    }
                };
                ((GridView) List.View).Columns.Add(column);
            }
            ConfigureButtons();
        }

        public void UpdateList(IEnumerable<IAppModel> enumerable) {
            List.ItemsSource = enumerable;
            CollectionViewSource.GetDefaultView(List.ItemsSource).Refresh();
        }

        private void ConfigureButtons() {
            if (OnAdd != null) {
                var addButton = new IconButton(AppResources.GetAddItemIcon, OnAdd)
                    {ToolTip = "Добавить объект"};
                ButtonPanel.Children.Add(addButton);
            }

            if (OnUpdate != null) {
                var updateButton = new IconButton(AppResources.GetUpdateItemIcon, OnUpdate)
                    {ToolTip = "Обновить объект"};
                ButtonPanel.Children.Add(updateButton);
            }

            if (OnRemove != null) {
                var removeButton = new IconButton(AppResources.GetRemoveItemIcon, OnRemove)
                    {ToolTip = "Удалить объект"};
                ButtonPanel.Children.Add(removeButton);
            }
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
}