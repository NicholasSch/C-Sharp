class Program
{
    public class Edge
    {
        public int LeftVertex { get; set; }
        public int RightVertex { get; set; }
        public double Weight { get; set; }

        public Edge(int leftVertex, int rightVertex, double weight)
        {
            LeftVertex = leftVertex;
            RightVertex = rightVertex;
            Weight = weight;
        }
    }

    public class Graph
    {
        public int NumVertices { get; set; }
        public List<Edge> Edges { get; set; }
        public bool[,] AdjacencyMatrix { get; set; }

        public Graph(int numVertices)
        {
            NumVertices = numVertices;
            Edges = new List<Edge>();
            AdjacencyMatrix = new bool[numVertices, numVertices];
        }

        public void AddEdge(int leftVertex, int rightVertex, double weight)
        {
            Edges.Add(new Edge(leftVertex, rightVertex, weight));
            AdjacencyMatrix[leftVertex, rightVertex] = true;
            AdjacencyMatrix[rightVertex, leftVertex] = true;
        }
    }

    // Global optimization state parameters
    static double maxIndependentWeight = 0;
    static bool[] bestIndependentSet = Array.Empty<bool>();

    static void Main(string[] args) // Space: O(V^2) | Time: O(2^V) worst-case
    {
        string filePath = "C:\\VSCODE\\Maximum Weight Independent Set Solver\\entrada.txt";
        Graph graph;
        double[] vertexWeights;

        // Fallback Strategy: Load preset values if external text file is missing
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"[Warning] Input file not found at '{filePath}'. Loading pre-established default dataset...\n");

            int presetVertices = 7;
            graph = new Graph(presetVertices);
            vertexWeights = new double[] { 5, 2, 4, 3, 6, 2, 1 };

            int[,] presetMatrix = new int[,]
            {
                { 0, 0, 1, 0, 1, 0, 0 },
                { 0, 0, 1, 1, 1, 1, 1 },
                { 1, 1, 0, 0, 1, 0, 1 },
                { 0, 1, 0, 0, 0, 1, 0 },
                { 1, 1, 1, 0, 0, 0, 1 },
                { 0, 1, 0, 1, 0, 0, 1 },
                { 0, 1, 1, 0, 1, 1, 0 }
            };

            for (int i = 0; i < presetVertices; i++)
            {
                for (int j = 0; j < presetVertices; j++)
                {
                    if (presetMatrix[i, j] != 0)
                    {
                        graph.AddEdge(i, j, 1.0);
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
                int parsedVertices = int.Parse(fileLines[0].Trim());
                graph = new Graph(parsedVertices);

                // Fixed loop boundaries (1 to parsedVertices inclusively) to map the entire matrix
                for (int i = 1; i <= parsedVertices; i++)
                {
                    string[] rowElements = fileLines[i].Trim().Split(' ');
                    for (int j = 0; j < parsedVertices; j++)
                    {
                        if (double.Parse(rowElements[j]) != 0)
                        {
                            graph.AddEdge(i - 1, j, double.Parse(rowElements[j]));
                        }
                    }
                }

                string[] weightElements = fileLines[parsedVertices + 1].Trim().Split(' ');
                vertexWeights = new double[parsedVertices];
                for (int i = 0; i < parsedVertices; i++)
                {
                    vertexWeights[i] = double.Parse(weightElements[i]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical Error parsing file data: {ex.Message}");
                return;
            }
        }

        bestIndependentSet = new bool[graph.NumVertices];
        bool[] currentSetAllocation = new bool[graph.NumVertices];

        // Calculate maximum sum suffix array for branch pruning optimizations
        double[] suffixMaxWeights = new double[graph.NumVertices + 1];
        for (int i = graph.NumVertices - 1; i >= 0; i--)
        {
            suffixMaxWeights[i] = suffixMaxWeights[i + 1] + vertexWeights[i];
        }

        Console.WriteLine("--- Executing Maximum Weight Independent Set Search ---");
        FindMaximumWeightIndependentSet(graph, vertexWeights, 0, 0, currentSetAllocation, suffixMaxWeights);

        // Print final optimization result output metrics
        Console.WriteLine("\n=========================================");
        Console.WriteLine("          FINAL OPTIMAL SOLUTION        ");
        Console.WriteLine("=========================================");
        for (int i = 0; i < graph.NumVertices; i++)
        {
            Console.WriteLine($"Variable Vertex {i + 1} = {(bestIndependentSet[i] ? "1" : "0")}");
        }
        Console.WriteLine($"Biggest independent weighted set: {maxIndependentWeight}");
    }

    // Space: O(V) stack allocation | Time: O(2^V) worst-case combinatorial exploration
    static void FindMaximumWeightIndependentSet(
        Graph graph, 
        double[] weights, 
        int currentVertexIndex, 
        double accumulatedWeight, 
        bool[] currentSet, 
        double[] suffixMaxWeights)
    {
        // Base Condition: All vertex combinations evaluated
        if (currentVertexIndex == graph.NumVertices)
        {
            if (accumulatedWeight > maxIndependentWeight)
            {
                maxIndependentWeight = accumulatedWeight;
                Array.Copy(currentSet, bestIndependentSet, graph.NumVertices);
            }
            return;
        }

        // Bounding Prune Step: If current path cannot beat the best score, prune branch
        if (accumulatedWeight + suffixMaxWeights[currentVertexIndex] <= maxIndependentWeight)
        {
            return;
        }

        // Branch 1: Try to include the current vertex (if valid)
        bool canInclude = true;
        for (int previousVertex = 0; previousVertex < currentVertexIndex; previousVertex++)
        {
            if (currentSet[previousVertex] && graph.AdjacencyMatrix[currentVertexIndex, previousVertex])
            {
                canInclude = false;
                break;
            }
        }

        if (canInclude)
        {
            currentSet[currentVertexIndex] = true;
            FindMaximumWeightIndependentSet(graph, weights, currentVertexIndex + 1, accumulatedWeight + weights[currentVertexIndex], currentSet, suffixMaxWeights);
            currentSet[currentVertexIndex] = false; // Backtrack step
        }

        // Branch 2: Exclude the current vertex and move forward
        FindMaximumWeightIndependentSet(graph, weights, currentVertexIndex + 1, accumulatedWeight, currentSet, suffixMaxWeights);
    }
}