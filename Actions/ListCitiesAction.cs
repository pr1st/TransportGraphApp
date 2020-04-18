using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LiteDB;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Dialogs;
using TransportGraphApp.Models;

namespace TransportGraphApp.Actions {
    public static class ListCitiesAction {
        public static void Invoke() {
            var selectDialog = new SelectTransportSystemDialog();
            if (selectDialog.ShowDialog() != true) return;

            _selectedSystem = selectDialog.SelectedSystem;
            _cityTags = App.DataBase.GetCollection<CityTags>().FindOne(ct => ct.IsPrimary);
            
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
            genericEntityDialog.AddColumn("Кол.во Тэгов", c => c.Tags.Count);
            
            _nameControl = new StringWithHelpRowControl() {
                TitleValue = "Название",
                TitleToolTip = "Представляет собой уникальный индетификатор населенного пункта, в разных транспортных системах нас. пункт будет одним и тем же, если у них названия совпадают",
                OnEnterPressed = () => {
                    if (!_allCitiesList.Select(c => c.Name).Contains(_nameControl.Value)) return;
                    var city = _allCitiesList.First(c => c.Name == _nameControl.Value);
                    _costOfStayingControl.Value = city.CostOfStaying;
                    _tagsControl.Value = city.Tags;
                }
            };
            
            _costOfStayingControl = new PositiveDoubleRowControl() {
                TitleValue = "Стоимость проживания в данном населенном пункте",
                TitleToolTip = "Представляет собой среднюю стоимость проживания в данном нас. пункте (в условных денежных единицах в день), " +
                               "используется в комплексном алгоритме основанным на денежных затратах"
            };

            _tagsControl = new GenericTableRowControl<CityTag>() {
                TitleValue = "Тэги нас. пункта",
                TitleToolTip = "Представляет собой набор тэгов которые принадлежат данному нас. пункту, используется при спецификации алгоритма",
                OnAdd = cityTags => {
                    var d = new StringFieldDialog {
                        Title = "Добавить тэг населенному пункту",
                        IsViable = tagName => {
                            if (tagName.Trim() == "") {
                                ComponentUtils.ShowMessage("Введите не пустое название", MessageBoxImage.Error);
                                return false;
                            }
                            
                            if (cityTags.Select(ct => ct.Name).Contains(tagName.Trim())) {
                                ComponentUtils.ShowMessage("В списке уже есть данный тэг", MessageBoxImage.Error);
                                return false;
                            }

                            if (!_cityTags.Values.Select(ct => ct.Name).Contains(tagName.Trim())) {
                                ComponentUtils.ShowMessage("Название должно представлять собой один из тэгов населенных пунктов (предоставленных в выпадающем окошке)", MessageBoxImage.Error);
                                return false;
                            }

                            return true;
                        },
                        RowControl = {
                            TitleValue = "Введите название тэга",
                            Value = "",
                            HelpingValues = _cityTags.Values.Select(ct => ct.Name).ToList()
                        }
                    };
                    return d.ShowDialog() != true ? null : new CityTag() { Name = d.RowControl.Value};
                }
            };
            _tagsControl.AddColumn("Название", ct => ct.Name);
            
            genericEntityDialog.AddProperty(
                _nameControl,
                () => _nameControl.Value = "",
                с => _nameControl.Value = с.Name);
            
            genericEntityDialog.AddProperty(
                _costOfStayingControl,
                () => _costOfStayingControl.Value = 0,
                с => _costOfStayingControl.Value = с.CostOfStaying);
            
            genericEntityDialog.AddProperty(
                _tagsControl.GetUiElement,
                () => _tagsControl.Value = new List<CityTag>(),
                c => _tagsControl.Value = c.Tags);
            
            genericEntityDialog.ShowDialog();
        }

        private static TransportSystem _selectedSystem;
        private static CityTags _cityTags;
        
        private static IList<City> _citiesList;
        private static IList<City> _allCitiesList;
        
        private static StringWithHelpRowControl _nameControl;
        private static PositiveDoubleRowControl _costOfStayingControl;
        private static GenericTableRowControl<CityTag> _tagsControl; 
        
        private static bool AddCity() {
            if (_nameControl.Value == "") {
                ComponentUtils.ShowMessage("Введите не пустое название", MessageBoxImage.Error);
                return false;
            }

            if (_citiesList.Select(с => с.Name).Contains(_nameControl.Value)) {
                ComponentUtils.ShowMessage("Населенный пункт с таким названием уже существует в данной системе", MessageBoxImage.Error);
                return false;
            }

            if (_allCitiesList.Select(с => с.Name).Contains(_nameControl.Value)) {
                var newCity = _allCitiesList.First(c => c.Name == _nameControl.Value);
                newCity.CostOfStaying = _costOfStayingControl.Value;
                newCity.Tags = _tagsControl.Value;
                newCity.TransportSystemIds.Add(_selectedSystem.Id);
                App.DataBase.GetCollection<City>().Update(newCity);
            }
            else {
                App.DataBase.GetCollection<City>().Insert(new City() {
                    Name = _nameControl.Value,
                    CostOfStaying = _costOfStayingControl.Value,
                    Tags = _tagsControl.Value,
                    TransportSystemIds = new List<ObjectId>() {_selectedSystem.Id}
                });
            }

            return true;
        }
        
