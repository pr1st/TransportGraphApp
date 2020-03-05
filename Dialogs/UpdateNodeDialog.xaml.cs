using System.Windows;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Models;
using Attribute = TransportGraphApp.Models.Attribute;

namespace TransportGraphApp.Dialogs {
    public partial class UpdateNodeDialog : Window {
        public Node UpdatedNode => new Node() {
            Id = _initNode.Id,
            Name = _initNode.Name,
            X = (double) _xField.Value,
            Y = (double) _yField.Value,
            Attributes = _changeBox.UpdatedAttributes,
            GraphId = _initNode.GraphId
        };

        private readonly Node _initNode;
        private readonly AttributesChangeBox _changeBox;
        private readonly Attribute _xField;
        private readonly Attribute _yField;

        public UpdateNodeDialog(Node node) {
            _initNode = node;
            InitializeComponent();

            NodeNameLabel.Content = _initNode.Name;
            _changeBox = new AttributesChangeBox(node.Attributes);
            _yField = _changeBox.AddField(new Attribute() {
                Name = "Y",
                Type = AttributeType.Number,
                Value = _initNode.Y
            });
            _xField = _changeBox.AddField(new Attribute() {
                Name = "X",
                Type = AttributeType.Number,
                Value = _initNode.X
            });
            AttributePanel.Children.Add(_changeBox);
        }

        private void OkClicked(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }
    }
}