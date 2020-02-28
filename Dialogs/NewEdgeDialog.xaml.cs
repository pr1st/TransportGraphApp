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
    public partial class NewEdgeDialog : Window {
        public Edge CreatedEdge { get; private set; }

        private IList<Attribute> Attributes { get; } = new List<Attribute>();

        private readonly Graph _graph;

        public NewEdgeDialog(Graph g) {
            InitializeComponent();
            _graph = g;
            // from
            // to
            foreach (var a in g.DefaultEdgeAttributes) {
                var newAttribute = new Attribute() {
                    Name = a.Name,
                    Type = a.Type,
                    Value = a.Value
                };
                Attributes.Add(newAttribute);
                AttributeBox.Children.Add(ComponentUtils.CreateAttributeRow(newAttribute));
            }
        }

        private void OkClicked(object sender, RoutedEventArgs e) {
            CreatedEdge = new Edge() {
                Attributes = Attributes,
                GraphId = _graph.Id
            };
            DialogResult = true;
        }
    }
}