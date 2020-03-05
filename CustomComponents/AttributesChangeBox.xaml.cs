using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TransportGraphApp.Models;
using Attribute = TransportGraphApp.Models.Attribute;

namespace TransportGraphApp.CustomComponents {
    public partial class AttributesChangeBox : UserControl {
        public IList<Attribute> UpdatedAttributes { get; } = new List<Attribute>();

        public AttributesChangeBox(IEnumerable<Attribute> attributes) {
            InitializeComponent();
            foreach (var a in attributes) {
                var attributeToUpdate = new Attribute() {
                    Name = a.Name,
                    Type = a.Type,
                    Value = a.Value
                };
                UpdatedAttributes.Add(attributeToUpdate);
                var attributeRow = CreateAttributeRow(attributeToUpdate);
                StackPanel.Children.Add(attributeRow);
            }
        }

        public Attribute AddField(Attribute a) {
            var attributeToUpdate = new Attribute() {
                Name = a.Name,
                Type = a.Type,
                Value = a.Value
            };
            var attributeRow = CreateAttributeRow(attributeToUpdate);
            StackPanel.Children.Insert(0, attributeRow);
            return attributeToUpdate;
        }

        public static StackPanel CreateAttributeRow(Attribute attribute) {
            var stackPanel = new StackPanel() {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 5, 0, 0)
            };

            var name = new Label() {
                HorizontalAlignment = HorizontalAlignment.Left,
                Content = attribute.Name,
                Width = 120
            };
            stackPanel.Children.Add(name);

            var type = new Label() {
                HorizontalAlignment = HorizontalAlignment.Left,
                Content = attribute.Type,
                Width = 120
            };
            stackPanel.Children.Add(type);

            switch (attribute.Type) {
                case AttributeType.Number: {
                    var doubleTextBox = new DoubleTextBox {
                        Value = (double) attribute.Value
                    };
                    doubleTextBox.ValueChanged(v => attribute.Value = v);
                    stackPanel.Children.Add(doubleTextBox);
                    break;
                }
                case AttributeType.String: {
                    var textBox = new StringTextBox() {
                        Value = attribute.Value.ToString()
                    };
                    textBox.ValueChanged(v => attribute.Value = v);
                    stackPanel.Children.Add(textBox);
                    break;
                }
                case AttributeType.Boolean: {
                    var checkBox = new TrueFalseBox() {
                        Value = (bool) attribute.Value
                    };
                    checkBox.ValueChanged(v => attribute.Value = v);
                    stackPanel.Children.Add(checkBox);
                    break;
                }
                default: {
                    throw new Exception("Illegal state");
                }
            }

            return stackPanel;
        }
    }
}