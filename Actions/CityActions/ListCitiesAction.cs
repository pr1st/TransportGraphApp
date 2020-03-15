﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TransportGraphApp.Actions.CityActions {
    public class ListCitiesAction : IAppAction {
        private ListCitiesAction() {
        }

        public bool IsAvailable() {
            return App.CurrentState == AppState.GraphSelected;
        }

        public void Invoke() {
            throw new NotImplementedException();
        }
    }
}