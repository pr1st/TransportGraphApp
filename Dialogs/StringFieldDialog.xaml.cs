using System;
using System.Windows;
using TransportGraphApp.CustomComponents;

namespace TransportGraphApp.Dialogs {
    public partial class StringFieldDialog : Window {
        public StringFieldDialog() {
            InitializeComponent();
            Owner = App.Window;
            Icon = AppResources.GetAppIcon;
            RowControl = new StringWithHelpRowControl();
            RowControl.IsVisibleChanged += (sender, args) => RowControl.Focus();
            DataPanel.Children.Add(RowControl);
        }

        public StringWithHelpRowControl RowControl { get; }

        public Func<string, bool> IsViable { get; set; }
        
        private void OkClick(object sender, RoutedEventArgs e) {
            if (IsViable.Invoke(RowControl.Value)) {
                DialogResult = true;
            }
        }
    }
}