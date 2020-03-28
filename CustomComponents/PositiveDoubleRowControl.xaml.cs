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
        }
        
        public double Value {
            get {
                var text = TextBox.Text;
                if (text == "" || text == ".") {
                    return 0.0;
                }
                return double.Parse(text);
            } 
            set => TextBox.Text = value.ToString(CultureInfo.InvariantCulture);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            var initial = TextBox.Text;
            var received = e.Text;
            var caretIndex = TextBox.CaretIndex;
            string result;
            if (initial.Length > 0) {
                result = initial.Substring(0, caretIndex) + received +
                         initial[caretIndex..];
            }
            else {
                result = received;
            }

            var parsed = double.TryParse(result, out var res);
            if (!parsed || res < 0) {
                e.Handled = true;
            }
        }

        public void ValueChanged(Action<double> onChange) =>
            TextBox.TextChanged += (sender, args) => onChange.Invoke(Value);

        private void ElementGotFocus(object sender, RoutedEventArgs e) {
            TextBox.Select(0, TextBox.Text.Length);
        }

        private void ArrowUp(object sender, RoutedEventArgs e) {
            Value += 1.0;
        }

        private void ArrowDown(object sender, RoutedEventArgs e) {
            Value -= 1.0;
        }

        private void UpOrDownPressed(object sender, KeyEventArgs e) {
            if (e.Key == Key.Up)
                Value += 1.0;
            else if (e.Key == Key.Down)
                Value -= 1.0;
        }
    }
}