using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TransportGraphApp.CustomComponents {
    public partial class PositiveDoubleRowControl : UserControl {
        public PositiveDoubleRowControl() {
            InitializeComponent();
            Value = 0.0;
            TitleValue = "Unnamed";
        }
        
        public string TitleValue {
            get => StringTitle.Text;
            set => StringTitle.Text = value;
        }
        
        public string TitleToolTip {
            get => (string)StringTitle.ToolTip;
            set => StringTitle.ToolTip = value;
        }
        
        public double Value {
            get {
                var text = ValueBox.Text;
                if (text == "" || text == ".") {
                    return 0.0;
                }
                return double.Parse(text);
            } 
            set => ValueBox.Text = value.ToString(CultureInfo.InvariantCulture);
        }
        

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            var initial = ValueBox.Text;
            var received = e.Text;
            var caretIndex = ValueBox.CaretIndex;
            
            if (received == " " || received == "," || received == "-") {
                e.Handled = true;
                return;
            }
            
            string result;
            if (initial.Length > 0) {
                result = initial.Substring(0, caretIndex) + received +
                         initial[caretIndex..];
            }
            else {
                result = received;
            }

            var parsed = double.TryParse(result, out _);
            if (!parsed) {
                e.Handled = true;
            }
        }

        private void ElementGotFocus(object sender, RoutedEventArgs e) {
            ValueBox.Select(0, ValueBox.Text.Length);
        }
    }
}