using System.Collections.Generic;
using TransportGraphApp.Dialogs;
using TransportGraphApp.Models;

namespace TransportGraphApp.Actions {
    public static class ListCitiesAction {
        public static void Invoke() {
            var selectDialog = new SelectTransportSystemDialog();
            if (selectDialog.ShowDialog() != true) return;

            selectedSystem = selectDialog.SelectedSystem;
            
            var genericEntityDialog = new GenericEntityDialog<City>() {
                Title = "Населенные пункты",
                ListTitle = "Доступные населенные пункты",
                OpenAddNewItemWindowButtonTitle = "Открыть окно для добавления населенного пункта",
                AddNewItemWindowTitle = "Добавить населенный пункт",
                UpdateItemWindowTitle = "Обновить населенный пункт",
                AddItemFunction = AddCity,
                UpdateItemFunction = UpdateCity,
                RemoveItemFunction = RemoveCity,
                UpdateCollectionFunction = UpdateCollection
            };
            
            genericEntityDialog.AddColumn("Название", c => c.Name);
            genericEntityDialog.AddColumn("Стоимость проживания", c => c.CostOfStaying);
        }

        private static TransportSystem selectedSystem;
        
        private static bool AddCity() {
            return true;
        }
        
        private static bool UpdateCity(City selectedCity) {
            return true;
        }
        
        private static bool RemoveCity(City selectedCity) {
            return false;
        }

        private static IEnumerable<City> UpdateCollection() {
            return null;
        }
    }
}