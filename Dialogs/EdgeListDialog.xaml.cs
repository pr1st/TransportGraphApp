using System;
using System.Collections.Generic;
using System.Globalization;
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
using TransportGraphApp.Actions;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Dialogs {
    public partial class EdgeListDialog : Window {
        private Edge SelectedEdge => (Edge) ListView.SelectedItem;

        private readonly Graph _graph;
        private readonly IEnumerable<Node> nodes;

        public EdgeListDialog(Graph g) {
            InitializeComponent();
            _graph = g;
            nodes = AppDataBase.Instance.GetCollection<Node>().Find(n => n.GraphId == _graph.Id);

            var enumerable = nodes as Node[] ?? nodes.ToArray();
            ((GridView) ListView.View).Columns.Add(new GridViewColumn() {
                Header = "From",
                DisplayMemberBinding = new Binding() {
                    Converter = new EdgeFromConverter() {Nodes = enumerable}
                }
            });
            ((GridView) ListView.View).Columns.Add(new GridViewColumn() {
                Header = "To",
                DisplayMemberBinding = new Binding() {
                    Converter = new EdgeToConverter() {Nodes = enumerable}
                }
            });

            for (var i = 0; i < _graph.DefaultEdgeAttributes.Count; i++) {
                var a = _graph.DefaultEdgeAttributes[i];
                var gridViewColumn = new GridViewColumn() {
                    Header = a.Name,
                    DisplayMemberBinding = new Binding() {
                        Converter = new EdgeAttributeConverter() {AttributeId = i}
                    }
                };
                ((GridView) ListView.View).Columns.Add(gridViewColumn);
            }
        }

        private void ConfigureButtons() {
            var addButton = ComponentUtils.ButtonWithIcon(AppResources.GetPlusSignIcon);
            addButton.Click += (sender, args) => {
                NewEdgeAction.Invoke();
                UpdateStateToInit();
            };
            ModifyListButtons.Children.Add(addButton);

            var updateButton = ComponentUtils.ButtonWithIcon(AppResources.GetUpdateSignIcon);
            updateButton.Click += (sender, args) => {
                if (ListView.SelectedItem == null) {
                    ComponentUtils.ShowMessage("Select edge to change attributes", MessageBoxImage.Error);
                    return;
                }

                UpdateEdgeAction.Invoke(SelectedEdge);
                UpdateStateToInit();
            };
            ModifyListButtons.Children.Add(updateButton);

            var deleteButton = ComponentUtils.ButtonWithIcon(AppResources.GetCloseSignIcon);
            deleteButton.Click += (sender, args) => {
                if (ListView.SelectedItem == null) {
                    ComponentUtils.ShowMessage("Select edge to delete it", MessageBoxImage.Error);
                    return;
                }

                foreach (var listViewSelectedItem in ListView.SelectedItems) {
                    DeleteEdgeAction.Invoke((Edge) listViewSelectedItem);
                }

                UpdateStateToInit();
            };
            ModifyListButtons.Children.Add(deleteButton);
        }

        private void UpdateStateToInit() {
            var edges = AppDataBase.Instance.GetCollection<Edge>().Find(e => e.GraphId == _graph.Id);
            ListView.ItemsSource = edges;
            CollectionViewSource.GetDefaultView(ListView.ItemsSource).Refresh();
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
        public IEnumerable<Node> Nodes { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Edge edge) {
                return Nodes.SkipWhile(n => n.Id != edge.FromNodeId).First().Name;
            }

            throw new NotImplementedException("Illegal state");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException("Illegal state");
        }
    }

    public class EdgeToConverter : IValueConverter {
        public IEnumerable<Node> Nodes { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Edge edge) {
                return Nodes.SkipWhile(n => n.Id != edge.ToNodeId).First().Name;
            }

            throw new NotImplementedException("Illegal state");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException("Illegal state");
        }
    }
}