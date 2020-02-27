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
                Margin = new Thickness(0, 5, 0, 0),
                ItemsSource = consumerList
            };

            var inputFields = CreateInputFields(a => {
                consumerList.Add(a);
                CollectionViewSource.GetDefaultView(attributeListView.ItemsSource).Refresh();
            });


            var deleteButton = ComponentUtils.ButtonWithIcon(AppResources.GetCloseSignIcon);
            deleteButton.Click += (sender, args) => {
                var selectedItem = attributeListView.SelectedItem;
                consumerList.Remove((Attribute) selectedItem);
                CollectionViewSource.GetDefaultView(attributeListView.ItemsSource).Refresh();
            };
            inputFields.Children.Add(deleteButton);


            var tabContent = new StackPanel() {Orientation = Orientation.Vertical};
            tabContent.Children.Add(inputFields);
            tabContent.Children.Add(attributeListView);

            tabItem.Content = tabContent;
            return tabItem;
        }


        private static StackPanel CreateInputFields(Action<Attribute> onAdd) {
            var inputFields = new StackPanel() {Orientation = Orientation.Horizontal};

            var nameBox = new TextBox() {
                FontSize = 16,
                MinWidth = 120,
                Margin = new Thickness(0, 0, 5, 0)
            };
            inputFields.Children.Add(nameBox);

            var attributes = new ComboBox {
                ItemsSource = Enum.GetValues(typeof(AttributeType)), SelectedItem = AttributeType.Number,
                Margin = new Thickness(0, 0, 5, 0)
            };
            attributes.SelectionChanged += (sender, args) => {
                UIElement newElement;
                switch ((AttributeType) attributes.SelectedItem) {
                    case AttributeType.Number: {
                        var doubleTextBox = ComponentUtils.DoubleTextBox();
                        doubleTextBox.Margin = new Thickness(0, 0, 5, 0);
                        newElement = doubleTextBox;
                        break;
                    }
                    case AttributeType.String: {
                        newElement = new TextBox() {
                            FontSize = 16,
                            MinWidth = 120,
                            Margin = new Thickness(0, 0, 5, 0)
                        };
                        break;
                    }
                    case AttributeType.Boolean: {
                        newElement = new CheckBox() {
                            FontSize = 16,
                            MinWidth = 120,
                            Margin = new Thickness(0, 0, 5, 0)
                        };
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

            var numBox = ComponentUtils.DoubleTextBox();
            numBox.Margin = new Thickness(0, 0, 5, 0);
            inputFields.Children.Add(numBox);


            var addButton = ComponentUtils.ButtonWithIcon(AppResources.GetPlusSignIcon);
            addButton.Click += (sender, args) => {
                object value;
                var valueProducer = inputFields.Children[2];
                switch ((AttributeType) attributes.SelectedItem) {
                    case AttributeType.Number: {
                        try {
                            value = double.Parse(((TextBox) valueProducer).Text);
                        }
                        catch (Exception) {
                            value = 0.0;
                        }

                        ((TextBox) valueProducer).Text = "";
                        break;
                    }
                    case AttributeType.String: {
                        value = ((TextBox) valueProducer).Text;
                        ((TextBox) valueProducer).Text = "";
                        break;
                    }
                    case AttributeType.Boolean: {
                        value = ((CheckBox) valueProducer).IsChecked;
                        ((CheckBox) valueProducer).IsChecked = false;
                        break;
                    }
                    default: {
                        throw new Exception("Illegal state");
                    }
                }

                onAdd.Invoke(new Attribute()
                    {Name = nameBox.Text, Type = (AttributeType) attributes.SelectedItem, Value = value});
                nameBox.Text = "";
            };
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