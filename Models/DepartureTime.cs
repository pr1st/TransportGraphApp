using System;
using System.Collections.Generic;

namespace TransportGraphApp.Models {
    public class DepartureTime {
        public IList<DayOfWeek> DaysAvailable { get; set; } = new List<DayOfWeek>();

        public int Hour { get; set; }

        public int Minute { get; set; }

        public static IDictionary<string, Func<DepartureTime, object>> PropertyMatcher() {
            return new Dictionary<string, Func<DepartureTime, object>>() {
                {"Время отправления", dt => $"{dt.Hour:D2}:{dt.Minute:D2}"},
                {"Пн", dt => dt.DaysAvailable.Contains(DayOfWeek.Monday) ? "+" : "-"},
                {"Вт", dt => dt.DaysAvailable.Contains(DayOfWeek.Tuesday) ? "+" : "-"},
                {"Ср", dt => dt.DaysAvailable.Contains(DayOfWeek.Wednesday) ? "+" : "-"},
                {"Чт", dt => dt.DaysAvailable.Contains(DayOfWeek.Thursday) ? "+" : "-"},
                {"Пт", dt => dt.DaysAvailable.Contains(DayOfWeek.Friday) ? "+" : "-"},
                {"Сб", dt => dt.DaysAvailable.Contains(DayOfWeek.Saturday) ? "+" : "-"},
                {"Вс", dt => dt.DaysAvailable.Contains(DayOfWeek.Sunday) ? "+" : "-"},
            };
        }
    }
}