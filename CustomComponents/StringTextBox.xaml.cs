using System.Windows;
using System.Windows.Controls;

namespace TransportGraphApp.CustomComponents {
    public partial class StringTextBox : UserControl {
        public StringTextBox() {
            InitializeComponent();
            Value = "";
        }

        public const string DescriptionInfo =
            "Represents input field for text\n\n" +
            "Enter all the text you want\n";

        public string Value {
            get => TextBox.Text;
            set => TextBox.Text = value;
        }

        private void ElementGotFocus(object sender, RoutedEventArgs e) {
            TextBox.Select(0, TextBox.Text.Length);
        }
    }
}