namespace GraphNetworkSimulator.Models
{
    public class Node
    {
        public string Id { get; }
        public List<Tuple<string, Node>> Cache { get; }
        public List<Tuple<Node, Node>> Edges { get; }
        public List<Tuple<string, Node>> Resources { get; }
        
        public object CacheLock { get; } = new object();

        public Node(string id)
        {
            Id = id;
            Cache = new List<Tuple<string, Node>>();
            Edges = new List<Tuple<Node, Node>>();
            Resources = new List<Tuple<string, Node>>();
        }
    }
}