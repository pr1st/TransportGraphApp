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
using Attribute = TransportGraphApp.Models.Attribute;

namespace TransportGraphApp.Dialogs {
    public partial class ChangeGraphAttributesDialog : Window {
        public ChangeGraphAttributesDialog(Graph graph) {
            InitializeComponent();
            GraphNameLabel.Content = graph.Name;
            foreach (var graphGraphAttribute in graph.GraphAttributes) {
                AttributePanel.Children.Add(ComponentUtils.CreateAttributeRow(graphGraphAttribute));
            }
        }

        private void OkClicked(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }
    }
}