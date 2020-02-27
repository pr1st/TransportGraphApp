using System;
using System.Collections.Generic;
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
using Attribute = TransportGraphApp.Models.Attribute;

namespace TransportGraphApp.Dialogs {
    public partial class SelectGraphDialog : Window {
        public Graph SelectedGraph => (Graph) ListView.SelectedItem;

        public SelectGraphDialog() {
            InitializeComponent();
            ConfigureButtons();
            UpdateStateToInit();
            ListView.SelectionChanged += (sender, args) => {
                if (ListView.SelectedItem == null)
                    return;
                var selected = (Graph)ListView.SelectedItem;
                TabControl.ItemsSource = new List<TabItem> {
                    CreateTab("Graph", selected.GraphAttributes),
                    CreateTab("Node", selected.DefaultNodeAttributes),
                    CreateTab("Edge", selected.DefaultEdgeAttributes)
                };
                CollectionViewSource.GetDefaultView(TabControl.ItemsSource).Refresh();
            };
            if (AppGraph.Instance.Graph != null) {
                ListView.SelectedItem = AppGraph.Instance.Graph;
            }
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
            var addButton = ComponentUtils.ButtonWithIcon(AppResources.GetPlusSignIcon);
            addButton.Click += (sender, args) => {
                NewGraphAction.Invoke();
                UpdateStateToInit();
            };
            ModifyListButtons.Children.Add(addButton);

            var updateButton = ComponentUtils.ButtonWithIcon(AppResources.GetUpdateSignIcon);
            updateButton.Click += (sender, args) => {
                if (ListView.SelectedItem == null) {
                    ComponentUtils.ShowMessage("Select graph to change attributes", MessageBoxImage.Error);
                    return;
                }

                ChangeGraphAttributesAction.Invoke(SelectedGraph);
                UpdateStateToInit();
            };
            ModifyListButtons.Children.Add(updateButton);

            var deleteButton = ComponentUtils.ButtonWithIcon(AppResources.GetCloseSignIcon);
            deleteButton.Click += (sender, args) => {
                if (ListView.SelectedItem == null) {
                    ComponentUtils.ShowMessage("Select graph to delete it", MessageBoxImage.Error);
                    return;
                }

                DeleteGraphAction.Invoke(SelectedGraph);
                UpdateStateToInit();
            };
            ModifyListButtons.Children.Add(deleteButton);
        }

        private void UpdateStateToInit() {
            ListView.ItemsSource = AppDataBase.Instance.GetCollection<Graph>().FindAll();
            TabControl.ItemsSource = new List<TabItem> {
                CreateTab("Graph", new List<Attribute>()),
                CreateTab("Node", new List<Attribute>()),
                CreateTab("Edge", new List<Attribute>())
            };
            CollectionViewSource.GetDefaultView(ListView.ItemsSource).Refresh();
            CollectionViewSource.GetDefaultView(TabControl.ItemsSource).Refresh();
        }

        private void OkClicked(object sender, RoutedEventArgs e) {
            if (ListView.SelectedItem == null) {
                ComponentUtils.ShowMessage("Select graph", MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
        }
    }
}