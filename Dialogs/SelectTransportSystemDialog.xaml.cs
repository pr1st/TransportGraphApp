using System;
using System.Collections.Generic;
using System.Windows;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Models;

namespace TransportGraphApp.Dialogs {
    public partial class SelectTransportSystemDialog : Window {
        private GenericEntityListControl<TransportSystem> _entityList;

        public TransportSystem SelectedSystem => _entityList.Selected;

        public SelectTransportSystemDialog() {
            InitializeComponent();
            Owner = App.Window;
            Icon = AppResources.GetAppIcon;

            var propertyMatcher = new Dictionary<string, Func<TransportSystem, object>> {
                {
                    "Название", 
                    ts => ts.Name
                }, {
                    "Кол-во нас. пунктов",
                    ts => App.DataBase.GetCollection<City>().Count(c => c.TransportSystemId == ts.Id)
                }, {
                    "Кол-во маршрутов",
                    ts => App.DataBase.GetCollection<Road>().Count(r => r.TransportSystemId == ts.Id)
                }
            };
            _entityList = new GenericEntityListControl<TransportSystem>(
                "Доступные транспортные системы",
                propertyMatcher,
                ts => { });
            
            ListPanel.Children.Add(_entityList.GetUiElement());
            _entityList.SetSource(App.DataBase.GetCollection<TransportSystem>().FindAll());
        }
        
        private void SelectClick(object sender, RoutedEventArgs e) {
            if (SelectedSystem == null) {
                ComponentUtils.ShowMessage("Выберите транспортную систему из списка", MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
        }
    }
}