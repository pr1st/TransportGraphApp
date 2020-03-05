using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TransportGraphApp.Actions;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Models;

namespace TransportGraphApp.Dialogs {
    public partial class NodeListDialog : Window {
        private Node SelectedNode => (Node) ListView.SelectedItem;

        private readonly Func<IEnumerable<Node>> _nodeSupplier;

        public NodeListDialog(Graph graph, Func<IEnumerable<Node>> nodeSupplier) {
            _nodeSupplier = nodeSupplier;
            InitializeComponent();
            Icon = AppResources.GetAppIcon;

            for (var i = 0; i < graph.DefaultNodeAttributes.Count; i++) {
                var a = graph.DefaultNodeAttributes[i];
                var gridViewColumn = new GridViewColumn() {
                    Header = a.Name,
                    DisplayMemberBinding = new Binding() {
                        Converter = new NodeAttributeConverter() {AttributeId = i}
                    }
                };
                ((GridView) ListView.View).Columns.Add(gridViewColumn);
            }

            ConfigureButtons();
            UpdateStateToInit();
        }

        private void ConfigureButtons() {
            var addButton = new IconButton(AppResources.GetAddItemIcon, () => {
                NewNodeAction.Invoke();
                UpdateStateToInit();
            }) {ToolTip = "Add node"};
            ModifyListButtons.Children.Add(addButton);

            var updateButton = new IconButton(AppResources.GetUpdateItemIcon, () => {
                var selected = ListView.SelectedItem;
                if (selected == null) {
                    return;
                }

                UpdateNodeAction.Invoke(SelectedNode);
                UpdateStateToInit(SelectedNode);
            }) {ToolTip = "Change node attributes"};
            ModifyListButtons.Children.Add(updateButton);

            var deleteButton = new IconButton(AppResources.GetRemoveItemIcon, () => {
                var selected = ListView.SelectedItem;
                if (selected == null) {
                    return;
                }

                foreach (var listViewSelectedItem in ListView.SelectedItems) {
                    DeleteNodeAction.Invoke((Node) listViewSelectedItem);
                }

                UpdateStateToInit();
            }) {ToolTip = "Remove node"};
            ModifyListButtons.Children.Add(deleteButton);
        }

        private void UpdateStateToInit(Node selectedNode = null) {
            var nodes = _nodeSupplier.Invoke().ToList();
            ListView.ItemsSource = nodes;
            CollectionViewSource.GetDefaultView(ListView.ItemsSource).Refresh();
            ListView.SelectedItem =
                selectedNode == null ? null : nodes.SkipWhile(n => n.Id != selectedNode.Id).First();
        }
    }

    public class NodeAttributeConverter : IValueConverter {
        public int AttributeId { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Node node) {
                return node.Attributes[AttributeId].Value;
            }

            throw new NotImplementedException("Illegal state");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException("Illegal state");
        }
    }
}