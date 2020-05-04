﻿using System;

 namespace TransportGraphApp.Graph {
    public class Weight : IEquatable<Weight>{
        public double Value { get; }

        public Weight() {
            Value = double.MaxValue;
        }

        public Weight(double w) {
            Value = w;
        }
        
        public static Weight operator+(Weight w1, Weight w2) {
            return new Weight(w1.Value + w2.Value);
        }
        
        public static Weight operator-(Weight w1, Weight w2) {
            return new Weight(w1.Value - w2.Value);
        }
        
        public static bool operator<(Weight w1, Weight w2) {
            return w1.Value < w2.Value;
        }
        
        public static bool operator>(Weight w1, Weight w2) {
            return w1.Value > w2.Value;
        }
        
        public bool Equals(Weight other) {
            return other != null && Math.Abs(Value - other.Value) < 0.01;
        }
    }
}