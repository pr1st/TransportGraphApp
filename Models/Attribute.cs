
namespace TransportGraphApp.Models {
    public class Attribute {
        public string Name { get; set; }

        public AttributeType Type { get; set; }

        public object Value { get; set; }

        public override string ToString() {
            return $"{Name}: {Type} ({Value})";
        }
    }
}