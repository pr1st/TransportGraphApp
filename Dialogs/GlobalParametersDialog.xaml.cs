using System.Windows;

namespace TransportGraphApp.Dialogs {
    public partial class GlobalParametersDialog : Window {
        public GlobalParametersDialog() {
            InitializeComponent();
            Owner = App.Window;
            Icon = AppResources.GetAppIcon;
            
            // _availableRoadTypesControl = new GenericTableRowControl<string>() {
            //     TitleValue = "Используемые типы дорог",
            //     TitleToolTip = "Представляет собой набор допустимых типов маршрутов в данной системе, используется при добавлении или обновлении маршрута",
            //     OnAdd = roadTypes => {
            //         var d = new StringFieldDialog() {
            //             Title = "Новый тип дороги",
            //             IsViable = newRoadTypeName => {
            //                 if (newRoadTypeName.Trim() == "") {
            //                     ComponentUtils.ShowMessage("Введите не пустое название", MessageBoxImage.Error);
            //                     return false;
            //                 }
            //
            //                 if (roadTypes.Contains(newRoadTypeName.Trim())) {
            //                     ComponentUtils.ShowMessage("Тип дороги с таким названием уже существует",
            //                         MessageBoxImage.Error);
            //                     return false;
            //                 }
            //
            //                 return true;
            //             }
            //         };
            //         d.FieldName.Text = "Введите название";
            //         d.FieldValue.Text = "";
            //         return d.ShowDialog() != true ? null : d.FieldValue.Text;
            //     }
            // };
            // _availableRoadTypesControl.AddColumn("Название", s => s);
        }
    }
}