using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

class Program
{
    // Space: O(1) | Time: O(1)
    public class Edge : IComparable<Edge>
    {   
        public int Source { get; }
        public int Destination { get; }
        public double Weight { get; }

        public Edge(int source, int destination, double weight)
        {
            Source = source;
            Destination = destination;
            Weight = weight;
        }
        
        public int CompareTo(Edge? other)
        {
            if (other == null) return 1;
            return Weight.CompareTo(other.Weight);
        }
    }

    public class Graph
    {
        public int Vertices { get; }
        public List<Edge> Edges { get; }
        public List<List<(int Neighbor, double Weight)>> AdjacencyList { get; }

        public Graph(int vertices)
        {
            Vertices = vertices;
            Edges = new List<Edge>();
            AdjacencyList = new List<List<(int, double)>>(vertices);
            for (int i = 0; i < vertices; i++)
            {
                AdjacencyList.Add(new List<(int, double)>());
            }
        }
        
        public void AddEdge(int source, int destination, double weight)
        {
            Edges.Add(new Edge(source, destination, weight));
            AdjacencyList[source].Add((destination, weight));
        }

        // Optimized Disjoint Set (Union-Find) with Path Compression and Rank Optimization
        private class DisjointSet
        {
            private readonly int[] _parent;
            private readonly int[] _rank;

            public DisjointSet(int size) // Space: O(V) | Time: O(V)
            {
                _parent = new int[size];
                _rank = new int[size];
                for (int i = 0; i < size; i++)
                {
                    _parent[i] = i;
                }
            }

            public int Find(int element) // Time: O(α(V)) amortized due to Path Compression
            {
                if (_parent[element] != element)
                {
                    _parent[element] = Find(_parent[element]);
                }
                return _parent[element];
            }

            public bool Union(int element1, int element2) // Time: O(α(V)) amortized via Rank balancing
            {
                int root1 = Find(element1);
                int root2 = Find(element2);

                if (root1 == root2) return false;

                if (_rank[root1] < _rank[root2])
                {
                    _parent[root1] = root2;
                }
                else if (_rank[root1] > _rank[root2])
                {
                    _parent[root2] = root1;
                }
                else
                {
                    _parent[root2] = root1;
                    _rank[root1]++;
                }
                return true;
            }
        }

        // Kruskal's MST Implementation -> Space: O(V + E) | Time: O(E log E)
        public List<Edge> KruskalMST()
        {
            List<Edge> mst = new List<Edge>();
            DisjointSet disjointSet = new DisjointSet(Vertices);

            // Sorting edges by weight dominates the processing run
            Edges.Sort(); // Time: O(E log E)

            foreach (Edge edge in Edges) // Loop runs E times -> Time: O(E * α(V))
            {
                if (disjointSet.Union(edge.Source, edge.Destination))
                {
                    mst.Add(edge);
                }
            }
            return mst;
        }

        // Optimized Prim's MST using PriorityQueue -> Space: O(V + E) | Time: O(E log V)
        public (List<int> Predecessors, List<Edge> Mst) PrimMST(int root)
        {
            List<int> predecessors = Enumerable.Repeat(-2, Vertices).ToList();
            List<Edge> mst = new List<Edge>();
            bool[] visited = new bool[Vertices];
            double[] minWeightToCluster = Enumerable.Repeat(double.PositiveInfinity, Vertices).ToArray();

            // Priority Queue records: (TargetNode, ParentNode) sorted by travel weight
            var priorityQueue = new PriorityQueue<(int Node, int Parent, double Weight), double>();
            
            minWeightToCluster[root] = 0;
            priorityQueue.Enqueue((root, -1, 0.0), 0.0); // Time: O(log V)

            while (priorityQueue.Count > 0) // Loop runs up to V times -> Time: O(V log V)
            {
                var current = priorityQueue.Dequeue(); // Time: O(log V)

                if (visited[current.Node]) continue;
                visited[current.Node] = true;

                predecessors[current.Node] = current.Parent;
                if (current.Parent != -1)
                {
                    mst.Add(new Edge(current.Parent, current.Node, current.Weight));
                }

                foreach (var edge in AdjacencyList[current.Node]) // Runs E times globally across execution
                {
                    if (!visited[edge.Neighbor] && edge.Weight < minWeightToCluster[edge.Neighbor])
                    {
                        minWeightToCluster[edge.Neighbor] = edge.Weight;
                        priorityQueue.Enqueue((edge.Neighbor, current.Node, edge.Weight), edge.Weight); // Time: O(log V)
                    }
                }
            }

            return (predecessors, mst);
        }

