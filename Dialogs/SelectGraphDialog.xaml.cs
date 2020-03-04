using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TransportGraphApp.Actions;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Models;
using Attribute = TransportGraphApp.Models.Attribute;

namespace TransportGraphApp.Dialogs {
    public partial class SelectGraphDialog : Window {
        private readonly Func<IEnumerable<Graph>> _graphSupplier;

        public Graph SelectedGraph => (Graph) ListView.SelectedItem;

        public SelectGraphDialog(Func<IEnumerable<Graph>> graphSupplier, Graph alreadySelected = null) {
            _graphSupplier = graphSupplier;
            InitializeComponent();
            Icon = AppResources.GetAppIcon;
            TabControl.ItemsSource = new List<TabItem> {
                CreateTab("Graph", new List<Attribute>()),
                CreateTab("Node", new List<Attribute>()),
                CreateTab("Edge", new List<Attribute>())
            };

            ConfigureButtons();
            ListView.SelectionChanged += (sender, args) => {
                if (ListView.SelectedItem == null) {
                    TabControl.ItemsSource = new List<TabItem> {
                        CreateTab("Graph", new List<Attribute>()),
                        CreateTab("Node", new List<Attribute>()),
                        CreateTab("Edge", new List<Attribute>())
                    };
                }
                else {
                    var selected = (Graph)ListView.SelectedItem;
                    TabControl.ItemsSource = new List<TabItem> {
                        CreateTab("Graph", selected.GraphAttributes),
                        CreateTab("Node", selected.DefaultNodeAttributes),
                        CreateTab("Edge", selected.DefaultEdgeAttributes)
                    };
                }
                CollectionViewSource.GetDefaultView(TabControl.ItemsSource).Refresh();
            };
            UpdateStateToInit(alreadySelected);
        }

        private static TabItem CreateTab(string title, IEnumerable<Attribute> attributes) {
            var tabItem = new TabItem() {Header = title};
            var attributeListView = new ListView {
                Height = 200,
                Margin = new Thickness(0, 5, 0, 0),
                ItemsSource = attributes
            };

            var tabContent = new StackPanel() {Orientation = Orientation.Vertical};
            tabContent.Children.Add(attributeListView);
            tabItem.Content = tabContent;
            return tabItem;
        }

        private void ConfigureButtons() {
            var addButton = new IconButton(AppResources.GetAddItemIcon, (() => {
                NewGraphAction.Invoke();
                UpdateStateToInit(null);
            })) {ToolTip = "Add graph"};
            ModifyListButtons.Children.Add(addButton);

            var updateButton = new IconButton(AppResources.GetUpdateItemIcon, (() => {
                var selected = ListView.SelectedItem;
                if (selected == null) {
                    return;
                }

                ChangeGraphAttributesAction.Invoke(SelectedGraph);
                UpdateStateToInit((Graph) selected);
            })) {ToolTip = "Change graph attributes"};
            ModifyListButtons.Children.Add(updateButton);

            var deleteButton = new IconButton(AppResources.GetRemoveItemIcon, () => {
                var selected = ListView.SelectedItem;
                if (selected == null) {
                    return;
                }

                DeleteGraphAction.Invoke(SelectedGraph);
                UpdateStateToInit(null);
            }) {ToolTip = "Remove graph"};
            ModifyListButtons.Children.Add(deleteButton);
        }

        private void UpdateStateToInit(Graph selectedGraph) {
            var graphs = _graphSupplier.Invoke().ToList();
            ListView.ItemsSource = graphs;
            CollectionViewSource.GetDefaultView(ListView.ItemsSource).Refresh();
            ListView.SelectedItem = selectedGraph == null ? null : graphs.SkipWhile(g => g.Id != selectedGraph.Id).First();
        }

        private void OkClicked(object sender, RoutedEventArgs e) {
            if (ListView.SelectedItem == null) {
                ComponentUtils.ShowMessage("Graph was not selected", MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
        }
    }
}