        private static bool UpdateCity(City selectedCity) {
            if (_nameControl.Value == "") {
                ComponentUtils.ShowMessage("Введите не пустое название", MessageBoxImage.Error);
                return false;
            }

            if (selectedCity.Name != _nameControl.Value &&
                _citiesList.Select(t => t.Name).Contains(_nameControl.Value)) {
                ComponentUtils.ShowMessage("Населенный пункт с таким названием в этой системе уже существует", MessageBoxImage.Error);
                return false;
            }

            selectedCity.Name = _nameControl.Value;
            selectedCity.CostOfStaying = _costOfStayingControl.Value;
            selectedCity.Tags = _tagsControl.Value;
            
            App.DataBase.GetCollection<City>().Update(selectedCity);
            ComponentUtils.ShowMessage("Данный населенный пункт был обновлен", MessageBoxImage.Information);
            return true;
        }
        
        private static bool RemoveCity(City selectedCity) {
            App.DataBase.GetCollection<Road>().DeleteMany(r => r.TransportSystemId == _selectedSystem.Id && 
                                                               (r.FromCityId == selectedCity.Id || r.ToCityId == selectedCity.Id));
            
            if (selectedCity.TransportSystemIds.Count() != 1) {
                selectedCity.TransportSystemIds.Remove(_selectedSystem.Id);
                App.DataBase.GetCollection<City>().Update(selectedCity);
            }
            else {
                App.DataBase.GetCollection<City>().Delete(selectedCity.Id);
            }
            
            return true;
        }

        private static IEnumerable<City> UpdateCollection() {
            _allCitiesList = App.DataBase.GetCollection<City>().FindAll().ToList();
            _citiesList = _allCitiesList.Where(c => c.TransportSystemIds.Contains(_selectedSystem.Id)).ToList();
            _nameControl.HelpingValues = _allCitiesList.Select(c => c.Name).ToList();
            return _citiesList;
        }
        
        // todo saving for future button 
        // private void ImportFromFileClick() {
        //     var sb = new StringBuilder();
        //     var msg1 = "Файл должен быть в json формате";
        //     var msg2 = "В файле корневой элемент должен быть массив состоящий из нас. пунктов";
        //     var msg3 = "Поля которые используются:";
        //     var msg4 = "Name - нименование нас. пункта";
        //     var msg5 = "Latitude - широта";
        //     var msg6 = "Longitude - долгота";
        //     var msg7 = "CostOfStaying - Стоимость проживания в городе";
        //     var msg8 = "Пример корректного файла:";
        //     var msg9 =
        //         "[\n" +
        //         "    {\n" +
        //         "        \"Name\":\"Адыгейск\",\n" +
        //         "        \"Latitude\":44.878414,\n" +
        //         "        \"Longitude\":39.190289,\n" +
        //         "        \"CostOfStaying\":123\n" +
        //         "    },\n" +
        //         "    {\n" +
        //         "        \"Name\":\"Майкоп\",\n" +
        //         "        \"Latitude\":44.6098268,\n" +
        //         "        \"Longitude\":40.1006527,\n" +
        //         "        \"CostOfStaying\":234\n" +
        //         "    }\n" +
        //         "]";
        //     sb.AppendLine(msg1);
        //     sb.AppendLine(msg2);
        //     sb.AppendLine(msg3);
        //     sb.AppendLine(msg4);
        //     sb.AppendLine(msg5);
        //     sb.AppendLine(msg6);
        //     sb.AppendLine(msg7);
        //     sb.AppendLine(msg8);
        //     sb.AppendLine(msg9);
        //     ComponentUtils.ShowMessage(sb.ToString(), MessageBoxImage.Information);
        //
        //     var openFileDialog = new OpenFileDialog() {
        //         Filter = "json files (*.json)|*.json",
        //         InitialDirectory = Directory.GetCurrentDirectory()
        //     };
        //     
        //     if (openFileDialog.ShowDialog() != true) return;
        //
        //     try {
        //         var cities = JsonSerializer.Deserialize<IList<City>>(File.ReadAllText(openFileDialog.FileName));
        //         var allNames = cities
        //             .Select(c => c.Name)
        //             .Concat(_currentCitiesList.Select(c => c.Name))
        //             .ToList();
        //         if (allNames.Distinct().Count() != allNames.Count()) {
        //             ComponentUtils.ShowMessage("Выбранной файл содержит названия нас. пунктов которые уже есть в списке " +
        //                                        "или сам содержит дубликаты названий нас. пунктов", MessageBoxImage.Error);
        //             return;
        //         }
        //         foreach (var c in cities) {
        //             // c.TransportSystemId = _selectedTransportSystem.Id;
        //             App.DataBase.GetCollection<City>().Insert(c);   
        //         }
        //         UpdateState();
        //         DisplayNew();
        //     }
        //     catch (JsonException) {
        //         ComponentUtils.ShowMessage("Выбранной файл представлен в неверном формате", MessageBoxImage.Error);
        //     } 
        // }
    }
}