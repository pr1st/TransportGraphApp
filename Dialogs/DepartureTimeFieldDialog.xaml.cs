using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TransportGraphApp.Models;

namespace TransportGraphApp.Dialogs {
    public partial class DepartureTimeFieldDialog : Window {
        public DepartureTimeFieldDialog() {
            InitializeComponent();
            Owner = App.Window;
            Icon = AppResources.GetAppIcon;
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
            HoursBox.Text = "0";
            MinutesBox.Text = "0";
        }

        public DepartureTime DepartureTime() {
            var list = new List<DayOfWeek>();
            AddIfChecked(list, MondayCheckBox, DayOfWeek.Monday);
            AddIfChecked(list, TuesdayCheckBox, DayOfWeek.Tuesday);
            AddIfChecked(list, WednesdayCheckBox, DayOfWeek.Wednesday);
            AddIfChecked(list, ThursdayCheckBox, DayOfWeek.Thursday);
            AddIfChecked(list, FridayCheckBox, DayOfWeek.Friday);
            AddIfChecked(list, SaturdayCheckBox, DayOfWeek.Saturday);
            AddIfChecked(list, SundayCheckBox, DayOfWeek.Sunday);
            return new DepartureTime() {
                DaysAvailable = list,
                Hour = int.Parse(HoursBox.Text),
                Minute = int.Parse(MinutesBox.Text)
            };
        }

        private static void AddIfChecked(IList<DayOfWeek> list, CheckBox checkBox, DayOfWeek type) {
            if (checkBox.IsChecked.GetValueOrDefault(false)) {
                list.Add(type);
            }
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
            if (!parsed || res > 23 || res < 0) {
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
            if (!parsed || res > 59 || res < 0) {
                e.Handled = true;
            }
        }

        private void OkClick(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }
    }
}