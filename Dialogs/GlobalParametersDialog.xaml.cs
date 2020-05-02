using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TransportGraphApp.Actions;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Dialogs.ResultDialogs;
using TransportGraphApp.Models;

namespace TransportGraphApp.Dialogs {
    public partial class GlobalParametersDialog : Window {
        // updateable properties
        private static CityTags _cityTags;
        private static RoadTypes _roadTypes;

        // ui controls
        private GenericTableRowControl<CityTag> _cityTagsControl;
        private GenericTableRowControl<RoadType> _roadTypesControl;
        
        public GlobalParametersDialog() {
            InitializeComponent();
            Owner = App.Window;
            Icon = AppResources.GetAppIcon;

            _cityTags = App.DataBase.GetCollection<CityTags>().FindOne(ct => ct.IsPrimary);
            _roadTypes = App.DataBase.GetCollection<RoadTypes>().FindOne(rt => rt.IsPrimary);
            
            InitCityTagsControl();
            InitRoadTypesControl();

            Closed += (sender, args) => CancelClick();
        }

        // init methods
        private void InitCityTagsControl() {
            _cityTagsControl = new GenericTableRowControl<CityTag>() {
                TitleValue = "Используемые типы населенных пунктов",
                TitleToolTip = "Представляет собой набор допустимых типов населенных пунктов в сети, используется при добавлении или обновлении населенного пункта",
                OnAdd = alreadyUsedCityTags => {
                    var addDialog = new AddStringDialog() {
                        Title = "Новый тип населенного пункта",
                        IsViable = newCityTagName => {
                            if (newCityTagName.Trim() == "") {
                                ComponentUtils.ShowMessage("Введите не пустое название", MessageBoxImage.Error);
                                return false;
                            }

                            if (alreadyUsedCityTags.Contains(new CityTag() { Name = newCityTagName.Trim()})) {
                                ComponentUtils.ShowMessage("Тип населенного пункта с таким названием уже существует",
                                    MessageBoxImage.Error);
                                return false;
                            }

                            return true;
                        },
                        RowControl = { 
                            TitleValue = "Введите название",
                            Value = ""
                        }
                    };
                    
                    if (addDialog.ShowDialog() != true) return null;
                    
                    var created = new CityTag() { Name = addDialog.RowControl.Value};
                    return new List<CityTag>() {created};
                },
                Value = _cityTags.Values
            };
            _cityTagsControl.AddColumns(CityTag.PropertyMatcher());
            
            PropertiesPanel.Children.Add(_cityTagsControl.GetUiElement);
        }

        private void InitRoadTypesControl() {
            _roadTypesControl = new GenericTableRowControl<RoadType>() {
                TitleValue = "Используемые типы дорог",
                TitleToolTip = "Представляет собой набор допустимых типов маршрутов в сети, используется при добавлении или обновлении маршрута",
                OnAdd = alreadyUsedRoadTypes => {
                    var addDialog = new AddStringDialog() {
                        Title = "Новый тип дороги",
                        IsViable = newRoadTypeName => {
                            if (newRoadTypeName.Trim() == "") {
                                ComponentUtils.ShowMessage("Введите не пустое название", MessageBoxImage.Error);
                                return false;
                            }

                            if (alreadyUsedRoadTypes.Contains(new RoadType() { Name = newRoadTypeName.Trim()})) {
                                ComponentUtils.ShowMessage("Тип дороги с таким названием уже существует",
                                    MessageBoxImage.Error);
                                return false;
                            }

                            return true;
                        },
                        RowControl = { 
                            TitleValue = "Введите название",
                            Value = ""
                        }
                    };
                    if (addDialog.ShowDialog() != true) return null;
                    
                    var created = new RoadType() { Name = addDialog.RowControl.Value};
                    return new List<RoadType>() {created};
                },
                Value = _roadTypes.Values
            };
            _roadTypesControl.AddColumns(RoadType.PropertyMatcher());

            PropertiesPanel.Children.Add(_roadTypesControl.GetUiElement);
        }
        
        
        private void CancelClick() {
            _cityTags.Values = _cityTagsControl.Value;
            App.DataBase.GetCollection<CityTags>().Update(_cityTags);
            
            _roadTypes.Values = _roadTypesControl.Value;
            App.DataBase.GetCollection<RoadTypes>().Update(_roadTypes);
        }
    }
}