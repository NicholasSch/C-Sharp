using System.Collections.Concurrent;
using GraphNetworkSimulator.Models;

namespace GraphNetworkSimulator.Core
{
    public class Algorithms
    {
        private readonly Random _random = new Random();
        private volatile bool _resourceFound = false;

        // Random Walk Strategy (Bounded Randomized DFS)
        // Time Complexity: Worst-case O(V + E) where V is vertices and E is edges, strictly capped by the TTL limit.
        // Space Complexity: Worst-case O(V) to store visited tracker states and execution recursion stack frames.
        public (Node? foundNode, int messagesSent, int nodesAccessed) RandomWalk(Node initialNode, string resourceId, int ttl)
        {
            var visited = new HashSet<Node>();
            int messagesSent = 0;
            int nodesAccessed = 0;

            Node? DFS(Node currentNode, int remainingTtl)
            {
                visited.Add(currentNode);
                nodesAccessed++;

                // Verify local resource collection
                foreach (var resource in currentNode.Resources)
                {
                    if (resource.Item1 == resourceId)
                        return currentNode;
                }

                // Halt if path limits are exhausted
                if (remainingTtl <= 0)
                    return null;

                // Randomize neighborhood selections to remove traversal biases
                var unvisitedNeighbors = currentNode.Edges
                    .Select(e => e.Item2)
                    .Where(n => !visited.Contains(n))
                    .OrderBy(_ => _random.Next())
                    .ToList();

                foreach (var neighbor in unvisitedNeighbors)
                {
                    messagesSent++;
                    Console.WriteLine($"[Random Walk] Routing: Node {currentNode.Id} -> Node {neighbor.Id}");
                    
                    Node? result = DFS(neighbor, remainingTtl - 1);
                    if (result != null)
                        return result;
                }

                return null;
            }

            Node? foundNode = DFS(initialNode, ttl);
            return (foundNode, messagesSent, nodesAccessed);
        }

        // Random Informed Walk Strategy
        // Time Complexity: O(TTL) since it executes a simple linear chain loop step-by-step.
        // Space Complexity: O(1) auxiliary space as it relies on iterative updates without deep frame allocations.
        public (Node? foundNode, int messagesSent, int nodesAccessed) RandomInformedWalk(Node initialNode, string resourceId, int ttl)
        {
            Node currentNode = initialNode;
            int messagesSent = 0;
            int nodesAccessed = 0;
            int stepsTaken = 0;

            while (stepsTaken < ttl)
            {
                nodesAccessed++;
                Console.WriteLine($"[Informed Walk] Inspecting memory properties at Node {currentNode.Id}");

                // 1. Direct local resource verification
                foreach (var resource in currentNode.Resources)
                {
                    if (resource.Item1 == resourceId)
                    {
                        if (currentNode != initialNode)
                        {
                            lock (initialNode.CacheLock)
                            {
                                initialNode.Cache.Add(new Tuple<string, Node>(resourceId, currentNode));
                            }
                        }
                        return (currentNode, messagesSent, nodesAccessed);
                    }
                }

                // 2. Local network index cache verification
                lock (currentNode.CacheLock)
                {
                    foreach (var cachedEntry in currentNode.Cache)
                    {
                        if (cachedEntry.Item1 == resourceId)
                        {
                            if (currentNode != initialNode)
                            {
                                lock (initialNode.CacheLock)
                                {
                                    initialNode.Cache.Add(new Tuple<string, Node>(resourceId, cachedEntry.Item2));
                                }
                            }
                            return (cachedEntry.Item2, messagesSent, nodesAccessed);
                        }
                    }
                }

                // 3. Fallback: Drop down to a completely random structural link connection
                if (currentNode.Edges.Count == 0)
                    return (null, messagesSent, nodesAccessed);

                messagesSent++;
                int randomEdgeIndex = _random.Next(0, currentNode.Edges.Count);
                Node nextNode = currentNode.Edges[randomEdgeIndex].Item2;
                
                Console.WriteLine($"[Informed Walk] Cache Miss. Traveling from Node {currentNode.Id} -> Node {nextNode.Id}");
                currentNode = nextNode;
                stepsTaken++;
            }

            return (null, messagesSent, nodesAccessed);
        }

