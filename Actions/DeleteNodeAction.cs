using System;
using System.Collections.Generic;
using System.Text;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions
{
    internal static class DeleteNodeAction
    {
        public static void Invoke(Node selectedNode) {
            AppDataBase.Instance.GetCollection<Node>().DeleteMany(n => n.Id == selectedNode.Id);
            AppDataBase.Instance.GetCollection<Edge>().DeleteMany(e => e.FromNodeId == selectedNode.Id || e.ToNodeId == selectedNode.Id);
        }
    }
}
