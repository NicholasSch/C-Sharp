using System;
using System.Collections.Generic;

class Program
{
    public class Vertex
    {
        public string Name { get; set; }
        public bool IsExplored { get; set; }
        public List<Vertex> VertexChildren { get; set; }
        public Vertex? Predecessor { get; set; }
        public int Distance { get; set; }

        public Vertex(string name)
        {
            Name = name;
            IsExplored = false;
            VertexChildren = new List<Vertex>();
            Predecessor = null;
            Distance = 0;
        }

        // Space: O(V) | Time: O(V + E)
        public void RunBreadthFirstSearch()
        {   
            Queue<Vertex> queue = new Queue<Vertex>();
            this.IsExplored = true;
            queue.Enqueue(this);   

            while (queue.Count > 0)
            {
                Vertex currentVertex = queue.Dequeue();
                Console.WriteLine($"Vertex: {currentVertex.Name}");
                
                if (currentVertex.Predecessor != null)
                {
                    Console.WriteLine($"Predecessor: {currentVertex.Predecessor.Name}");
                }
                else
                {
                    Console.WriteLine("Predecessor: None");
                }  
                Console.WriteLine($"Distance from start: {currentVertex.Distance}\n");
            
                foreach (Vertex child in currentVertex.VertexChildren)
                {   
                    if (!child.IsExplored)
                    {
                        child.IsExplored = true;
                        child.Predecessor = currentVertex;
                        child.Distance = currentVertex.Distance + 1;
                        queue.Enqueue(child);
                    }
                }
            }
        }

        // Space: O(V) | Time: O(V + E)
        public void RunDepthFirstSearch()
        {
            Stack<Vertex> stack = new Stack<Vertex>();
            this.IsExplored = true;
            stack.Push(this);

            while (stack.Count > 0)
            {
                Vertex currentVertex = stack.Pop();
                Console.WriteLine($"Vertex: {currentVertex.Name}");
                
                if (currentVertex.Predecessor != null)
                {
                    Console.WriteLine($"Predecessor: {currentVertex.Predecessor.Name}");
                }
                else
                {
                    Console.WriteLine("Predecessor: None");
                }  
                Console.WriteLine($"Distance from start: {currentVertex.Distance}\n");

                foreach (Vertex child in currentVertex.VertexChildren) 
                {
                    if (!child.IsExplored)
                    {
                        child.IsExplored = true;
                        child.Predecessor = currentVertex;
                        child.Distance = currentVertex.Distance + 1;
                        stack.Push(child);
                    }
                }
            }
        }
    }

    static void Main(string[] args) // Space: O(V^2) | Time: O(V^2) matrix compilation
    {
        // Hardcoded preset adjacency matrix (6x6 representing nodes A through F)
        int[,] adjacencyMatrix = new int[,]
        {
            { 0, 1, 1, 0, 0, 0 }, // Connections for A -> B, C
            { 1, 0, 0, 1, 1, 0 }, // Connections for B -> A, D, E
            { 1, 0, 0, 0, 0, 1 }, // Connections for C -> A, F
            { 0, 1, 0, 0, 0, 0 }, // Connections for D -> B
            { 0, 1, 0, 0, 0, 1 }, // Connections for E -> B, F
            { 0, 0, 1, 0, 1, 0 }  // Connections for F -> C, E
        };

        int size = adjacencyMatrix.GetLength(0);

        // Hydrate graph vertices mapping
        Dictionary<string, Vertex> vertices = new Dictionary<string, Vertex>();
        int asciiStartValue = (int)'A';
        
        for (int i = 0; i < size; i++)
        {
            char vertexName = (char)(asciiStartValue + i);
            Vertex vertex = new Vertex(vertexName.ToString());
            vertices[vertex.Name] = vertex;
        }

        // Establish structural child edge links
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {   
                if (adjacencyMatrix[i, j] != 0)
                {
                    char parentName = (char)(asciiStartValue + i);
                    char childName = (char)(asciiStartValue + j);
                    vertices[parentName.ToString()].VertexChildren.Add(vertices[childName.ToString()]);
                }
            }
        }

        int executionOption = 1;
        while (executionOption != 0)
        {
            ResetGraphState(vertices);

            Console.WriteLine("Choose initial vertex from listed options:");
            foreach (Vertex vertex in vertices.Values)
            {
                Console.WriteLine(vertex.Name);
            }
            
            string targetVertexInput = Console.ReadLine() ?? "";
            
            if (!vertices.ContainsKey(targetVertexInput))
            {
                Console.WriteLine("Invalid vertex selection.");
                continue;
            }

            Console.WriteLine("\n--- Starting Breadth-First Search (BFS) ---");
            vertices[targetVertexInput].RunBreadthFirstSearch();
            
            ResetGraphState(vertices);

            Console.WriteLine("--- Starting Depth-First Search (DFS) ---");
            vertices[targetVertexInput].RunDepthFirstSearch();
            
            Console.WriteLine("Type 0 to stop, or any other number to run another test visualization:");
            string input = Console.ReadLine() ?? "0";
            int.TryParse(input, out executionOption);
        }
    }

    private static void ResetGraphState(Dictionary<string, Vertex> vertices)
    {
        foreach (Vertex vertex in vertices.Values)
        {
            vertex.IsExplored = false;
            vertex.Predecessor = null;
            vertex.Distance = 0;
        }
    }
}