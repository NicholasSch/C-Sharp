class Program
{
    public class Edge
    {
        public int Source { get; set; }
        public int Destination { get; set; }
        public double Weight { get; set; }

        public Edge(int source, int destination, double weight)
        {
            Source = source;
            Destination = destination;
            Weight = weight;
        }
    }

    public class Graph
    {
        public int NumVertices { get; set; }
        public List<Edge> Edges { get; set; }
        public double[] VertexWeights { get; set; }
        public List<int>[] Dependencies { get; set; }

        public Graph(int numVertices)
        {
            NumVertices = numVertices;
            Edges = new List<Edge>();
            VertexWeights = new double[numVertices];
            Dependencies = new List<int>[numVertices];
            for (int i = 0; i < numVertices; i++)
            {
                Dependencies[i] = new List<int>();
            }
        }

        public void AddEdge(int source, int destination, double weight)
        {
            Edges.Add(new Edge(source, destination, weight));
            Dependencies[source].Add(destination);
        }
    }

    // Global tracking states for the optimal closure configuration
    static double maxTotalProfit = 0;
    static List<int> optimalProjectSet = new List<int>();

    static void Main(string[] args) // Space: O(V^2) | Time: O(2^V) worst-case combinatorial search
    {
        string filePath = "C:\\VSCODE\\Project selection with dependencies\\Project.txt";
        Graph graph;

        // Fallback Strategy: Load preset data if external file is missing
        if (!File.Exists(filePath))
        {
            Console.WriteLine("[Warning] Project.txt not found. Loading pre-established matrix fallback assets...\n");
            
            int presetVertices = 6;
            graph = new Graph(presetVertices);
            graph.VertexWeights = new double[] { 25, 10, 5, -10, -20, -5 };

            int[,] presetMatrix = new int[,]
            {
                { 0, 0, 0, 1, 0, 1 },
                { 1, 0, 1, 1, 1, 0 },
                { 0, 0, 0, 0, 1, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0 }
            };

            for (int i = 0; i < presetVertices; i++)
            {
                for (int j = 0; j < presetVertices; j++)
                {
                    if (presetMatrix[i, j] != 0)
                    {
                        graph.AddEdge(i, j, presetMatrix[i, j]);
                    }
                }
            }
        }
        else
        {
            // Parse dataset from external file source
            try
            {
                string[] fileLines = File.ReadAllLines(filePath);
                int vertices = int.Parse(fileLines[0].Trim());
                graph = new Graph(vertices);

                for (int i = 1; i <= vertices; i++)
                {
                    string[] rowElements = fileLines[i].Trim().Split(' ');
                    for (int j = 0; j < vertices; j++)
                    {
                        if (int.Parse(rowElements[j]) != 0)
                        {
                            graph.AddEdge(i - 1, j, double.Parse(rowElements[j]));
                        }
                    }
                }

                string[] weightElements = fileLines[vertices + 1].Trim().Split(' ');
                for (int j = 0; j < vertices; j++)
                {
                    graph.VertexWeights[j] = double.Parse(weightElements[j]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical Error parsing file data: {ex.Message}");
                return;
            }
        }

        List<List<int>> closures = new List<List<int>>();
        bool dynamicGraphValid = true;

        // Precompute the transitive closure for each project using Breadth-First Search
        for (int i = 0; i < graph.NumVertices; i++)
        {
            var (allDependencies, isCycleFree) = ComputeTransitiveClosureBFS(i, graph);
            closures.Add(allDependencies);
            
            if (!isCycleFree)
            {
                dynamicGraphValid = false;
                break;
            }
        }

        if (dynamicGraphValid)
        {
            Console.WriteLine("--- Executing Project Dependency Closure Search ---");
            bool[] currentSelection = new bool[graph.NumVertices];
            SolveProjectSelectionMaxProfit(graph, closures, 0, currentSelection);

            if (maxTotalProfit > 0)
            {
                Console.WriteLine("\n=========================================");
                Console.WriteLine("          FINAL OPTIMAL SOLUTION        ");
                Console.WriteLine("=========================================");
                Console.Write("Selected Projects (1-indexed): [");
                Console.Write(string.Join(",", optimalProjectSet.Select(p => p + 1)));
                Console.WriteLine("]");
                Console.WriteLine($"Final maximized profit sum: {maxTotalProfit}");
            }
            else
            {
                Console.WriteLine("\nOptimization Complete: No project selections yield positive net profit.");
            }
        }
        else
        {
            Console.WriteLine("Invalid graph configuration: Dependency cycle loop detected.");
        }
    }

    // Space: O(V) | Time: O(V + E) per project node
    static (List<int> Dependencies, bool IsCycleFree) ComputeTransitiveClosureBFS(int rootVertex, Graph graph)
    {
        List<int> visitedDependencies = new List<int>();
        Queue<int> queue = new Queue<int>();
        queue.Enqueue(rootVertex);

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();
            if (!visitedDependencies.Contains(current))
            {
                visitedDependencies.Add(current);
                foreach (int neighbor in graph.Dependencies[current])
                {   
                    // If a dependency points back to the starting node, a cycle exists
                    if (neighbor == rootVertex)
                    {
                        return (visitedDependencies, false);
                    }
                    queue.Enqueue(neighbor);
                }
            }
        }
        return (visitedDependencies, true);
    }

    // Exact Backtracking Closure Selector Strategy
    // Space: O(V) stack frames | Time: O(2^V) worst-case combo evaluations
    static void SolveProjectSelectionMaxProfit(Graph graph, List<List<int>> closures, int projectIndex, bool[] currentSelection)
    {
        // Base Condition: All candidate projects branched
        if (projectIndex == graph.NumVertices)
        {
            double currentProfitSum = 0;
            List<int> selectedIndices = new List<int>();

            for (int i = 0; i < graph.NumVertices; i++)
            {
                if (currentSelection[i])
                {
                    currentProfitSum += graph.VertexWeights[i];
                    selectedIndices.Add(i);
                }
            }

            if (currentProfitSum > maxTotalProfit)
            {
                maxTotalProfit = currentProfitSum;
                optimalProjectSet = new List<int>(selectedIndices);
            }
            return;
        }

        // Branch 1: Attempt to select the project (along with all its mandatory dependencies)
        bool[] backupState = (bool[])currentSelection.Clone();
        bool validSelection = true;

        // Force-activate all items requested by the transitive dependency closure
        foreach (int dependency in closures[projectIndex])
        {
            currentSelection[dependency] = true;
        }

        // Cross-verify that adding dependencies didn't break closure requirements for already selected elements
        for (int i = 0; i < graph.NumVertices; i++)
        {
            if (currentSelection[i])
            {
                foreach (int dep in closures[i])
                {
                    if (!currentSelection[dep])
                    {
                        validSelection = false;
                        break;
                    }
                }
            }
        }

        if (validSelection)
        {
            SolveProjectSelectionMaxProfit(graph, closures, projectIndex + 1, currentSelection);
        }

        // Reset state for backtracking branch
        Array.Copy(backupState, currentSelection, graph.NumVertices);

        // Branch 2: Exclude this project from being explicitly initialized at this branch layer
        SolveProjectSelectionMaxProfit(graph, closures, projectIndex + 1, currentSelection);
    }
}