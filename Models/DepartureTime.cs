using System;
using System.Collections;
using System.Collections.Generic;

namespace TransportGraphApp.Models {
    public class EveryDayDepartureTime {
        public DateTime Time { get; set; }
    }
    
    public class WeekDaysDepartureTime {
        public IList<DayOfWeek> DaysAvailable { get; set; }
        public DateTime Time { get; set; }
    }
    
    public class SpecifiedDaysDepartureTime {
        public DateTime FromDay { get; set; }
        public DateTime ToDay { get; set; }
        public DateTime Time { get; set; }
    }
}