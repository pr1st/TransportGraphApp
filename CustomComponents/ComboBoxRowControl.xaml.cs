using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TransportGraphApp.CustomComponents {
    public partial class ComboBoxRowControl : UserControl {
        public ComboBoxRowControl() {
            InitializeComponent();
        }
    }

    public class GenericComboBoxRowControl<T> where T : Enum {
        private readonly ComboBoxRowControl _comboBoxRowControl = new ComboBoxRowControl();
        private readonly IDictionary<T, string> _typeToDescription = new Dictionary<T, string>();
        private readonly IDictionary<string, T> _descriptionToType = new Dictionary<string, T>();

        public string TitleValue {
            get => _comboBoxRowControl.StringTitle.Text;
            set => _comboBoxRowControl.StringTitle.Text = value;
        }

        public string TitleToolTip {
            get => (string) _comboBoxRowControl.StringTitle.ToolTip;
            set => _comboBoxRowControl.StringTitle.ToolTip = value;
        }

        public T Selected {
            get =>  _descriptionToType[(string) _comboBoxRowControl.ValueBox.SelectedItem];
            set => _comboBoxRowControl.ValueBox.SelectedItem = _typeToDescription[value];
        }
        
        public GenericComboBoxRowControl(IDictionary<T, string> descriptionMap) {
            _comboBoxRowControl.ValueBox.SelectionChanged += (sender, args) => {
                var info = descriptionMap[Selected];
                _comboBoxRowControl.InfoBox.Text = info;
            };
            
            foreach (var type in descriptionMap.Keys) {
                _typeToDescription[type] = type.GetDescription();
                _descriptionToType[type.GetDescription()] = type;
            }
            
            _comboBoxRowControl.ValueBox.ItemsSource = descriptionMap.Keys.Select(t => t.GetDescription());
        }
        
        public UIElement GetUiElement => _comboBoxRowControl;
    }
}