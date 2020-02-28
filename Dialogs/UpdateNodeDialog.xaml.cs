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
    public partial class UpdateNodeDialog : Window {
        public UpdateNodeDialog(Node node) {
            InitializeComponent();
            AttributePanel.Children.Add(ComponentUtils.CreateAttributeRow(new Attribute() {
                Name = "Name",
                Type = AttributeType.String,
                Value = node.Name
            }));
            AttributePanel.Children.Add(ComponentUtils.CreateAttributeRow(new Attribute() {
                Name = "X",
                Type = AttributeType.Number,
                Value = node.X
            }));
            AttributePanel.Children.Add(ComponentUtils.CreateAttributeRow(new Attribute() {
                Name = "Y",
                Type = AttributeType.Number,
                Value = node.Y
            }));
            foreach (var a in node.Attributes) {
                AttributePanel.Children.Add(ComponentUtils.CreateAttributeRow(a));
            }
        }

        private void OkClicked(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }
    }
}