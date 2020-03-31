using System.Windows.Controls;

namespace TransportGraphApp.CustomComponents {
    public partial class TrueFalseRowControl : UserControl {
        public TrueFalseRowControl() {
            InitializeComponent();
            Value = false;
            TitleValue = "Unnamed";
        }
        
        public string TitleValue {
            get => StringTitle.Text;
            set => StringTitle.Text = value;
        }
        
        public bool Value {
            get {
                if (ValueBox.IsChecked == null)
                    return false;
                return (bool) ValueBox.IsChecked;
            }
            set => ValueBox.IsChecked = value;
        }
    }
}