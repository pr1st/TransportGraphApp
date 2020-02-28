using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TransportGraphApp.Actions;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Dialogs {
    public partial class NodeListDialog : Window {
        private Node SelectedNode => (Node) ListView.SelectedItem;

        private readonly Graph _graph;

        public NodeListDialog(Graph g) {
            InitializeComponent();
            _graph = g;
            for (var i = 0; i < _graph.DefaultNodeAttributes.Count; i++) {
                var a = _graph.DefaultNodeAttributes[i];
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
            var addButton = ComponentUtils.ButtonWithIcon(AppResources.GetPlusSignIcon);
            addButton.Click += (sender, args) => {
                NewNodeAction.Invoke();
                UpdateStateToInit();
            };
            ModifyListButtons.Children.Add(addButton);

            var updateButton = ComponentUtils.ButtonWithIcon(AppResources.GetUpdateSignIcon);
            updateButton.Click += (sender, args) => {
                if (ListView.SelectedItem == null) {
                    ComponentUtils.ShowMessage("Select node to change attributes", MessageBoxImage.Error);
                    return;
                }

                UpdateNodeAction.Invoke(SelectedNode);
                UpdateStateToInit();
            };
            ModifyListButtons.Children.Add(updateButton);

            var deleteButton = ComponentUtils.ButtonWithIcon(AppResources.GetCloseSignIcon);
            deleteButton.Click += (sender, args) => {
                if (ListView.SelectedItem == null) {
                    ComponentUtils.ShowMessage("Select node to delete it", MessageBoxImage.Error);
                    return;
                }
                foreach (var listViewSelectedItem in ListView.SelectedItems) {
                    DeleteNodeAction.Invoke((Node)listViewSelectedItem);
                }
                UpdateStateToInit();
            };
            ModifyListButtons.Children.Add(deleteButton);
        }

        private void UpdateStateToInit() {
            ListView.ItemsSource = AppDataBase.Instance.GetCollection<Node>().Find(n => n.GraphId == _graph.Id);
            CollectionViewSource.GetDefaultView(ListView.ItemsSource).Refresh();
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