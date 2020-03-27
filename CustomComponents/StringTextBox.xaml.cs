using System;
using System.Windows;
using System.Windows.Controls;

namespace TransportGraphApp.CustomComponents {
    public partial class StringTextBox : UserControl {
        public StringTextBox() {
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

        public void ValueChanged(Action<string> onChange) =>
            TextBox.TextChanged += (sender, args) => onChange.Invoke(Value);

        private void ElementGotFocus(object sender, RoutedEventArgs e) {
            TextBox.Select(0, TextBox.Text.Length);
        }
    }
}