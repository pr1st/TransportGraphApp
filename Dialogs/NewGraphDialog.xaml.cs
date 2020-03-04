using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Models;
using static System.String;
using Attribute = TransportGraphApp.Models.Attribute;

namespace TransportGraphApp.Dialogs {
    public partial class NewGraphDialog : Window {
        private readonly IList<Attribute> _graphAttributes = new List<Attribute>();

        private readonly IList<Attribute> _nodeAttributes = new List<Attribute>();

        private readonly IList<Attribute> _edgeAttributes = new List<Attribute>();

        private readonly IEnumerable<string> _alreadyUsedGraphNames;

        public Graph Graph => new Graph() {
            Name = GraphNameBox.Value,
            GraphAttributes = _graphAttributes,
            DefaultNodeAttributes = _nodeAttributes,
            DefaultEdgeAttributes = _edgeAttributes
        };

        public NewGraphDialog(IEnumerable<string> alreadyUsedGraphNames) {
            _alreadyUsedGraphNames = alreadyUsedGraphNames;
            InitializeComponent();
            Icon = AppResources.GetAppIcon;

            var tabItems = new List<TabItem> {
                CreateTab("Graph", _graphAttributes),
                CreateTab("Node", _nodeAttributes),
                CreateTab("Edge", _edgeAttributes)
            };
            TabControl.ItemsSource = tabItems;
        }

        private static TabItem CreateTab(string title, ICollection<Attribute> consumerList) {
            var tabItem = new TabItem() {Header = title};

            var attributeListView = new ListView {
                Height = 200,
                ItemsSource = consumerList
            };

            var labels = new StackPanel() {
                Orientation = Orientation.Horizontal
            };
            labels.Children.Add(new Label() {
                Content = "Name",
                Width = 130
            });
            labels.Children.Add(new Label() {
                Content = "Type",
                Width = 90
            });
            labels.Children.Add(new Label() {
                Content = "Default value",
                Width = 120
            });

            var inputFields = CreateInputFields(a => {
                consumerList.Add(a);
                CollectionViewSource.GetDefaultView(attributeListView.ItemsSource).Refresh();
            });
            var deleteButton = new IconButton(AppResources.GetRemoveItemIcon, () => {
                if (attributeListView.SelectedItem == null) {
                    return;
                }

                foreach (var selected in (IList<Attribute>) attributeListView.SelectedItems) {
                    consumerList.Remove(selected);
                }

                CollectionViewSource.GetDefaultView(attributeListView.ItemsSource).Refresh();
            }) {ToolTip = "Delete attribute"};
            inputFields.Children.Add(deleteButton);


            var tabContent = new StackPanel() {Orientation = Orientation.Vertical};
            tabContent.Children.Add(labels);
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

            var addButton = new IconButton(AppResources.GetAddItemIcon, () => {
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
            }) { ToolTip = "Add attribute" };
            inputFields.Children.Add(addButton);

            return inputFields;
        }

        private void OkClicked(object sender, RoutedEventArgs e) {
            var graphName = GraphNameBox.Value;
            if (graphName == Empty) {
                ComponentUtils.ShowMessage("Enter graph name", MessageBoxImage.Error);
                return;
            }

            if (_alreadyUsedGraphNames.Contains(graphName)) {
                ComponentUtils.ShowMessage("Graph with this name already exists", MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
        }
    }
}