using System;
using System.Collections.Generic;
using System.Text;

namespace TransportGraphApp.Actions.HelpActions
{
    public class OverviewAction : IAppAction {
        public bool IsAvailable() => true;

        public void Invoke() {
            throw new NotImplementedException();
        }
    }
}
