using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TransportGraphApp.Actions;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Models;

namespace TransportGraphApp.Dialogs {
    public partial class GlobalParametersDialog : Window {

        private GenericTableRowControl<CityTag> _availableCityTypesControl;
        private GenericTableRowControl<RoadType> _availableRoadTypesControl;

        private CityTags _initCityTags;
        private RoadTypes _initRoadTypes;

        public GlobalParametersDialog() {
            InitializeComponent();
            Owner = App.Window;
            Icon = AppResources.GetAppIcon;

            AddCollectionsIfNotExisted();
            
            _availableCityTypesControl = new GenericTableRowControl<CityTag>() {
                TitleValue = "Используемые типы населенных пунктов",
                TitleToolTip = "Представляет собой набор допустимых типов населенных пунктов в сети, используется при добавлении или обновлении населенного пункта",
                OnAdd = roadTypes => {
                    var d = new StringFieldDialog {
                        Title = "Новый тип населенного пункта",
                        IsViable = newRoadTypeName => {
                            if (newRoadTypeName.Trim() == "") {
                                ComponentUtils.ShowMessage("Введите не пустое название", MessageBoxImage.Error);
                                return false;
                            }

                            if (roadTypes.Select(rt => rt.Name).Contains(newRoadTypeName.Trim())) {
                                ComponentUtils.ShowMessage("Тип населенного пункта с таким названием уже существует",
                                    MessageBoxImage.Error);
                                return false;
                            }

                            return true;
                        },
                        FieldName = {Text = "Введите название"},
                        FieldValue = {Text = ""}
                    };
                    return d.ShowDialog() != true ? null : new CityTag() { Name = d.FieldValue.Text};
                },
                Value = _initCityTags.Values
            };
            _availableCityTypesControl.AddColumn("Название", c => c.Name);

            _availableRoadTypesControl = new GenericTableRowControl<RoadType>() {
                TitleValue = "Используемые типы дорог",
                TitleToolTip = "Представляет собой набор допустимых типов маршрутов в сети, используется при добавлении или обновлении маршрута",
                OnAdd = roadTypes => {
                    var d = new StringFieldDialog {
                        Title = "Новый тип дороги",
                        IsViable = newRoadTypeName => {
                            if (newRoadTypeName.Trim() == "") {
                                ComponentUtils.ShowMessage("Введите не пустое название", MessageBoxImage.Error);
                                return false;
                            }

                            if (roadTypes.Select(rt => rt.Name).Contains(newRoadTypeName.Trim())) {
                                ComponentUtils.ShowMessage("Тип дороги с таким названием уже существует",
                                    MessageBoxImage.Error);
                                return false;
                            }

                            return true;
                        },
                        FieldName = {Text = "Введите название"},
                        FieldValue = {Text = ""}
                    };
                    return d.ShowDialog() != true ? null : new RoadType() { Name = d.FieldValue.Text};
                },
                Value = _initRoadTypes.Values
            };
            _availableRoadTypesControl.AddColumn("Название", r => r.Name);

            PropertiesPanel.Children.Add(_availableCityTypesControl.GetUiElement);
            PropertiesPanel.Children.Add(_availableRoadTypesControl.GetUiElement);

            Closed += (sender, args) => CancelClick();
        }
        
        
        private void CancelClick() {
            Console.WriteLine("doin it");
            
            _initCityTags.Values = _availableCityTypesControl.Value;
            App.DataBase.GetCollection<CityTags>().Update(_initCityTags);
            
            _initRoadTypes.Values = _availableRoadTypesControl.Value;
            App.DataBase.GetCollection<RoadTypes>().Update(_initRoadTypes);
        }


        private void AddCollectionsIfNotExisted() {
            if (!App.DataBase.GetCollection<CityTags>().Find(ct => ct.IsPrimary).Any()) {
                App.DataBase.GetCollection<CityTags>().Insert(new CityTags() {IsPrimary = true});
            }
            if (!App.DataBase.GetCollection<RoadTypes>().Find(rt => rt.IsPrimary).Any()) {
                App.DataBase.GetCollection<RoadTypes>().Insert(new RoadTypes() {IsPrimary = true});
            }

            _initCityTags = App.DataBase.GetCollection<CityTags>().FindOne(ct => ct.IsPrimary);
            _initRoadTypes = App.DataBase.GetCollection<RoadTypes>().FindOne(rt => rt.IsPrimary);
        }
    }
}