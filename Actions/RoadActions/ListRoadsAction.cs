namespace TransportGraphApp.Actions.RoadActions {
    public class ListRoadsAction : IAppAction {
        private ListRoadsAction() {
        }

        public bool IsAvailable() {
            return App.CurrentStates[AppStates.TransportSystemSelected];
        }

        public void Invoke() {
            throw new System.NotImplementedException();
        }
    }
}