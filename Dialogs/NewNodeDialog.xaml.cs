using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Models;
using Attribute = TransportGraphApp.Models.Attribute;

namespace TransportGraphApp.Dialogs {
    public partial class NewNodeDialog : Window {
        public Node CreatedNode => new Node() {
            Name = (string) _nameField.Value,
            X = (double) _xField.Value,
            Y = (double) _yField.Value,
            Attributes = _changeBox.UpdatedAttributes,
            GraphId = _graph.Id
        };

        private readonly Attribute _nameField;
        private readonly Attribute _xField;
        private readonly Attribute _yField;
        private readonly Graph _graph;

        private readonly IEnumerable<string> _alreadyUsedNames;

        private readonly AttributesChangeBox _changeBox;

        public NewNodeDialog(Graph g, IEnumerable<string> alreadyUsedNames) {
            _graph = g;
            _alreadyUsedNames = alreadyUsedNames;
            InitializeComponent();
            Icon = AppResources.GetAppIcon;

            _changeBox = new AttributesChangeBox(g.DefaultNodeAttributes);

            _yField = _changeBox.AddField(new Attribute() {
                Name = "Y",
                Type = AttributeType.Number,
                Value = 0.0
            });
            _xField = _changeBox.AddField(new Attribute() {
                Name = "X",
                Type = AttributeType.Number,
                Value = 0.0
            });
            _nameField = _changeBox.AddField(new Attribute() {
                Name = "Name",
                Type = AttributeType.String,
                Value = ""
            });
            AttributePanel.Children.Add(_changeBox);
        }

        private void OkClicked(object sender, RoutedEventArgs e) {
            var name = (string)_nameField.Value;
            if (name == "") {
                ComponentUtils.ShowMessage("Enter node name", MessageBoxImage.Error);
                return;
            }

            if (_alreadyUsedNames.Contains(name)) {
                ComponentUtils.ShowMessage("Node with this name already exists", MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
        }
    }
}