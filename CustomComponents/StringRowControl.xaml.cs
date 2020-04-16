using System;
using System.Windows;
using System.Windows.Controls;

namespace TransportGraphApp.CustomComponents {
    public partial class StringRowControl : UserControl {
        public StringRowControl() {
            InitializeComponent();
            Value = "";
            TitleValue = "Unnamed";
        }
        
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

        private void ElementGotFocus(object sender, RoutedEventArgs e) {
            ValueBox.Select(0, ValueBox.Text.Length);
        }
    }
}