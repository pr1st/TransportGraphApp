using System.Collections.Generic;
using LiteDB;

namespace TransportGraphApp.Models {
    internal interface IAttributeElement {
        IDictionary<string, BsonValue> Attributes { get; }
    }
}