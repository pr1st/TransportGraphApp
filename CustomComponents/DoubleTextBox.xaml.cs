using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TransportGraphApp.CustomComponents {
    public partial class DoubleTextBox : UserControl {
        public DoubleTextBox() {
            InitializeComponent();
            Value = 0.0;
        }

        public const string DescriptionInfo = 
            "Represents input field with double precision\n\n" +
            "Decimal point is a dot (.)\n\n" +
            "You can use keyboard arrows to increment/decrement number\n";

        public double Value {
            get {
                var text = TextBox.Text;
                if (text == "" || text == "-" || text == "." || text == "-.") {
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

            if (result == "-") {
                e.Handled = false;
                return;
            }

            if (received == ",") {
                e.Handled = true;
                return;
            }

            e.Handled = !double.TryParse(result, out _);
        }

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