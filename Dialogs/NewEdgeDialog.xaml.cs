using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Models;

namespace TransportGraphApp.Dialogs {
    public partial class NewEdgeDialog : Window {
        public Edge CreatedEdge => new Edge() {
            FromNodeId = _nodeNames[FromTextBox.Text],
            ToNodeId = _nodeNames[ToTextBox.Text],
            Attributes = _changeBox.UpdatedAttributes,
            GraphId = _graph.Id
        };

        private readonly Graph _graph;
        private readonly IDictionary<string, int> _nodeNames;
        private readonly AttributesChangeBox _changeBox;

        private TextBox _currentTextFieldSender;

        public NewEdgeDialog(Graph g, IDictionary<string, int> nodeNames) {
            _graph = g;
            _nodeNames = nodeNames;
            InitializeComponent();
            Icon = AppResources.GetAppIcon;

            NodeNamesList.ItemsSource = _nodeNames.Keys;
            var view = (CollectionView) CollectionViewSource.GetDefaultView(NodeNamesList.ItemsSource);
            view.Filter = item =>
                _currentTextFieldSender == null ||
                ((string) item).ToLower().Contains(_currentTextFieldSender.Text.ToLower());

            _changeBox = new AttributesChangeBox(g.DefaultEdgeAttributes);
            AttributePanel.Children.Add(_changeBox);
        }

        private void OkClicked(object sender, RoutedEventArgs e) {
            if (!_nodeNames.ContainsKey(FromTextBox.Text) || !_nodeNames.ContainsKey(ToTextBox.Text)) {
                ComponentUtils.ShowMessage("From and To text fields should represent node names",
                    MessageBoxImage.Error);
                return;
            }

            ;
            DialogResult = true;
        }

        private void UpdateNodeNamesResults(object sender, KeyEventArgs e) {
            _currentTextFieldSender = (TextBox) sender;
            CollectionViewSource.GetDefaultView(NodeNamesList.ItemsSource).Refresh();
        }

        private void EnterNodeNameResult(object sender, KeyEventArgs e) {
            if (e.Key != Key.Enter) return;
            _currentTextFieldSender.Text = (string) NodeNamesList.SelectedItem;
            e.Handled = true;
        }

        private void UpOrDownPressed(object sender, KeyEventArgs e) {
            if (e.Key == Key.Down || e.Key == Key.Up) {
                NodeNamesList.Focus();
            }
        }
    }
}