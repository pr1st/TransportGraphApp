using System;
using System.Collections.Generic;
using System.Windows;
using TransportGraphApp.CustomComponents;

namespace TransportGraphApp.Dialogs.ResultDialogs {
    public partial class SelectEntitiesDialog : Window {
        public SelectEntitiesDialog() {
            InitializeComponent();
            Owner = App.Window;
            Icon = AppResources.GetAppIcon;
        }
    }

    public class GenericSelectEntitiesDialog<T> {
        private readonly SelectEntitiesDialog _selectEntitiesDialog = new SelectEntitiesDialog();
        private readonly GenericEntityListControl<T> _entityList;
        
        public IList<T> Selected { get; }

        public GenericSelectEntitiesDialog(string title, IDictionary<string, Func<T, object>> propertyMatcher, IEnumerable<T> entities) {
            _selectEntitiesDialog.Title = title;

            _selectEntitiesDialog.SelectButton.Click += (sender, args) => {
                if (_entityList.Selected == null) {
                    ComponentUtils.ShowMessage("Выберите элемент(ы) из списка", MessageBoxImage.Error);
                    return;
                }

                _selectEntitiesDialog.DialogResult = true;
            };

            _entityList = new GenericEntityListControl<T>(
                "Список",
                propertyMatcher,
                t => { });
            
            _selectEntitiesDialog.ListPanel.Children.Add(_entityList.GetUiElement());
            _entityList.SetSource(entities);

            var isSelected = _selectEntitiesDialog.ShowDialog();
            if (isSelected == true) {
                Selected = _entityList.SelectedAll();
            }
        }
    }
}