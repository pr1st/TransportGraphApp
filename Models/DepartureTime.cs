using System;
using System.Collections;
using System.Collections.Generic;

namespace TransportGraphApp.Models {
    public class DepartureTime {
        public IList<DayOfWeek> DaysAvailable { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
    }
}