using GraphNetworkSimulator.Models;

namespace GraphNetworkSimulator.Core
{
    // Topology parsing engine responsible for structural layout parsing and graph validation
    public class GraphBuilder
    {
        public Dictionary<string, Node> Nodes { get; } = new Dictionary<string, Node>();
        public List<Tuple<Node, Node>> AllEdges { get; } = new List<Tuple<Node, Node>>();

        // Time Complexity: O(L + V + E) where L represents lines parsed from flat-file descriptors
        // Space Complexity: O(V + E) allocation overhead mapping nodes and link structures inside memory collections
        public Graph BuildGraph(string[] lines)
        {
            int numNodes = 0, minNeighbors = 0, maxNeighbors = 0;
            bool parsingResources = false, parsingEdges = false;

            foreach (var line in lines.Select(l => l.Trim()))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line.StartsWith("num_nodes:"))
                    numNodes = int.Parse(line.Split(":")[1].Trim());
                else if (line.StartsWith("min_neighbors:"))
                    minNeighbors = int.Parse(line.Split(":")[1].Trim());
                else if (line.StartsWith("max_neighbors:"))
                    maxNeighbors = int.Parse(line.Split(":")[1].Trim());
                else if (line.StartsWith("resources:"))
                {
                    parsingResources = true;
                    parsingEdges = false;
                }
                else if (line.StartsWith("edges:"))
                {
                    parsingEdges = true;
                    parsingResources = false;
                }
                else if (parsingResources && line.Contains(":"))
                {
                    var parts = line.Split(":");
                    var nodeId = parts[0].Trim();
                    var resourceList = parts[1].Split(",").Select(r => r.Trim());

                    if (!Nodes.ContainsKey(nodeId))
                        Nodes[nodeId] = new Node(nodeId);

                    foreach (var resource in resourceList)
                        Nodes[nodeId].Resources.Add(new Tuple<string, Node>(resource, Nodes[nodeId]));
                }
                else if (parsingEdges && line.Contains(","))
                {
                    var parts = line.Split(",");
                    var fromNode = parts[0].Trim();
                    var toNode = parts[1].Trim();

                    if (fromNode == toNode)
                        throw new InvalidOperationException($"Self-loops are structurally forbidden: {fromNode}");

                    if (!Nodes.ContainsKey(fromNode)) Nodes[fromNode] = new Node(fromNode);
                    if (!Nodes.ContainsKey(toNode)) Nodes[toNode] = new Node(toNode);

                    var nodeA = Nodes[fromNode];
                    var nodeB = Nodes[toNode];

                    nodeA.Edges.Add(new Tuple<Node, Node>(nodeA, nodeB));
                    nodeB.Edges.Add(new Tuple<Node, Node>(nodeB, nodeA));
                    AllEdges.Add(new Tuple<Node, Node>(nodeA, nodeB));
                }
            }

            // Connectivity Verification Check (BFS traversal approach)
            if (!IsGraphConnected(Nodes.Values.ToList(), numNodes))
                throw new InvalidOperationException("Graph validation failed: Topology is structurally disconnected.");

            foreach (var node in Nodes.Values)
            {
                if (node.Resources.Count == 0)
                    throw new InvalidOperationException($"Graph validation failed: Node {node.Id} contains zero resources.");

                if (node.Edges.Count < minNeighbors || node.Edges.Count > maxNeighbors)
                    throw new InvalidOperationException($"Graph validation failed: Node {node.Id} violates degree boundary limits.");
            }

            return new Graph
            {
                NumNodes = numNodes,
                MinNeighbors = minNeighbors,
                MaxNeighbors = maxNeighbors,
                Resources = Nodes.SelectMany(n => n.Value.Resources).ToArray(),
                Edges = AllEdges.ToArray()
            };
        }

        private bool IsGraphConnected(List<Node> nodes, int expectedCount)
        {
            if (nodes.Count == 0) return true;

            var visited = new HashSet<Node>();
            var queue = new Queue<Node>();
            
            queue.Enqueue(nodes[0]);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (visited.Contains(current)) continue;
                visited.Add(current);

                foreach (var edge in current.Edges)
                {
                    var neighbor = edge.Item2;
                    if (!visited.Contains(neighbor))
                        queue.Enqueue(neighbor);
                }
            }

            return visited.Count == nodes.Count && nodes.Count == expectedCount;
        }
    }
}