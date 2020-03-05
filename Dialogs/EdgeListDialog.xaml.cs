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
    public partial class EdgeListDialog : Window {
        private Edge SelectedEdge => (Edge) ListView.SelectedItem;

        private readonly Func<IEnumerable<Edge>> _edgeSupplier;

        public EdgeListDialog(Graph graph, Func<IEnumerable<Edge>> edgeSupplier,
            IDictionary<int, string> nodeNamesDictionary) {
            _edgeSupplier = edgeSupplier;
            InitializeComponent();
            Icon = AppResources.GetAppIcon;


            ((GridView) ListView.View).Columns.Add(new GridViewColumn() {
                Header = "From",
                DisplayMemberBinding = new Binding() {
                    Converter = new EdgeFromConverter() {Nodes = nodeNamesDictionary }
                }
            });
            ((GridView) ListView.View).Columns.Add(new GridViewColumn() {
                Header = "To",
                DisplayMemberBinding = new Binding() {
                    Converter = new EdgeToConverter() {Nodes = nodeNamesDictionary }
                }
            });

            for (var i = 0; i < graph.DefaultEdgeAttributes.Count; i++) {
                var a = graph.DefaultEdgeAttributes[i];
                var gridViewColumn = new GridViewColumn() {
                    Header = a.Name,
                    DisplayMemberBinding = new Binding() {
                        Converter = new EdgeAttributeConverter() {AttributeId = i}
                    }
                };
                ((GridView) ListView.View).Columns.Add(gridViewColumn);
            }

            ConfigureButtons();
            UpdateStateToInit();
        }

        private void ConfigureButtons() {
            var addButton = new IconButton(AppResources.GetAddItemIcon, () => {
                    NewEdgeAction.Invoke();
                    UpdateStateToInit();
                })
                {ToolTip = "Add edge"};
            ModifyListButtons.Children.Add(addButton);

            var updateButton = new IconButton(AppResources.GetUpdateItemIcon, () => {
                    var selected = ListView.SelectedItem;
                    if (selected == null) {
                        return;
                    }

                    UpdateEdgeAction.Invoke(SelectedEdge);
                    UpdateStateToInit(SelectedEdge);
                })
                {ToolTip = "Change edge attributes"};
            ModifyListButtons.Children.Add(updateButton);

            var deleteButton = new IconButton(AppResources.GetRemoveItemIcon, () => {
                    var selected = ListView.SelectedItem;
                    if (selected == null) {
                        return;
                    }

                    foreach (var listViewSelectedItem in ListView.SelectedItems) {
                        DeleteEdgeAction.Invoke((Edge) listViewSelectedItem);
                    }

                    UpdateStateToInit();
                })
                {ToolTip = "Remove edge"};
            ModifyListButtons.Children.Add(deleteButton);
        }

        private void UpdateStateToInit(Edge selectedEdge = null) {
            var edges = _edgeSupplier.Invoke().ToList();
            ListView.ItemsSource = edges;
            CollectionViewSource.GetDefaultView(ListView.ItemsSource).Refresh();
            ListView.SelectedItem =
                selectedEdge == null ? null : edges.SkipWhile(e => e.Id != selectedEdge.Id).First();
        }
    }


    public class EdgeAttributeConverter : IValueConverter {
        public int AttributeId { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Edge edge) {
                return edge.Attributes[AttributeId].Value;
            }

            throw new NotImplementedException("Illegal state");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException("Illegal state");
        }
    }

    public class EdgeFromConverter : IValueConverter {
        public IDictionary<int, string> Nodes { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Edge edge) {
                return Nodes.SkipWhile(n => n.Key != edge.FromNodeId).First().Value;
            }

            throw new NotImplementedException("Illegal state");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException("Illegal state");
        }
    }

    public class EdgeToConverter : IValueConverter {
        public IDictionary<int, string> Nodes { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Edge edge) {
                return Nodes.SkipWhile(n => n.Key != edge.ToNodeId).First().Value;
            }

            throw new NotImplementedException("Illegal state");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException("Illegal state");
        }
    }
}