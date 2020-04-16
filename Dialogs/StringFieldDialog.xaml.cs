using System;
using System.Windows;
using System.Windows.Controls;

namespace TransportGraphApp.Dialogs {
    public partial class StringFieldDialog : Window {
        public StringFieldDialog() {
            InitializeComponent();
            Owner = App.Window;
            Icon = AppResources.GetAppIcon;
            FieldValue.IsVisibleChanged += (sender, args) => FieldValue.Focus();
        }

        public Func<string, bool> IsViable { get; set; }
        
        private void OkClick(object sender, RoutedEventArgs e) {
            if (IsViable.Invoke(FieldValue.Text)) {
                DialogResult = true;
            }
        }
    }
}