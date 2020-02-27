using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TransportGraphApp.Models;
using Attribute = TransportGraphApp.Models.Attribute;

namespace TransportGraphApp {
    internal static class ComponentUtils {
        public static void ShowMessage(string message, MessageBoxImage icon) {
            MessageBox.Show(
                message,
                AppResources.GetAppTitle,
                MessageBoxButton.OK,
                icon
            );
        }

        public static TextBox DoubleTextBox() {
            var numBox = new TextBox() {
                FontSize = 16,
                MinWidth = 120
            };
            numBox.PreviewTextInput += NumberValidationTextBox;
            return numBox;
        }

        public static Button ButtonWithIcon(ImageSource icon) {
            var img = new Image {Source = icon};

            // without this do not work :(
            Console.WriteLine(img.Source.Height);

            var stackPnl = new StackPanel {Orientation = Orientation.Horizontal};
            stackPnl.Children.Add(img);

            var button = new Button {Content = stackPnl, Margin = new Thickness(5, 0, 5, 0)};
            return button;
        }

        private static void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            var initial = ((TextBox) sender).Text;
            var received = e.Text;
            var caretIndex = ((TextBox) sender).CaretIndex;
            string result;
            if (initial.Length > 0) {
                result = initial.Substring(0, caretIndex) + received +
                         initial.Substring(caretIndex, initial.Length - caretIndex);
            }
            else {
                result = received;
            }

            e.Handled = !double.TryParse(result, out _);
        }

        public static StackPanel CreateAttributeRow(Attribute attribute) {
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
                    doubleTextBox.Text = attribute.Value.ToString();
                    doubleTextBox.Width = 120;
                    doubleTextBox.HorizontalAlignment = HorizontalAlignment.Center;
                    doubleTextBox.VerticalAlignment = VerticalAlignment.Center;
                    doubleTextBox.TextChanged += (sender, args) => {
                        double.TryParse(doubleTextBox.Text, out var val);
                        attribute.Value = val;
                    };
                    stackPanel.Children.Add(doubleTextBox);
                    break;
                }
                case AttributeType.String: {
                    var textBox = new TextBox {
                        Text = attribute.Value.ToString(),
                        Width = 120,
                        FontSize = 16,
                        Margin = new Thickness(5,0,5,0),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    textBox.TextChanged += (sender, args) => attribute.Value = textBox.Text;
                    stackPanel.Children.Add(textBox);
                    break;
                }
                case AttributeType.Boolean: {
                    var checkBox = new CheckBox {
                        IsChecked = (bool?) attribute.Value,
                        Width = 120,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    checkBox.Checked += (sender, args) => attribute.Value = checkBox.IsChecked;
                    checkBox.Unchecked += (sender, args) => attribute.Value = checkBox.IsChecked;
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