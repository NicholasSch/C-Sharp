using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    public class Vertex
    {
        public string Name { get; set; }
        public List<Edge> Edges { get; set; }
        public int ShortestTimeTo { get; set; }
        public bool IsExplored { get; set; }
        public Vertex? PreviousVertex { get; set; }

        public Vertex(string name)
        {
            Name = name;
            Edges = new List<Edge>();
            ShortestTimeTo = int.MaxValue;
            IsExplored = false;
            PreviousVertex = null;
        }
    }

    public class Edge
    {
        public Vertex LeftVertex { get; set; }
        public Vertex RightVertex { get; set; }
        public int EdgeTime { get; set; }

        public Edge(Vertex leftVertex, Vertex rightVertex, int edgeTime)
        {
            LeftVertex = leftVertex;
            RightVertex = rightVertex;
            EdgeTime = edgeTime;
        }
    }

    public class Board
    {
        public List<Vertex> AllVertices { get; set; }
        public List<Vertex> PolicePositions { get; set; }
        public Vertex ThiefPosition { get; set; }

        public Board(List<Vertex> allVertices, List<Vertex> startPolicePositions, Vertex startThiefPosition)
        {
            AllVertices = allVertices;
            PolicePositions = startPolicePositions;
            ThiefPosition = startThiefPosition;
        }

        private string GetOccupantMarker(Vertex vertex)
        {
            List<string> occupants = new List<string>();
            
            if (ThiefPosition == vertex) occupants.Add("🕵️  Thief");
            if (PolicePositions[0] == vertex) occupants.Add("👮 P1");
            if (PolicePositions[1] == vertex) occupants.Add("👮 P2");

            if (occupants.Count > 0)
                return "[" + string.Join(" + ", occupants) + "]";
            
            return "[ Empty ]";
        }

        public void DrawBoard()
        {
            Console.Clear();
            Console.WriteLine("=========================================================================");
            Console.WriteLine("             TACTICAL GRAPH GRID: POLICE vs THIEF DASHBOARD              ");
            Console.WriteLine("=========================================================================");
            Console.WriteLine();
            
            // This ASCII map now accurately reflects every single link from your code
            Console.WriteLine("        [I]=============[A]=============[E]                              ");
            Console.WriteLine("         |               |               |                               ");
            Console.WriteLine("         |               |               |                               ");
            Console.WriteLine("        [G]             [J]             [C]                              ");
            Console.WriteLine("         |               |             / |                               ");
            Console.WriteLine("         |               |            /  |                               ");
            Console.WriteLine("        [B]=============[F]==========/   |   <-- (Direct Line to C)      ");
            Console.WriteLine("         |               |               |                               ");
            Console.WriteLine("         |               |               |                               ");
            Console.WriteLine("        [D]=============[H]=============/    <-- (Direct Line to B)      ");
            Console.WriteLine();
            Console.WriteLine("=========================================================================");
            Console.WriteLine("                           LIVE LOCATION TRACKER                         ");
            Console.WriteLine("=========================================================================");

            // Render the clean status layout grid
            for (int i = 0; i < AllVertices.Count; i += 2)
            {
                Vertex v1 = AllVertices[i];
                Vertex v2 = AllVertices[i + 1];

                string col1 = $"Node {v1.Name}: {GetOccupantMarker(v1)}".PadRight(35);
                string col2 = $"Node {v2.Name}: {GetOccupantMarker(v2)}";

                Console.WriteLine($"{col1} | {col2}");
            }
            Console.WriteLine("=========================================================================");
        }
    }

    public static int CalculateShortestDistance(Vertex startVertex, Vertex finalVertex, List<Vertex> allVertices)
    {
        startVertex.ShortestTimeTo = 0;
        Vertex? currentVertex;

        while ((currentVertex = GetUnexploredVertexWithSmallestDistance(allVertices)) != null)
        {
            currentVertex.IsExplored = true;

            foreach (Edge edge in currentVertex.Edges)
            {
                if (edge.RightVertex.ShortestTimeTo > edge.LeftVertex.ShortestTimeTo + edge.EdgeTime)
                {
                    edge.RightVertex.ShortestTimeTo = edge.LeftVertex.ShortestTimeTo + edge.EdgeTime;
                    edge.RightVertex.PreviousVertex = edge.LeftVertex;
                }
            }
        }

        int shortestDistanceValue = finalVertex.ShortestTimeTo;

        foreach (Vertex vertex in allVertices)
        {
            vertex.IsExplored = false;
            vertex.ShortestTimeTo = int.MaxValue;
            vertex.PreviousVertex = null;
        }

        return shortestDistanceValue;
    }

    public static Vertex? GetUnexploredVertexWithSmallestDistance(List<Vertex> allVertices)
    {
        int smallestWeightBound = int.MaxValue;
        Vertex? targetVertex = null;

        foreach (Vertex vertex in allVertices)
        {
            if (!vertex.IsExplored && vertex.ShortestTimeTo < smallestWeightBound)
            {
                targetVertex = vertex;
                smallestWeightBound = vertex.ShortestTimeTo;
            }
        }
        return targetVertex;
    }

    public static Vertex GetBestThiefEvasiveMove(Vertex currentThiefVertex, List<Vertex> allVertices, Board board)
    {
        Vertex? nearestPoliceOfficer = null;
        int minimumDistanceToPolice = int.MaxValue;

        foreach (Vertex policeVertex in board.PolicePositions)
        {
            int distanceToOfficer = CalculateShortestDistance(currentThiefVertex, policeVertex, allVertices);
            if (distanceToOfficer < minimumDistanceToPolice)
            {
                nearestPoliceOfficer = policeVertex;
                minimumDistanceToPolice = distanceToOfficer;
            }
        }

        if (nearestPoliceOfficer == null) return currentThiefVertex;

        Vertex safestEvasionNode = currentThiefVertex;
        int maximumEvasionDistance = int.MinValue;

        foreach (Edge edge in currentThiefVertex.Edges)
        {
            int escapeRouteValue = CalculateShortestDistance(edge.RightVertex, nearestPoliceOfficer, allVertices);
            if (escapeRouteValue > maximumEvasionDistance)
            {
                safestEvasionNode = edge.RightVertex;
                maximumEvasionDistance = escapeRouteValue;
            }
        }

        return safestEvasionNode;
    }

    public static void AddBidirectionalEdge(Vertex v1, Vertex v2, int timeCost)
    {
        v1.Edges.Add(new Edge(v1, v2, timeCost));
        v2.Edges.Add(new Edge(v2, v1, timeCost));
    }

    static void Main(string[] args)
    {
        Vertex vertexA = new Vertex("A");
        Vertex vertexB = new Vertex("B");
        Vertex vertexC = new Vertex("C");
        Vertex vertexD = new Vertex("D");
        Vertex vertexE = new Vertex("E");
        Vertex vertexF = new Vertex("F");
        Vertex vertexG = new Vertex("G");
        Vertex vertexH = new Vertex("H");
        Vertex vertexI = new Vertex("I");
        Vertex vertexJ = new Vertex("J");

        AddBidirectionalEdge(vertexA, vertexE, 1);
        AddBidirectionalEdge(vertexA, vertexJ, 1);
        AddBidirectionalEdge(vertexB, vertexD, 1);
        AddBidirectionalEdge(vertexC, vertexE, 1);
        AddBidirectionalEdge(vertexJ, vertexF, 1);
        AddBidirectionalEdge(vertexB, vertexG, 1);
        AddBidirectionalEdge(vertexF, vertexH, 1);
        AddBidirectionalEdge(vertexG, vertexI, 1);
        AddBidirectionalEdge(vertexH, vertexD, 1);
        AddBidirectionalEdge(vertexI, vertexA, 1);
        AddBidirectionalEdge(vertexB, vertexF, 1);
        AddBidirectionalEdge(vertexB, vertexC, 1);
        AddBidirectionalEdge(vertexF, vertexC, 1);

        List<Vertex> allVertices = new List<Vertex> { vertexA, vertexB, vertexC, vertexD, vertexE, vertexF, vertexG, vertexH, vertexI, vertexJ };
        List<Vertex> policePositions = new List<Vertex> { vertexB, vertexH };
        Vertex thiefPosition = vertexJ;

        Board gameBoard = new Board(allVertices, policePositions, thiefPosition);

        while (true)
        {

            // 1. Thief turn
            Console.WriteLine("\n>>> THIEF TURN: Evading closest pursuit...");
            thiefPosition = GetBestThiefEvasiveMove(thiefPosition, allVertices, gameBoard);
            gameBoard.ThiefPosition = thiefPosition;
            
            Console.WriteLine($"Thief shifted locations to node: {thiefPosition.Name}");

            if (policePositions.Any(p => p == thiefPosition))
            {
                gameBoard.DrawBoard();
                Console.WriteLine("\n🏆 GAME OVER: The Thief walked directly into custody!");
                break;
            }
            
            gameBoard.DrawBoard();

            // 2. Police turn
            Console.WriteLine("\n>>> POLICE TURN: Select an officer to command:");
            Console.WriteLine($"1. Officer 1 (At Node {policePositions[0].Name})");
            Console.WriteLine($"2. Officer 2 (At Node {policePositions[1].Name})");
            
            int selectedOfficerIndex;
            while (!int.TryParse(Console.ReadLine(), out selectedOfficerIndex) || (selectedOfficerIndex != 1 && selectedOfficerIndex != 2))
            {
                Console.WriteLine("Invalid selection. Choose 1 or 2:");
            }
            selectedOfficerIndex--;

            Vertex activeOfficerNode = policePositions[selectedOfficerIndex];
            Console.WriteLine("\nChoose destination lane:");
            
            for (int i = 0; i < activeOfficerNode.Edges.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Move to Node {activeOfficerNode.Edges[i].RightVertex.Name}");
            }

            int targetMoveIndex;
            while (!int.TryParse(Console.ReadLine(), out targetMoveIndex) || targetMoveIndex < 1 || targetMoveIndex > activeOfficerNode.Edges.Count)
            {
                Console.WriteLine($"Invalid selection. Enter a number between 1 and {activeOfficerNode.Edges.Count}:");
            }

            policePositions[selectedOfficerIndex] = activeOfficerNode.Edges[targetMoveIndex - 1].RightVertex;
            gameBoard.PolicePositions = policePositions;

            if (policePositions.Any(p => p == thiefPosition))
            {
                gameBoard.DrawBoard();
                Console.WriteLine("\n🚨 GAME OVER: Target apprehended! Perimeter secured successfully.");
                break;
            }
        }
    }
}