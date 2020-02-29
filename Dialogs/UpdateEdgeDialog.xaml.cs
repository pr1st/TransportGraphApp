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

namespace TransportGraphApp.Dialogs {
    public partial class UpdateEdgeDialog : Window {
        private readonly IList<Node> _nodes;
        private readonly IList<string> _nodeNames;
        private Edge _edge;

        public UpdateEdgeDialog(Edge edge) {
            InitializeComponent();
            _nodes = AppDataBase.Instance
                .GetCollection<Node>()
                .Find(n => n.GraphId == edge.GraphId)
                .ToList();
            _nodeNames = _nodes.Select(n => n.Name).ToList();
            _edge = edge;
            FromTextBox.Text = _nodes.SkipWhile(n => n.Id != edge.FromNodeId).First().Name;
            ToTextBox.Text = _nodes.SkipWhile(n => n.Id != edge.ToNodeId).First().Name;
            foreach (var a in edge.Attributes) {
                AttributeBox.Children.Add(ComponentUtils.CreateAttributeRow(a));
            }
        }

        private void OkClicked(object sender, RoutedEventArgs e) {
            if (!_nodeNames.Contains(FromTextBox.Text) || !_nodeNames.Contains(ToTextBox.Text)) {
                ComponentUtils.ShowMessage("From and To text fields should represent node names",
                    MessageBoxImage.Error);
                return;
            }

            _edge.FromNodeId = _nodes.SkipWhile(n => n.Name != FromTextBox.Text).First().Id;
            _edge.ToNodeId = _nodes.SkipWhile(n => n.Name != ToTextBox.Text).First().Id;
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