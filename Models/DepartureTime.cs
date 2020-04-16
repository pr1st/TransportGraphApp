using System;
using System.Collections.Generic;

namespace TransportGraphApp.Models {
    public class DepartureTime {
        public IList<DayOfWeek> DaysAvailable { get; set; } = new List<DayOfWeek>();

        public int Hour { get; set; }

        public int Minute { get; set; }
    }
}