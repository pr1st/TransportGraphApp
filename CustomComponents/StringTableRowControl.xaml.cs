using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TransportGraphApp.CustomComponents {
    public partial class StringTableRowControl : UserControl {
        public StringTableRowControl() {
            InitializeComponent();
            ComponentUtils.InsertIconToButton(AddButton, AppResources.GetAddItemIcon, "Добавить новый тип дороги");
            ComponentUtils.InsertIconToButton(RemoveButton, AppResources.GetRemoveItemIcon, "Удалить выделенные типы");
            _values = new List<string>();
            StringList.ItemsSource = _values;
            TitleValue = "Unnamed";
            AddBox.Text = "";
            IsViable = (s, l) => true;
        }
        
        private IList<string> _values;
        
        public string TitleValue {
            get => StringTitle.Text;
            set => StringTitle.Text = value;
        }
        
        public IList<string> Value {
            get => _values;
            set {
                _values = value;
                StringList.ItemsSource = _values;
                CollectionViewSource.GetDefaultView(StringList.ItemsSource).Refresh();
            }
        }
        
        public Func<string, IList<string>, bool> IsViable { get; set; }

        private void AddElement(object sender, RoutedEventArgs e) {
            if (!IsViable.Invoke(AddBox.Text, _values)) return;
            
            _values.Add(AddBox.Text);
            CollectionViewSource.GetDefaultView(StringList.ItemsSource).Refresh();
            AddBox.Text = "";
        }

        private void RemoveElements(object sender, RoutedEventArgs e) {
            if (StringList.SelectedItem == null) return;
            
            foreach (var selectedItem in StringList.SelectedItems) {
                _values.Remove((string) selectedItem);
            }
            CollectionViewSource.GetDefaultView(StringList.ItemsSource).Refresh();
        }
    }
}