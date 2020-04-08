using System;
using System.Windows.Controls;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.CustomComponents {
    public partial class AlgorithmConfigControl : UserControl {
        public AlgorithmConfig AlgorithmConfig => new AlgorithmConfig() {
            AlgorithmType = AlgorithmType.Cost,
            StartHour = 123,
            StartMinute = 123,
            StartWeekDay = DayOfWeek.Friday
        };

        public AlgorithmConfigControl() {
            InitializeComponent();
        }
    }
}