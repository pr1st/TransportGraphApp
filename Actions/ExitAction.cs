﻿namespace TransportGraphApp.Actions {
    public static class ExitAction  {
        public static void Invoke() {
            App.DataBase.Close();
        }
    }
}