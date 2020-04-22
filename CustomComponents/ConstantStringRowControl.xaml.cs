using System.Windows.Controls;

namespace TransportGraphApp.CustomComponents {
    public partial class ConstantStringRowControl : UserControl {
        public ConstantStringRowControl() {
            InitializeComponent();
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
    }
}