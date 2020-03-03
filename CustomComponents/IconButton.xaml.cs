using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;

namespace TransportGraphApp.CustomComponents {
    public partial class IconButton : UserControl {
        public IconButton(ImageSource icon, ThreadStart onClick) {
            InitializeComponent();
            var img = new Image { Source = icon };

            // without this do not work :(
            Console.WriteLine(img.Source.Height);

            var stackPnl = new StackPanel { Orientation = Orientation.Horizontal };
            stackPnl.Children.Add(img);

            Button.Content = stackPnl;
            Button.Click += (sender, args) => onClick();
        }
    }
}