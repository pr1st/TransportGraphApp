using System.Collections.Generic;
using System.Windows.Controls;

namespace TransportGraphApp.CustomComponents {
    public partial class ComboBoxRowControl : UserControl {
        
        private IList<string> _itemNames = new List<string>();
        private IList<string> _displayedNames = new List<string>();
        private IList<string> _itemsInfo = new List<string>();
        
        public ComboBoxRowControl() {
            InitializeComponent();
            ValueBox.SelectionChanged += (sender, args) => {
                var info = _itemsInfo[_displayedNames.IndexOf((string) ValueBox.SelectedItem)];
                InfoBox.Text = info;
            };
        }
        
        public string Selected {
            get => _itemNames[_displayedNames.IndexOf((string) ValueBox.SelectedItem)];
            set => ValueBox.SelectedItem = _displayedNames[_itemNames.IndexOf(value)];
        }

        public string TitleValue {
            get => StringTitle.Text;
            set => StringTitle.Text = value;
        }
        
        public string TitleToolTip {
            get => (string)StringTitle.ToolTip;
            set => StringTitle.ToolTip = value;
        }

        public void AddItem(string itemName, string itemDisplayedName, string itemDescription) {
            _itemNames.Add(itemName);
            _displayedNames.Add(itemDisplayedName);
            _itemsInfo.Add(itemDescription);
            ValueBox.Items.Add(itemDisplayedName);
        }
    }
}