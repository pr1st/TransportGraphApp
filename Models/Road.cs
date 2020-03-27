using System.Collections;
using System.Collections.Generic;
using LiteDB;

namespace TransportGraphApp.Models {
    public class Road : IAppModel {
        public ObjectId Id { get; set; }
        
        public ObjectId FromCityId { get; set; }
        
        public ObjectId TpCityId { get; set; }

        public double Length { get; set; }
        
        public double Cost { get; set; }

        public double Time { get; set; }
        
        public IList<string> RoadTypes { get; set; } = new List<string>();
        
        public IList<EveryDayDepartureTime> EveryDayDepartureTime { get; set; } = new List<EveryDayDepartureTime>();
        public IList<WeekDaysDepartureTime> WeekDaysDepartureTime { get; set; } = new List<WeekDaysDepartureTime>();
        public IList<SpecifiedDaysDepartureTime> SpecifiedDaysDepartureTime { get; set; } = new List<SpecifiedDaysDepartureTime>();
        public ObjectId TransportSystemId { get; set; }
    }
}