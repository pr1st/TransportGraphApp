using System.Collections.Generic;
using System.Windows;

namespace TransportGraphApp.Actions {
    public interface IAppAction {
        public bool IsAvailable();
        public void Invoke();
    }
}