        // Bellman-Ford Shortest Path with Negative Cycle Detection -> Space: O(V) | Time: O(V * E)
        public (List<int> Predecessors, List<double> Distances, bool HasNegativeCycle) BellmanFord(int source)
        {
            List<int> predecessors = Enumerable.Repeat(-1, Vertices).ToList();
            List<double> distances = Enumerable.Repeat(double.PositiveInfinity, Vertices).ToList();
            
            distances[source] = 0;

            // Relax edges V - 1 times sequentially
            for (int i = 0; i < Vertices - 1; i++) // Outermost loop iterates V times
            {
                foreach (Edge edge in Edges) // Inner loop processes E edges -> Time: O(V * E)
                {
                    if (!double.IsPositiveInfinity(distances[edge.Source]) && 
                        distances[edge.Source] + edge.Weight < distances[edge.Destination])
                    {
                        distances[edge.Destination] = distances[edge.Source] + edge.Weight;
                        predecessors[edge.Destination] = edge.Source;
                    }
                }
            }

            // Post-relaxation safety run for Negative-Weight Cycle Detection
            foreach (Edge edge in Edges) // Time: O(E)
            {
                if (!double.IsPositiveInfinity(distances[edge.Source]) && 
                    distances[edge.Source] + edge.Weight < distances[edge.Destination])
                {
                    return (predecessors, distances, true); // Found negative dependency loop anomalies
                }
            }

            return (predecessors, distances, false);
        }
    }

    static void Main(string[] args)
    {
        RunKruskalDemo();
        RunPrimDemo();
        RunBellmanFordDemo();
    }

    static void RunKruskalDemo()
    {
        string path = "C:\\VSCODE\\Kruskal Prim and BellmanFord algorithms\\kruskal.txt";
        int vertices;
        Graph graph;

        if (!File.Exists(path))
        {
            Console.WriteLine("[Warning] kruskal.txt not found. Using pre-established matrix fallback assets...");
            vertices = 7;
            graph = new Graph(vertices);
            int[,] matrix = {
                { 0, 2, 3, 3, 0, 0, 0 },
                { 2, 0, 4, 0, 3, 0, 0 },
                { 3, 4, 0, 5, 1, 0, 0 },
                { 3, 0, 5, 0, 0, 7, 0 },
                { 0, 3, 1, 0, 0, 8, 0 },
                { 0, 0, 0, 7, 8, 0, 9 },
                { 0, 0, 0, 0, 0, 9, 0 }
            };
            PopulateGraphFromMatrix(graph, matrix, vertices);
        }
        else
        {
            string[] lines = File.ReadAllLines(path);
            vertices = int.Parse(lines[0].Trim());
            graph = new Graph(vertices);
            PopulateGraphFromFile(graph, lines, vertices);
        }

        List<Edge> kruskalMST = graph.KruskalMST();
        Console.WriteLine("=========================================");
        Console.WriteLine("             KRUSKAL MST RESULT          ");
        Console.WriteLine("=========================================");
        if (kruskalMST.Count == vertices - 1)
        {
            Console.WriteLine("CONNECTED MST: TRUE");
            foreach (var edge in kruskalMST)
            {
                Console.WriteLine($"({edge.Source}, {edge.Destination}) -> Weight: {edge.Weight}");
            }
        }
        else
        {
            Console.WriteLine("CONNECTED MST: FALSE (Graph is Disconnected)");
        }
        Console.WriteLine();
    }

