using System;

namespace TransportGraphApp.Graph {
    public class Time : IEquatable<Time> {
        public int Value { get; set; }

        public Time() {
            Value = 0;
        }

        public Time(int t) {
            Value = t;
        }

        private const int MaxValue = 60 * 24 * 7;

        public static Time operator +(Time t1, Time t2) {
            return new Time((t1.Value + t2.Value) % MaxValue);
        }

        public static Time operator -(Time t1, Time t2) {
            return new Time((t1.Value - t2.Value + MaxValue) % MaxValue);
        }

        public static bool operator <(Time t1, Time t2) {
            return t1.Value < t2.Value;
        }

        public static bool operator >(Time t1, Time t2) {
            return t1.Value > t2.Value;
        }

        public bool Equals(Time other) {
            return other != null && Value == other.Value;
        }

        public override string ToString() {
            return $"Time({Value})";
        }

    }
}