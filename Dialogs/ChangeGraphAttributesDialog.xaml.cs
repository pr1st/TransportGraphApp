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
using TransportGraphApp.Models;
using Attribute = TransportGraphApp.Models.Attribute;

namespace TransportGraphApp.Dialogs {
    public partial class ChangeGraphAttributesDialog : Window {
        public ChangeGraphAttributesDialog(Graph graph) {
            InitializeComponent();
            GraphNameLabel.Content = graph.Name;
            foreach (var graphGraphAttribute in graph.GraphAttributes) {
                AttributePanel.Children.Add(CreateAttributeRow(graphGraphAttribute));
            }
        }

        private static StackPanel CreateAttributeRow(Attribute attribute) {
            var stackPanel = new StackPanel() {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 5, 0, 0)
            };
            var name = new Label() {
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 16,
                Content = attribute.Name,
                Width = 120
            };
            stackPanel.Children.Add(name);

            var type = new Label() {
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 16,
                Content = attribute.Type,
                Width = 120
            };
            stackPanel.Children.Add(type);

            switch (attribute.Type) {
                case AttributeType.Number: {
                    var doubleTextBox = ComponentUtils.DoubleTextBox();
                    doubleTextBox.Text = attribute.DefaultValue.ToString();
                    doubleTextBox.Width = 120;
                    doubleTextBox.HorizontalAlignment = HorizontalAlignment.Center;
                    doubleTextBox.VerticalAlignment = VerticalAlignment.Center;
                    doubleTextBox.TextChanged += (sender, args) => attribute.DefaultValue = doubleTextBox.Text;
                    stackPanel.Children.Add(doubleTextBox);
                    break;
                }
                case AttributeType.String: {
                    var textBox = new TextBox {
                        Text = attribute.DefaultValue.ToString(),
                        Width = 120,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    textBox.TextChanged += (sender, args) => attribute.DefaultValue = textBox.Text;
                    stackPanel.Children.Add(textBox);
                    break;
                }
                case AttributeType.Boolean: {
                    var checkBox = new CheckBox {
                        IsChecked = (bool?) attribute.DefaultValue,
                        Width = 120,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    checkBox.Checked += (sender, args) => attribute.DefaultValue = checkBox.IsChecked;
                    checkBox.Unchecked += (sender, args) => attribute.DefaultValue = checkBox.IsChecked;
                    stackPanel.Children.Add(checkBox);
                    break;
                }
                default: {
                    throw new Exception("Illegal state");
                }
            }

            return stackPanel;
        }

        private void OkClicked(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }
    }
}