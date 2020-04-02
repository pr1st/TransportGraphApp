using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TransportGraphApp.Models;

namespace TransportGraphApp.CustomComponents {
    public partial class DepartureTimeTableControl : UserControl {
        public DepartureTimeTableControl() {
            InitializeComponent();
            ComponentUtils.InsertIconToButton(AddButton, AppResources.GetAddItemIcon,
                "Добавить новое время отправления");
            ComponentUtils.InsertIconToButton(RemoveButton, AppResources.GetRemoveItemIcon,
                "Удалить выделенные элементы");
            _values = new List<DepartureTime>();
            TimeList.ItemsSource = _values;

            var column = new GridViewColumn() {
                Header = "Время",
                DisplayMemberBinding = new Binding() {
                    Converter = new TimeConverter()
                }
            };
            ((GridView) TimeList.View).Columns.Add(column);
            CreateWeekDayColumn("Пн", DayOfWeek.Monday);
            CreateWeekDayColumn("Вт", DayOfWeek.Tuesday);
            CreateWeekDayColumn("Ср", DayOfWeek.Wednesday);
            CreateWeekDayColumn("Чт", DayOfWeek.Thursday);
            CreateWeekDayColumn("Пт", DayOfWeek.Friday);
            CreateWeekDayColumn("Сб", DayOfWeek.Saturday);
            CreateWeekDayColumn("Вс", DayOfWeek.Sunday);
            ToInitState();
        }

        private void CreateWeekDayColumn(string name, DayOfWeek dayOfWeek) {
            var column = new GridViewColumn() {
                Header = name,
                DisplayMemberBinding = new Binding() {
                    Converter = new DaysOfWeekConverter(dayOfWeek)
                }
            };
            ((GridView) TimeList.View).Columns.Add(column);
        }

        private IList<DepartureTime> _values;

        public IList<DepartureTime> Value {
            get => _values;
            set {
                _values = value;
                TimeList.ItemsSource = _values;
                CollectionViewSource.GetDefaultView(TimeList.ItemsSource).Refresh();
            }
        }

        private void AddElement(object sender, RoutedEventArgs e) {
            var list = new List<DayOfWeek>();
            AddIfChecked(list, MondayCheckBox, DayOfWeek.Monday);
            AddIfChecked(list, TuesdayCheckBox, DayOfWeek.Tuesday);
            AddIfChecked(list, WednesdayCheckBox, DayOfWeek.Wednesday);
            AddIfChecked(list, ThursdayCheckBox, DayOfWeek.Thursday);
            AddIfChecked(list, FridayCheckBox, DayOfWeek.Friday);
            AddIfChecked(list, SaturdayCheckBox, DayOfWeek.Saturday);
            AddIfChecked(list, SundayCheckBox, DayOfWeek.Sunday);
            var departureTime = new DepartureTime() {
                DaysAvailable = list,
                Hour = int.Parse(HoursBox.Text),
                Minute = int.Parse(MinutesBox.Text)
            };
            _values.Add(departureTime);
            CollectionViewSource.GetDefaultView(TimeList.ItemsSource).Refresh();
            ToInitState();
        }

        private void ToInitState() {
            MondayCheckBox.IsChecked = true;
            TuesdayCheckBox.IsChecked = true;
            WednesdayCheckBox.IsChecked = true;
            ThursdayCheckBox.IsChecked = true;
            FridayCheckBox.IsChecked = true;
            SaturdayCheckBox.IsChecked = true;
            SundayCheckBox.IsChecked = true;
            HoursBox.Text = "00";
            MinutesBox.Text = "00";
        }

        private static void AddIfChecked(IList<DayOfWeek> list, CheckBox checkBox, DayOfWeek type) {
            if (checkBox.IsChecked.GetValueOrDefault(false)) {
                list.Add(type);
            }
        }

        private void RemoveElements(object sender, RoutedEventArgs e) {
            if (TimeList.SelectedItem == null) return;
            
            foreach (var selectedItem in TimeList.SelectedItems) {
                _values.Remove((DepartureTime) selectedItem);
            }
            CollectionViewSource.GetDefaultView(TimeList.ItemsSource).Refresh();
        }

        private void HourValidationTextBox(object sender, TextCompositionEventArgs e) {
            var initial = HoursBox.Text;
            var received = e.Text;
            var caretIndex = HoursBox.CaretIndex;
            
            if (received == " ") {
                e.Handled = true;
                return;
            }
            
            string result;
            if (initial.Length > 0) {
                result = initial.Substring(0, caretIndex) + received +
                         initial[caretIndex..];
            }
            else {
                result = received;
            }
            
            var parsed = int.TryParse(result, out var res);
            if (!parsed || res > 24 || res < 0) {
                e.Handled = true;
            }
        }
        
        private void MinuteValidationTextBox(object sender, TextCompositionEventArgs e) {
            var initial = MinutesBox.Text;
            var received = e.Text;
            var caretIndex = MinutesBox.CaretIndex;
            
            if (received == " ") {
                e.Handled = true;
                return;
            }
            
            string result;
            if (initial.Length > 0) {
                result = initial.Substring(0, caretIndex) + received +
                         initial[caretIndex..];
            }
            else {
                result = received;
            }
            
            var parsed = int.TryParse(result, out var res);
            if (!parsed || res > 60 || res < 0) {
                e.Handled = true;
            }
        }
    }

    internal class DaysOfWeekConverter : IValueConverter {
        private readonly DayOfWeek _dayOfWeek;

        public DaysOfWeekConverter(DayOfWeek dayOfWeek) {
            _dayOfWeek = dayOfWeek;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is DepartureTime dt) {
                return dt.DaysAvailable.Contains(_dayOfWeek) ? "+" : "-";
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    
    internal class TimeConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is DepartureTime dt) {
                return $"{dt.Hour:D2}:{dt.Minute:D2}";
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}