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
using LiteDB;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;
using Attribute = TransportGraphApp.Models.Attribute;

namespace TransportGraphApp.Dialogs {
    public partial class NewNodeDialog : Window {
        public Node CreatedNode { get; private set; }

        private IList<Attribute> Attributes { get; } = new List<Attribute>();

        private Attribute NameAttribute { get; }

        public Attribute XAttribute { get; }
        public Attribute YAttribute { get; }

        private readonly Graph _graph;

        public NewNodeDialog(Graph g) {
            InitializeComponent();
            _graph = g;
            NameAttribute = new Attribute() {
                Name = "Name",
                Type = AttributeType.String,
                Value = ""
            };
            AttributeBox.Children.Add(ComponentUtils.CreateAttributeRow(NameAttribute));
            XAttribute = new Attribute() {
                Name = "X",
                Type = AttributeType.Number,
                Value = 0.0
            };
            AttributeBox.Children.Add(ComponentUtils.CreateAttributeRow(XAttribute));
            YAttribute = new Attribute() {
                Name = "Y",
                Type = AttributeType.Number,
                Value = 0.0
            };
            AttributeBox.Children.Add(ComponentUtils.CreateAttributeRow(YAttribute));
            foreach (var gNodeAttribute in g.DefaultNodeAttributes) {
                var newAttribute = new Attribute() {
                    Name = gNodeAttribute.Name,
                    Type = gNodeAttribute.Type,
                    Value = gNodeAttribute.Value
                };
                Attributes.Add(newAttribute);
                AttributeBox.Children.Add(ComponentUtils.CreateAttributeRow(newAttribute));
            }
        }

        private void OkClicked(object sender, RoutedEventArgs e) {
            if (AppDataBase.Instance.GetCollection<Node>().Exists(n => n.Name == (string) NameAttribute.Value)) {
                ComponentUtils.ShowMessage("Node with this name already exists", MessageBoxImage.Error);
                return;
            }
            CreatedNode = new Node() {
                Name = (string) NameAttribute.Value,
                X = (double) XAttribute.Value,
                Y = (double) YAttribute.Value,
                Attributes = Attributes,
                GraphId = _graph.Id
            };
            DialogResult = true;
        }
    }
}