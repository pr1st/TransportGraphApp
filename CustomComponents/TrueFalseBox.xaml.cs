using System;
using System.Windows.Controls;

namespace TransportGraphApp.CustomComponents {
    public partial class TrueFalseBox : UserControl {
        public TrueFalseBox() {
            InitializeComponent();
            Value = false;
        }

        public void ValueChanged(Action<bool> onChange) {
            CheckBox.Checked += (sender, args) => onChange.Invoke(Value);
            CheckBox.Unchecked += (sender, args) => onChange.Invoke(Value);
        }

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