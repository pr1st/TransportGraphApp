using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class NewEdgeDialog : Window {
        public Edge CreatedEdge { get; private set; }

        private IList<Attribute> Attributes { get; } = new List<Attribute>();

        private readonly Graph _graph;
        private readonly IList<Node> _nodes;
        private readonly IList<string> _nodeNames;

        public NewEdgeDialog(Graph g) {
            InitializeComponent();
            _graph = g;
            _nodes = AppDataBase.Instance
                .GetCollection<Node>()
                .Find(n => n.GraphId == g.Id)
                .ToList();
            _nodeNames = _nodes.Select(n => n.Name).ToList();
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
            if (!_nodeNames.Contains(FromTextBox.Text) || !_nodeNames.Contains(ToTextBox.Text)) {
                ComponentUtils.ShowMessage("From and To text fields should represent node names", MessageBoxImage.Error);
                return;
            };
            CreatedEdge = new Edge() {
                FromNodeId = _nodes.SkipWhile(n => n.Name != FromTextBox.Text).First().Id,
                ToNodeId = _nodes.SkipWhile(n => n.Name != ToTextBox.Text).First().Id,
                Attributes = Attributes,
                GraphId = _graph.Id
            };
            DialogResult = true;
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e) {
            var border = (Border) ((ScrollViewer) ResultStack.Parent).Parent;

            var target = ((TextBox) sender);
            var query = target.Text;

            border.Visibility = query.Length == 0 ? Visibility.Collapsed : Visibility.Visible;

            ResultStack.Children.Clear();

            foreach (var name in _nodeNames) {
                if (!name.ToLower().StartsWith(query.ToLower())) continue;
                var block = new TextBlock {
                    Text = name,
                    Margin = new Thickness(2, 3, 2, 3),
                    Cursor = Cursors.Hand
                };

                block.MouseLeftButtonUp += (s, args) => { target.Text = ((TextBlock) s).Text; };

                ResultStack.Children.Add(block);
            }
        }
    }
}