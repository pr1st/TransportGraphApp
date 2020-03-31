using System;
using System.Collections;
using System.Collections.Generic;
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

        private void ValueChanged(object sender, TextChangedEventArgs e) {
            Popup.IsOpen = true;
            CollectionViewSource.GetDefaultView(HelpList.ItemsSource).Refresh();
        }

        private bool ItemFilter(object item) {
            return ((string)item).ToLower().StartsWith(Value.ToLower());
            
        }

        private void ValueBoxPressedDown(object sender, KeyEventArgs e) {
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
            }
        }
    }
}