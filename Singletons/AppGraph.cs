using TransportGraphApp.Models;

namespace TransportGraphApp.Singletons {
    internal class AppGraph {
        private static AppGraph _instance;
        private TransportSystem _transportSystem;

        public static AppGraph Instance => _instance ??= new AppGraph();


        private AppGraph() { }


        public TransportSystem TransportSystem {
            get => _transportSystem;
            set {
                _transportSystem = value;
                AppWindow.Instance.DrawGraph();
            }
        }
    }
}