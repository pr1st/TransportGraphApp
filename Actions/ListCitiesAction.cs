using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Dialogs;
using TransportGraphApp.Dialogs.ResultDialogs;
using TransportGraphApp.Models;

namespace TransportGraphApp.Actions {
    public static class ListCitiesAction {
        // unmodifiable properties
        private static CityTags _availableCityTags;
        private static IList<TransportSystem> _availableTransportSystems;

        // updateable properties
        private static IList<City> _citiesList;

        // ui controls
        private static GenericEntityDialog<City> _dialog;
        private static StringRowControl _nameControl;
        private static PositiveDoubleRowControl _costOfStayingControl;
        private static GenericTableRowControl<CityTag> _tagsControl;
        private static GenericTableRowControl<TransportSystem> _transportSystemsControl;

        public static void Invoke() {
            _availableCityTags = App.DataBase.GetCollection<CityTags>().FindOne(ct => ct.IsPrimary);
            _availableTransportSystems = App.DataBase.GetCollection<TransportSystem>().FindAll().ToList();

            _dialog = new GenericEntityDialog<City>() {
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

            _dialog.AddColumns(City.PropertyMatcher());

            InitNameProperty();
            InitCostOfStayingProperty();
            InitTagsProperty();
            InitTransportSystemsProperty();

            UpdateCollection();
            _dialog.ShowDialog();
        }

        // callback methods
        private static bool AddCity() {
            if (!IsViable(null)) return false;

            App.DataBase.GetCollection<City>().Insert(new City() {
                Name = _nameControl.Value,
                CostOfStaying = _costOfStayingControl.Value,
                Tags = _tagsControl.Value,
                TransportSystemIds = _transportSystemsControl.Value.Select(ts => ts.Id).ToList()
            });

            return true;
        }

        private static bool UpdateCity(City selectedCity) {
            if (!IsViable(selectedCity.Name)) return false;

            selectedCity.Name = _nameControl.Value;
            selectedCity.CostOfStaying = _costOfStayingControl.Value;
            selectedCity.Tags = _tagsControl.Value;
            selectedCity.TransportSystemIds = _transportSystemsControl.Value.Select(ts => ts.Id).ToList();

            App.DataBase.GetCollection<City>().Update(selectedCity);
            ComponentUtils.ShowMessage("Данный населенный пункт был обновлен", MessageBoxImage.Information);
            return true;
        }

        private static bool RemoveCity(City selectedCity) {
            App.DataBase.GetCollection<Road>()
                .DeleteMany(r => r.FromCityId == selectedCity.Id || r.ToCityId == selectedCity.Id);

            App.DataBase.GetCollection<City>().Delete(selectedCity.Id);

            return true;
        }

        // support method for callback methods
        private static bool IsViable(string previousName) {
            if (_nameControl.Value == "") {
                ComponentUtils.ShowMessage("Введите не пустое название населенного пункта", MessageBoxImage.Error);
                return false;
            }

            if (_citiesList.Select(с => с.Name).Contains(_nameControl.Value)
                && previousName != _nameControl.Value) {
                ComponentUtils.ShowMessage("Населенный пункт с таким названием уже существует", MessageBoxImage.Error);
                return false;
            }

            if (!_transportSystemsControl.Value.Any()) {
                ComponentUtils.ShowMessage(
                    "Населенный пункт должен принадлежать по крайней мере одной траснортной системе",
                    MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        // init methods
        private static void InitNameProperty() {
            _nameControl = new StringRowControl() {
                TitleValue = "Название",
                TitleToolTip = "Представляет собой уникальный индетификатор населенного пункта, " +
                               "в разных транспортных системах населенный пункт будет одним и тем же, если у них названия совпадают"
            };
            _dialog.AddProperty(
                _nameControl,
                () => _nameControl.Value = "",
                с => _nameControl.Value = с.Name);
        }

        private static void InitCostOfStayingProperty() {
            _costOfStayingControl = new PositiveDoubleRowControl() {
                TitleValue = "Стоимость проживания в данном населенном пункте",
                TitleToolTip =
                    "Представляет собой среднюю стоимость проживания в данном нас. пункте (в условных денежных единицах в день), " +
                    "используется в комплексном алгоритме основанным на денежных затратах"
            };
            _dialog.AddProperty(
                _costOfStayingControl,
                () => _costOfStayingControl.Value = 0,
                с => _costOfStayingControl.Value = с.CostOfStaying);
        }

        private static void InitTagsProperty() {
            _tagsControl = new GenericTableRowControl<CityTag>() {
                TitleValue = "Тэги нас. пункта",
                TitleToolTip =
                    "Представляет собой набор тэгов которые принадлежат данному нас. пункту, используется при спецификации алгоритма",
                OnAdd = alreadyUsedCityTags => {
                    var dialog = new GenericSelectEntitiesDialog<CityTag>(
                        "Добавить тэги к населенному пункту",
                        CityTag.PropertyMatcher(),
                        _availableCityTags.Values.Where(ct => !alreadyUsedCityTags.Contains(ct)));
                    return dialog.Selected;
                }
            };
            _tagsControl.AddColumns(CityTag.PropertyMatcher());

            _dialog.AddProperty(
                _tagsControl.GetUiElement,
                () => _tagsControl.Value = new List<CityTag>(),
                c => _tagsControl.Value = c.Tags);
        }

        private static void InitTransportSystemsProperty() {
            _transportSystemsControl = new GenericTableRowControl<TransportSystem>() {
                TitleValue = "Транспортные системы",
                TitleToolTip =
                    "Список транспортных систем к которым принадлежит данный населенный пункт",
                OnAdd = alreadyUsedTransportSystems => {
                    var dialog = new GenericSelectEntitiesDialog<TransportSystem>(
                        "Добавить транспортные системы к населенному пункту",
                        TransportSystem.PropertyMatcher(),
                        _availableTransportSystems.Where(ts => !alreadyUsedTransportSystems.Contains(ts)));
                    return dialog.Selected;
                }
            };
            _transportSystemsControl.AddColumns(TransportSystem.PropertyMatcher());

            _dialog.AddProperty(
                _transportSystemsControl.GetUiElement,
                () => _transportSystemsControl.Value = new List<TransportSystem>(),
                c => _transportSystemsControl.Value =
                    _availableTransportSystems.Where(ts => c.TransportSystemIds.Contains(ts.Id)).ToList());
        }

        // update state method
        private static IEnumerable<City> UpdateCollection() {
            _citiesList = App.DataBase.GetCollection<City>().FindAll().ToList();
            return _citiesList;
        }
    }
}