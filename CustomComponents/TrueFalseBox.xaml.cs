using System.Windows.Controls;

namespace TransportGraphApp.CustomComponents {
    public partial class TrueFalseBox : UserControl {
        public TrueFalseBox() {
            InitializeComponent();
            Value = false;
        }

        public const string DescriptionInfo =
            "Represents true false field\n\n" +
            "If checkbox is checked it means true, otherwise false\n\n" +
            "Use space to check/uncheck with keyboard\n";

        public bool Value {
            get {
                if (CheckBox.IsChecked == null)
                    return false;
                return (bool) CheckBox.IsChecked;
            }
            set => CheckBox.IsChecked = value;
        }
    }
}