class Program 
{
    // Space: O(1) | Time: O(1)
    public class City  
    {
        public string CityName { get; set; }
        public int ShortestTimeTo { get; set; }
        public bool Explored { get; set; }
        public List<Edge> Edges { get; set; }
        public City? PreviousCity { get; set; }

        public City(string cityName)
        {
            CityName = cityName;
            ShortestTimeTo = int.MaxValue;
            Explored = false;
            Edges = new List<Edge>();
            PreviousCity = null;
        }
    }

    // Space: O(1) | Time: O(1)
    public class Edge 
    { 
        public City TargetCity { get; set; }
        public int PathTime { get; set; }

        public Edge(City targetCity, int pathTime)
        {
            TargetCity = targetCity;
            PathTime = pathTime;
        }
    }

    // Space: O(V) | Time: O((V + E) * log V)
    public static void CalculateShortestPaths(City startingCity)
    {
        startingCity.ShortestTimeTo = 0;
        var minHeap = new PriorityQueue<City, int>();
        minHeap.Enqueue(startingCity, 0);

        while (minHeap.Count > 0)
        {
            City current = minHeap.Dequeue();

            if (current.Explored) continue;
            current.Explored = true;

            foreach (Edge edge in current.Edges)
            {
                City neighbor = edge.TargetCity;

                if (!neighbor.Explored)
                {
                    int timeToNeighbor = current.ShortestTimeTo + edge.PathTime;

                    if (timeToNeighbor < neighbor.ShortestTimeTo)
                    {
                        neighbor.ShortestTimeTo = timeToNeighbor;
                        neighbor.PreviousCity = current;
                        minHeap.Enqueue(neighbor, timeToNeighbor);
                    }
                }
            }
        }
    }

    // Space: O(V) | Time: O(V)
    public static List<string> GetShortestPathRoute(City destinationCity)
    {
        List<string> path = new List<string>();
        City? current = destinationCity;

        while (current != null)
        {
            path.Add(current.CityName);
            current = current.PreviousCity;
        }

        path.Reverse();
        return path;
    }

    static void Main(string[] args)
    {
        // Initializing 8 distinct cities
        City cityA = new City("A");
        City cityB = new City("B");
        City cityC = new City("C");
        City cityD = new City("D");
        City cityE = new City("E");
        City cityF = new City("F");
        City cityG = new City("G");
        City cityH = new City("H");

        // Configuring network with interconnected fixed-weight edges
        cityA.Edges.Add(new Edge(cityB, 5));
        cityA.Edges.Add(new Edge(cityC, 5));
        
        cityB.Edges.Add(new Edge(cityC, 1));
        cityB.Edges.Add(new Edge(cityD, 6));
        
        cityC.Edges.Add(new Edge(cityD, 2));
        cityC.Edges.Add(new Edge(cityE, 6));
        
        cityD.Edges.Add(new Edge(cityF, 4));
        
        cityE.Edges.Add(new Edge(cityD, 1));
        cityE.Edges.Add(new Edge(cityG, 2));
        
        cityF.Edges.Add(new Edge(cityG, 1));
        cityF.Edges.Add(new Edge(cityH, 5));
        
        cityG.Edges.Add(new Edge(cityH, 3));

        // Calculate paths starting from City A
        CalculateShortestPaths(cityA);

        // Extract and display the absolute shortest path to City H
        List<string> fullRoute = GetShortestPathRoute(cityH);
        string routeDisplay = string.Join(" -> ", fullRoute);

        Console.WriteLine($"Shortest Route from A to H: {routeDisplay}");
        Console.WriteLine($"Total Travel Time: {cityH.ShortestTimeTo} mins.");
    }
}