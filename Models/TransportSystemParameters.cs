using System;
using System.Collections.Generic;
using System.Text;

namespace TransportGraphApp.Models {
    public class TransportSystemParameters {
        public IList<string> AvailableRoadTypes { get; set; } = new List<string>();
        public bool IsAutoCompleteCities { get; set; }
    }
}