using System;
using System.IO;
using GraphNetworkSimulator.Core;
using GraphNetworkSimulator.Models;

namespace GraphNetworkSimulator
{
    class Program
    {
        static void Main()
        {
            const string filename = "input.txt";
            if (!File.Exists(filename))
            {
                Console.WriteLine($"Critical operational configuration failure: File '{filename}' is missing.");
                return;
            }

            Console.WriteLine($"Compiling topological map from data matrix '{filename}'...");
            string[] lines = File.ReadAllLines(filename);

            var builder = new GraphBuilder();
            try
            {
                builder.BuildGraph(lines);
                Console.WriteLine("Graph verification succeeded! Model loaded.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Structural Topology Compilation Error: " + ex.Message);
                return;
            }

            while (true)
            {
                Console.WriteLine("\n=======================================================");
                Console.WriteLine("Algorithms: random_walk | random_informed_walk | flooding | informed_flooding | exit");
                Console.Write("Select Strategy: ");
                string algorithmChoice = Console.ReadLine()?.Trim().ToLower() ?? "";

                if (algorithmChoice == "exit") break;

                Console.Write("Enter Starting Node ID: ");
                string startNodeId = Console.ReadLine()?.Trim() ?? "";

                Console.Write("Enter Target Resource ID: ");
                string resourceId = Console.ReadLine()?.Trim() ?? "";

                Console.Write("Enter Time-To-Live (TTL): ");
                if (!int.TryParse(Console.ReadLine(), out int ttl) || ttl < 0)
                {
                    Console.WriteLine("Invalid entry. TTL must be a non-negative integer baseline.");
                    continue;
                }

                if (!builder.Nodes.TryGetValue(startNodeId, out Node? startNode))
                {
                    Console.WriteLine($"Routing Error: Node '{startNodeId}' is not registered inside this topology.");
                    continue;
                }

                var algorithms = new Algorithms();
                (Node? foundNode, int messagesSent, int nodesAccessed) result;

                Console.WriteLine("\nExecuting search exploration matrix...");

                switch (algorithmChoice)
                {
                    case "random_walk":
                        result = algorithms.RandomWalk(startNode, resourceId, ttl);
                        break;
                    case "random_informed_walk":
                        result = algorithms.RandomInformedWalk(startNode, resourceId, ttl);
                        break;
                    case "flooding":
                        result = algorithms.FloodingMultithreaded(startNode, resourceId, ttl);
                        break;
                    case "informed_flooding":
                        result = algorithms.FloodingInformedMultithreaded(startNode, resourceId, ttl);
                        break;
                    default:
                        Console.WriteLine("Unknown strategy parameter. Verify string literal input syntax.");
                        continue;
                }

                Console.WriteLine("\n=== Search Traversal Matrix Diagnostics ===");
                if (result.foundNode != null)
                {
                    Console.WriteLine($"Status: SUCCESS — Resource discovered at Node [{result.foundNode.Id}]");
                }
                else
                {
                    Console.WriteLine("Status: FAILURE — Unreachable within the specified TTL horizon bounds.");
                }

                Console.WriteLine($"Total Edge Traversal Messages Dispatched: {result.messagesSent}");
                Console.WriteLine($"Total Graph Node References Evaluated   : {result.nodesAccessed}");
            }
        }
    }
}