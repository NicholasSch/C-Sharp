namespace GraphNetworkSimulator.Models
{
    public class Graph
    {
        public int NumNodes { get; set; }
        public int MinNeighbors { get; set; }
        public int MaxNeighbors { get; set; }
        public Tuple<string, Node>[] Resources { get; set; }
        public Tuple<Node, Node>[] Edges { get; set; }

        public Graph()
        {
            Resources = Array.Empty<Tuple<string, Node>>();
            Edges = Array.Empty<Tuple<Node, Node>>();
        }
    }
}