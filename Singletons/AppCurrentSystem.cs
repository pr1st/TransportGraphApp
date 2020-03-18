using TransportGraphApp.Models;

namespace TransportGraphApp.Singletons {
    internal class AppCurrentSystem {
        private static AppCurrentSystem _instance;
        private TransportSystem _transportSystem;

        public static AppCurrentSystem Instance => _instance ??= new AppCurrentSystem();


        private AppCurrentSystem() { }

        public void Select(TransportSystem ts) {
            _transportSystem = ts;
            App.ChangeAppState(AppStates.TransportSystemSelected, ts != null);
        }

        public TransportSystem Get() => _transportSystem;
    }
}