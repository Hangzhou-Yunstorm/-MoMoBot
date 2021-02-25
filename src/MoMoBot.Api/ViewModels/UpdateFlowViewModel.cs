using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Api.ViewModels
{
    public class UpdateFlowViewModel
    {
        public long FlowId { get; set; }
        public List<Edge> Edges { get; set; }
        public List<Node> Nodes { get; set; }
    }

    public class Edge
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
    }
    public class Node
    {
        public bool? isEnd { get; set; } = false;
        public string Id { get; set; }
        public string Label { get; set; }
        public string Question { get; set; }
        public string Shape { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public string Func { get; set; }
        public string Key { get; set; }
    }
}
