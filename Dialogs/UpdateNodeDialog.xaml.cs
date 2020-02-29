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
using TransportGraphApp.Singletons;
using Attribute = TransportGraphApp.Models.Attribute;

namespace TransportGraphApp.Dialogs {
    public partial class UpdateNodeDialog : Window {
        private Node _node;

        public UpdateNodeDialog(Node node) {
            InitializeComponent();
            _node = node;
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
            if (AppDataBase.Instance.GetCollection<Node>().Exists(n => n.Name == _node.Name)) {
                ComponentUtils.ShowMessage("Node with this name already exists", MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
        }
    }
}