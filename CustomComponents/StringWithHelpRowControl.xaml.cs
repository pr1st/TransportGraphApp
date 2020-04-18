using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace TransportGraphApp.CustomComponents {
    public partial class StringWithHelpRowControl : UserControl {
        public StringWithHelpRowControl() {
            InitializeComponent();
            Value = "";
            TitleValue = "Unnamed";
            _helpingValues = new List<string>();

            Popup.PlacementTarget = ValueBox;
            HelpList.ItemsSource = HelpingValues;
            var view = (CollectionView) CollectionViewSource.GetDefaultView(HelpList.ItemsSource);
            view.Filter = ItemFilter;
        }

        private IList<string> _helpingValues;

        public string Value {
            get => ValueBox.Text;
            set => ValueBox.Text = value;
        }

        public string TitleValue {
            get => StringTitle.Text;
            set => StringTitle.Text = value;
        }
        
        public string TitleToolTip {
            get => (string)StringTitle.ToolTip;
            set => StringTitle.ToolTip = value;
        }
        
        public ThreadStart OnEnterPressed { get; set; }

        public new bool Focus() {
            return ValueBox.Focus();
        }

        public IList<string> HelpingValues {
            get => _helpingValues;
            set {
                _helpingValues = value;
                HelpList.ItemsSource = _helpingValues;
                var view = (CollectionView) CollectionViewSource.GetDefaultView(HelpList.ItemsSource);
                view.Filter = ItemFilter;
                CollectionViewSource.GetDefaultView(HelpList.ItemsSource).Refresh();
            }
        }

        private void ElementGotFocus(object sender, RoutedEventArgs e) {
            ValueBox.Select(0, ValueBox.Text.Length);
        }

        private void ElementLostFocus(object sender, RoutedEventArgs e) {
            // Console.WriteLine(Popup.IsFocused);
            // Console.WriteLine(HelpList.IsFocused);
            // var it = HelpList.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
            // Console.WriteLine(it?.IsFocused);
            // Popup.IsOpen = false;
            //
            // HelpList.SelectedItem = null;
            // OnEnterPressed?.Invoke();
        }

        private void ValueChanged(object sender, TextChangedEventArgs e) {
            if (!ValueBox.IsFocused) return;
            if (_helpingValues == null || !_helpingValues.Any()) return;
            
            Popup.IsOpen = true;
            CollectionViewSource.GetDefaultView(HelpList.ItemsSource).Refresh();
        }

        private bool ItemFilter(object item) {
            return ((string)item).ToLower().Contains(Value.ToLower());
        }

        private void ValueBoxPressedDownOrTab(object sender, KeyEventArgs e) {
            if (e.Key == Key.Tab) {
                HelpList.SelectedItem = null;
                Popup.IsOpen = false;
                OnEnterPressed?.Invoke();
                return;
            }

            if (e.Key != Key.Down) return;
            
            if (HelpList.Items.Count == 1) {
                HelpList.SelectedIndex = 0;
                ValueBox.Text = (string) HelpList.SelectedItem;
                HelpList.SelectedItem = null;
                ValueBox.Focus();
                Popup.IsOpen = false;
                return;
            }
            
            var item = HelpList.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
            item?.Focus();
        }

        private void HelpListPressedEnter(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Enter when HelpList.SelectedItem != null:
                    ValueBox.Text = (string) HelpList.SelectedItem;
                    HelpList.SelectedItem = null;
                    ValueBox.Focus();
                    Popup.IsOpen = false;
                    OnEnterPressed?.Invoke();
                    break;
                case Key.Escape:
                    HelpList.SelectedItem = null;
                    ValueBox.Focus();
                    Popup.IsOpen = false;
                    break;
            }
        }

        private void HelpListMousePressed(object sender, MouseButtonEventArgs mouseButtonEventArgs) {
            if (HelpList.SelectedItem != null) {
                ValueBox.Text = (string) HelpList.SelectedItem;
                HelpList.SelectedItem = null;
                ValueBox.Focus();
                Popup.IsOpen = false;

                OnEnterPressed?.Invoke();
            }
        }
    }
}