        // Flooding Strategy (Concurrent Multithreaded BFS)
        // Time Complexity: Worst-case O(V + E) processing across parallel threads up to the designated TTL radius.
        // Space Complexity: Worst-case O(V) for thread-safe concurrent visited lookups and thread worker frames.
        public (Node? foundNode, int messagesSent, int nodesAccessed) FloodingMultithreaded(Node origin, string resourceId, int ttl)
        {
            _resourceFound = false;
            var visited = new ConcurrentDictionary<Node, bool>();
            var countdownEvent = new CountdownEvent(1); // Track background tasks safely without race conditions
            
            int messagesSent = 0;
            int nodesAccessed = 0;
            Node? resultNode = null;

            void Flood(object? state)
            {
                var (currentNode, currentTtl) = ((Node, int))state!;

                try
                {
                    if (_resourceFound || !visited.TryAdd(currentNode, true))
                        return;

                    Interlocked.Increment(ref nodesAccessed);

                    foreach (var resource in currentNode.Resources)
                    {
                        if (resource.Item1 == resourceId)
                        {
                            _resourceFound = true;
                            resultNode = currentNode;
                            return;
                        }
                    }

                    if (currentTtl <= 0 || _resourceFound)
                        return;

                    foreach (var edge in currentNode.Edges)
                    {
                        var neighbor = edge.Item2;
                        if (!visited.ContainsKey(neighbor) && !_resourceFound)
                        {
                            Console.WriteLine($"[Flooding] Node {currentNode.Id} broadcasting -> Node {neighbor.Id}");
                            Interlocked.Increment(ref messagesSent);
                            
                            // Register task before submitting to the thread pool
                            countdownEvent.AddCount();
                            ThreadPool.QueueUserWorkItem(Flood, (neighbor, currentTtl - 1));
                        }
                    }
                }
                finally
                {
                    countdownEvent.Signal(); // Safely drop work item tracking count
                }
            }

            ThreadPool.QueueUserWorkItem(Flood, (origin, ttl));
            countdownEvent.Wait(); // Hold execution line securely until thread signals hit zero

            return (resultNode, messagesSent, nodesAccessed);
        }

        // Informed Flooding Strategy (Concurrent Caching BFS)
        // Time Complexity: Worst-case O(V + E), optimized to exit branches early on local cache hits.
        // Space Complexity: Worst-case O(V) to support concurrent structures and safe thread mapping allocations.
        public (Node? foundNode, int messagesSent, int nodesAccessed) FloodingInformedMultithreaded(Node origin, string resourceId, int ttl)
        {
            _resourceFound = false;
            var visited = new ConcurrentDictionary<Node, bool>();
            var countdownEvent = new CountdownEvent(1);
            
            int messagesSent = 0;
            int nodesAccessed = 0;
            Node? resultNode = null;

            void Flood(object? state)
            {
                var (currentNode, currentTtl) = ((Node, int))state!;

                try
                {
                    if (_resourceFound || !visited.TryAdd(currentNode, true))
                        return;

                    Interlocked.Increment(ref nodesAccessed);

                    // 1. Direct local resource verification
                    foreach (var resource in currentNode.Resources)
                    {
                        if (resource.Item1 == resourceId)
                        {
                            _resourceFound = true;
                            resultNode = currentNode;

                            if (currentNode != origin)
                            {
                                lock (origin.CacheLock)
                                {
                                    origin.Cache.Add(new Tuple<string, Node>(resourceId, currentNode));
                                }
                            }
                            return;
                        }
                    }

                    // 2. Direct local cache validation
                    lock (currentNode.CacheLock)
                    {
                        foreach (var cached in currentNode.Cache)
                        {
                            if (cached.Item1 == resourceId)
                            {
                                _resourceFound = true;
                                resultNode = cached.Item2;

                                if (currentNode != origin)
                                {
                                    lock (origin.CacheLock)
                                    {
                                        origin.Cache.Add(new Tuple<string, Node>(resourceId, cached.Item2));
                                    }
                                }
                                return;
                            }
                        }
                    }

                    if (currentTtl <= 0 || _resourceFound)
                        return;

                    // 3. Broadcast to neighbors
                    foreach (var edge in currentNode.Edges)
                    {
                        var neighbor = edge.Item2;
                        if (!visited.ContainsKey(neighbor) && !_resourceFound)
                        {
                            Console.WriteLine($"[Informed Flooding] Node {currentNode.Id} broadcasting -> Node {neighbor.Id}");
                            Interlocked.Increment(ref messagesSent);
                            
                            countdownEvent.AddCount();
                            ThreadPool.QueueUserWorkItem(Flood, (neighbor, currentTtl - 1));
                        }
                    }
                }
                finally
                {
                    countdownEvent.Signal();
                }
            }

            ThreadPool.QueueUserWorkItem(Flood, (origin, ttl));
            countdownEvent.Wait();

            return (resultNode, messagesSent, nodesAccessed);
        }
    }
}