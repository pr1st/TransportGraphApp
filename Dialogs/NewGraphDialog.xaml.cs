using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LiteDB;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;
using static System.String;
using Attribute = TransportGraphApp.Models.Attribute;

namespace TransportGraphApp.Dialogs {
    public partial class NewGraphDialog : Window {
        public IList<Attribute> GraphAttributes { get; } = new List<Attribute>();
        public IList<Attribute> NodeAttributes { get; } = new List<Attribute>();
        public IList<Attribute> EdgeAttributes { get; } = new List<Attribute>();

        public string GraphName => GraphNameBox.Text;

        public NewGraphDialog() {
            InitializeComponent();
            var tabItems = new List<TabItem> {
                CreateTab("Graph", GraphAttributes),
                CreateTab("Node", NodeAttributes),
                CreateTab("Edge", EdgeAttributes)
            };
            TabControl.ItemsSource = tabItems;
        }

        private static TabItem CreateTab(string title, ICollection<Attribute> consumerList) {
            var tabItem = new TabItem() {Header = title};

            var attributeListView = new ListView {
                Height = 200,
                ItemsSource = consumerList
            };

            var inputFields = CreateInputFields(a => {
                consumerList.Add(a);
                CollectionViewSource.GetDefaultView(attributeListView.ItemsSource).Refresh();
            });
            var deleteButton = new IconButton(AppResources.GetCloseSignIcon, () => {
                if (attributeListView.SelectedItem == null) {
                    return;
                }

                foreach (var selected in (IList<Attribute>) attributeListView.SelectedItems) {
                    consumerList.Remove(selected);
                }

                CollectionViewSource.GetDefaultView(attributeListView.ItemsSource).Refresh();
            });
            inputFields.Children.Add(deleteButton);


            var tabContent = new StackPanel() {Orientation = Orientation.Vertical};
            tabContent.Children.Add(inputFields);
            tabContent.Children.Add(attributeListView);

            tabItem.Content = tabContent;
            return tabItem;
        }


        private static StackPanel CreateInputFields(Action<Attribute> onAdd) {
            var inputFields = new StackPanel() {Orientation = Orientation.Horizontal};

            var nameBox = new StringTextBox();
            inputFields.Children.Add(nameBox);

            var attributes = new ComboBox {
                ItemsSource = Enum.GetValues(typeof(AttributeType)), SelectedItem = AttributeType.Number,
                Margin = new Thickness(5, 5, 5, 5),
                Width = 80
            };
            attributes.SelectionChanged += (sender, args) => {
                UIElement newElement;
                switch ((AttributeType) attributes.SelectedItem) {
                    case AttributeType.Number: {
                        newElement = new DoubleTextBox();
                        break;
                    }
                    case AttributeType.String: {
                        newElement = new StringTextBox();
                        break;
                    }
                    case AttributeType.Boolean: {
                        newElement = new TrueFalseBox();
                        break;
                    }
                    default: {
                        throw new Exception("Illegal state");
                    }
                }

                inputFields.Children.RemoveAt(2);
                inputFields.Children.Insert(2, newElement);
            };
            inputFields.Children.Add(attributes);

            var numBox = new DoubleTextBox();
            inputFields.Children.Add(numBox);

            var addButton = new IconButton(AppResources.GetPlusSignIcon, () => {
                object value;
                var valueProducer = inputFields.Children[2];
                switch ((AttributeType) attributes.SelectedItem) {
                    case AttributeType.Number: {
                        var doubleTextBox = (DoubleTextBox) valueProducer;
                        value = doubleTextBox.Value;
                        doubleTextBox.Value = 0.0;
                        break;
                    }
                    case AttributeType.String: {
                        var stringTextBox = (StringTextBox) valueProducer;
                        value = stringTextBox.Value;
                        stringTextBox.Value = "";
                        break;
                    }
                    case AttributeType.Boolean: {
                        var trueFalseBox = (TrueFalseBox) valueProducer;
                        value = trueFalseBox.Value;
                        trueFalseBox.Value = false;
                        break;
                    }
                    default: {
                        throw new Exception("Illegal state");
                    }
                }

                onAdd.Invoke(new Attribute() {
                        Name = nameBox.Value,
                        Type = (AttributeType) attributes.SelectedItem,
                        Value = value
                    }
                );
                nameBox.Value = "";
            });
            inputFields.Children.Add(addButton);

            return inputFields;
        }

        private void OkClicked(object sender, RoutedEventArgs e) {
            var graphName = GraphNameBox.Text;
            if (graphName == Empty) {
                ComponentUtils.ShowMessage("Enter graph name", MessageBoxImage.Error);
                return;
            }

            var graphCollection = AppDataBase.Instance.GetCollection<Graph>();
            graphCollection.EnsureIndex(g => g.Name);
            var findOne = graphCollection.FindOne(g => g.Name == graphName);
            if (findOne != null) {
                ComponentUtils.ShowMessage("Graph with this name already exists", MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
        }
    }
}