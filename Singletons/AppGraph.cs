using TransportGraphApp.Models;

namespace TransportGraphApp.Singletons {
    internal class AppGraph {
        private static AppGraph _instance;

        public static AppGraph Instance => _instance ??= new AppGraph();


        private AppGraph() { }

        public Graph Graph { get; set; }
    }
}