using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using TransportGraphApp.Models;

namespace TransportGraphApp.Actions {
    internal static class UpdateNodeAction {
        public static void Invoke(Node n) {
            MessageBox.Show("Update node");
        }
    }
}