    static void RunPrimDemo()
    {
        string path = "C:\\VSCODE\\Kruskal Prim and BellmanFord algorithms\\prim.txt";
        int vertices;
        int root = 0;
        Graph graph;

        if (!File.Exists(path))
        {
            Console.WriteLine("[Warning] prim.txt not found. Using pre-established matrix fallback assets...");
            vertices = 7;
            root = 0;
            graph = new Graph(vertices);
            int[,] matrix = {
                { 0, 2, 3, 3, 0, 0, 0 },
                { 2, 0, 4, 0, 3, 0, 0 },
                { 3, 4, 0, 5, 1, 6, 0 },
                { 3, 0, 5, 0, 0, 7, 0 },
                { 0, 3, 1, 0, 0, 8, 0 },
                { 0, 0, 6, 7, 8, 0, 9 },
                { 0, 0, 0, 0, 0, 9, 0 }
            };
            PopulateGraphFromMatrix(graph, matrix, vertices);
        }
        else
        {
            string[] lines = File.ReadAllLines(path);
            vertices = int.Parse(lines[0].Trim());
            graph = new Graph(vertices);
            PopulateGraphFromFile(graph, lines, vertices);
            root = int.Parse(lines[vertices + 1].Trim());
        }

        var (predecessors, mst) = graph.PrimMST(root);
        Console.WriteLine("=========================================");
        Console.WriteLine("               PRIM MST RESULT           ");
        Console.WriteLine("=========================================");
        if (predecessors.All(x => x != -2))
        {
            Console.WriteLine("CONNECTED MST: TRUE");
            for (int i = 0; i < vertices; i++)
            {
                Console.WriteLine($"Node {i} -> Predecessor: {predecessors[i]}");
            }
            foreach (var edge in mst)
            {
                Console.WriteLine($"Path Link ({edge.Source}, {edge.Destination}) -> Weight: {edge.Weight}");
            }
        }
        else
        {
            Console.WriteLine("CONNECTED MST: FALSE (Incomplete Spanning Tree due to isolated clusters)");
        }
        Console.WriteLine();
    }

    static void RunBellmanFordDemo()
    {
        string path = "C:\\VSCODE\\Kruskal Prim and BellmanFord algorithms\\BellmanFord.txt";
        int vertices;
        int root = 0;
        Graph graph;

        if (!File.Exists(path))
        {
            Console.WriteLine("[Warning] BellmanFord.txt not found. Using pre-established matrix fallback assets...");
            vertices = 6;
            root = 0;
            graph = new Graph(vertices);
            int[,] matrix = {
                { 0, 10,  0,  0,  0, 8 },
                { 0,  0,  0,  2,  0, 0 },
                { 0,  1,  0,  0,  0, 0 },
                { 0,  0, -2,  0,  0, 0 },
                { 0, -4,  0, -1,  0, 8 },
                { 0,  0,  0,  0,  1, 0 }
            };
            PopulateGraphFromMatrix(graph, matrix, vertices);
        }
        else
        {
            string[] lines = File.ReadAllLines(path);
            vertices = int.Parse(lines[0].Trim());
            graph = new Graph(vertices);
            PopulateGraphFromFile(graph, lines, vertices);
            root = int.Parse(lines[vertices + 1].Trim());
        }

        var (predecessors, distances, hasNegativeCycle) = graph.BellmanFord(root);
        Console.WriteLine("=========================================");
        Console.WriteLine("            BELLMAN-FORD RESULT          ");
        Console.WriteLine("=========================================");
        if (hasNegativeCycle)
        {
            Console.WriteLine("CRITICAL WARNING: Graph contains a Negative-Weight Cycle! Shortest paths are undefined.");
        }
        else
        {
            Console.WriteLine("Predecessor Node Tracking Array:");
            Console.WriteLine(string.Join(" ", predecessors));
            Console.WriteLine("Calculated Shortest Weight Cost Distances:");
            Console.WriteLine(string.Join(" ", distances.Select(d => double.IsPositiveInfinity(d) ? "INF" : d.ToString())));
        }
        Console.WriteLine("=========================================");
    }

    static void PopulateGraphFromMatrix(Graph graph, int[,] matrix, int size)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (matrix[i, j] != 0)
                {
                    graph.AddEdge(i, j, matrix[i, j]);
                }
            }
        }
    }

    static void PopulateGraphFromFile(Graph graph, string[] lines, int size)
    {
        for (int i = 0; i < size; i++)
        {
            string[] values = lines[i + 1].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < size; j++)
            {
                double weight = double.Parse(values[j]);
                if (weight != 0)
                {
                    graph.AddEdge(i, j, weight);
                }
            }
        }
    }
}