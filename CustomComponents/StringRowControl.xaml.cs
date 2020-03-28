using System;
using System.Windows;
using System.Windows.Controls;

namespace TransportGraphApp.CustomComponents {
    public partial class StringRowControl : UserControl {
        public StringRowControl() {
            InitializeComponent();
            Value = "";
        }
        
        public string Value {
            get => TextBox.Text;
            set => TextBox.Text = value;
        }

        public string ValueTitle {
            get => StringTitle.Text;
            set => StringTitle.Text = value;
        }

        private void ElementGotFocus(object sender, RoutedEventArgs e) {
            TextBox.Select(0, TextBox.Text.Length);
        }
    }
}