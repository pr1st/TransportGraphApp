using System;
using System.Collections.Generic;
using System.Text;

namespace TransportGraphApp.Models {
    public class SystemParameters {
        public IList<string> AvailableRoadTypes { get; set; } = new List<string>();
        public bool IsAutoCompleteCities { get; set; }
    }
}