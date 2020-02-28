using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TransportGraphApp.Models;

namespace TransportGraphApp.Dialogs {
    public partial class UpdateEdgeDialog : Window {
        public UpdateEdgeDialog(Edge edge) {
            InitializeComponent();
            // from
            // to
            foreach (var a in edge.Attributes) {
                AttributePanel.Children.Add(ComponentUtils.CreateAttributeRow(a));
            }
        }

        private void OkClicked(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }
    }
}