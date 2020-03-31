using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TransportGraphApp.CustomComponents {
    public partial class LatitudeLongitudeRowControl : UserControl {
        public LatitudeLongitudeRowControl() {
            InitializeComponent();
            Latitude = 0.0;
            Longitude = 0.0;
        }

        private static double GetDouble(string text) {
            if (text == "" || text == "-" || text == "-." || text == ".") {
                return 0.0;
            }

            return double.Parse(text);
        }

        public double Latitude {
            get => GetDouble(LatitudeBox.Text);
            set => LatitudeBox.Text = value.ToString(CultureInfo.InvariantCulture);
        }

        public double Longitude {
            get => GetDouble(LongitudeBox.Text);
            set => LongitudeBox.Text = value.ToString(CultureInfo.InvariantCulture);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            var senderTextBox = (TextBox) sender;
            var initial = senderTextBox.Text;
            var received = e.Text;
            var caretIndex = senderTextBox.CaretIndex;

            if (received == " " || received == ",") {
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

            if (result == "-") {
                e.Handled = false;
                return;
            }

            var parsed = double.TryParse(result, out var res);
            if (!parsed || res > 180.0 || res < -180.0) {
                e.Handled = true;
            }
        }

        private void ElementGotFocus(object sender, RoutedEventArgs e) {
            var senderTextBox = (TextBox) sender;
            senderTextBox.Select(0, senderTextBox.Text.Length);
        }

        private void NumberValidation(object sender, TextChangedEventArgs e) {
            foreach (var change in e.Changes) {
                if (change.RemovedLength > 0) {
                    Latitude = Math.Max(-180.0, Latitude);
                    Latitude = Math.Min(180.0, Latitude);
                    Longitude = Math.Max(-180.0, Longitude);
                    Longitude = Math.Min(180.0, Longitude);
                }
            }
        }
    }
}