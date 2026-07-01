using System.Globalization;

class Program
{
    // Global problem inputs
    static int numItems;
    static double[] profits = Array.Empty<double>();
    static double[] weights = Array.Empty<double>();
    static double maxCapacity;

    static void Main(string[] args) // Space: O(N) | Time: O(2^N * N log N) worst-case
    {
        string filePath = "C:\\VSCODE\\Backpack problem\\Backpack problem input.txt";

        // Fallback Strategy: Load pre-established dataset if the text file is missing
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"[Warning] Input file not found at '{filePath}'. Loading pre-established default dataset...");
            
            numItems = 4;
            profits = new double[] { 3, 8, 5, 1 };
            weights = new double[] { 2, 3, 2, 4 };
            double[] defaultUpperBounds = new double[] { 100, 100, 100, 100 };
            double[] defaultLowerBounds = new double[] { 0, 0, 0, 0 };
            maxCapacity = 7;

            ExecuteOptimization(defaultLowerBounds, defaultUpperBounds);
            return;
        }

        // Parse dataset from text file
        try
        {
            string[] fileLines = File.ReadAllLines(filePath);
            numItems = int.Parse(fileLines[0].Trim());
            profits = Array.ConvertAll(fileLines[1].Trim().Split(' '), double.Parse);
            weights = Array.ConvertAll(fileLines[2].Trim().Split(' '), double.Parse);
            double[] upperBounds = Array.ConvertAll(fileLines[3].Trim().Split(' '), double.Parse);
            double[] lowerBounds = Array.ConvertAll(fileLines[4].Trim().Split(' '), double.Parse);
            
            string maxWeightRaw = fileLines[5].Trim().Replace(',', '.');
            maxCapacity = double.Parse(maxWeightRaw, CultureInfo.InvariantCulture);

            ExecuteOptimization(lowerBounds, upperBounds);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Critical Error parsing file data: {ex.Message}");
        }
    }

    static void ExecuteOptimization(double[] initialLowers, double[] initialUppers)
    {
        Console.WriteLine($"Variables count (n): {numItems}");
        Console.WriteLine($"Profits vector (c): {string.Join(", ", profits)}");
        Console.WriteLine($"Weights vector (w): {string.Join(", ", weights)}");
        Console.WriteLine($"Max Capacity (W): {maxCapacity}\n");

        // Run baseline continuous relaxation check
        var continuousRelaxation = SolveContinuousKnapsack(initialLowers, initialUppers);
        
        if (!continuousRelaxation.IsFeasible)
        {
            Console.WriteLine("Initial baseline problem is completely Infeasible.");
            return;
        }

        Console.WriteLine("--- Continuous Relaxation Root Result ---");
        Console.WriteLine($"Relaxed Variables: [{string.Join(", ", continuousRelaxation.Variables.Select(v => v.ToString("F4")))}]");
        Console.WriteLine($"Relaxed Objective Profit: {continuousRelaxation.Profit:F4}\n");

        // Start native branch and bound tree search
        Console.WriteLine("--- Executing Native Branch and Bound Tree ---");
        var finalResult = BranchAndBound(initialLowers, initialUppers, 0.0, new double[numItems]);

        if (finalResult.BestValue > 0.0)
        {
            Console.WriteLine("\n=========================================");
            Console.WriteLine("          FINAL OPTIMAL SOLUTION        ");
            Console.WriteLine("=========================================");
            Console.WriteLine($"Maximized Integer Profit: {finalResult.BestValue:F1}");
            Console.WriteLine($"Selected Item Quantities: [{string.Join(", ", finalResult.BestVariables)}]");
        }
        else
        {
            Console.WriteLine("\nOptimization Complete: No Feasible Integer Solution Exists.");
        }
    }

    // Native Continuous Bounded Knapsack Solver (Greedy Strategy)
    // Space: O(N) | Time: O(N log N) due to sorting by item efficiency
    static (bool IsFeasible, double Profit, double[] Variables) SolveContinuousKnapsack(double[] lowerBounds, double[] upperBounds)
    {
        double[] x = new double[numItems];
        double remainingCapacity = maxCapacity;

        // 1. Verify basic boundary integrity and fulfill absolute lower bounds
        for (int i = 0; i < numItems; i++)
        {
            if (lowerBounds[i] > upperBounds[i]) return (false, 0, x);
            x[i] = lowerBounds[i];
            remainingCapacity -= lowerBounds[i] * weights[i];
        }

        // If lower bounds already exceed the knapsack capacity, this branch is invalid
        if (remainingCapacity < 0) return (false, 0, x);

        // 2. Sort available item allocations based on profit efficiency (Profit / Weight)
        var itemsToAllocate = Enumerable.Range(0, numItems)
            .Select(i => new { Index = i, Efficiency = profits[i] / weights[i] })
            .OrderByDescending(item => item.Efficiency)
            .ToList();

        // 3. Greedily fill remaining space up to upper bounds
        foreach (var item in itemsToAllocate)
        {
            int idx = item.Index;
            double availableExtraAllocation = upperBounds[idx] - lowerBounds[idx];
            if (availableExtraAllocation <= 0) continue;

            double weightOfExtraAllocation = availableExtraAllocation * weights[idx];

            if (remainingCapacity >= weightOfExtraAllocation)
            {
                x[idx] += availableExtraAllocation;
                remainingCapacity -= weightOfExtraAllocation;
            }
            else
            {
                // Allocate fractional part of the item to exhaust remaining capacity
                double fractionalAllocation = remainingCapacity / weights[idx];
                x[idx] += fractionalAllocation;
                remainingCapacity = 0;
                break;
            }
        }

        double totalProfit = 0;
        for (int i = 0; i < numItems; i++)
        {
            totalProfit += x[i] * profits[i];
        }

        return (true, totalProfit, x);
    }

    // Recursive space segmentation loop
    static (double BestValue, double[] BestVariables) BranchAndBound(
        double[] lowerBounds, 
        double[] upperBounds, 
        double globalBestValue, 
        double[] globalBestVariables)
    {
        var relaxation = SolveContinuousKnapsack(lowerBounds, upperBounds);

        // Prune Step 1: If subproblem is infeasible, drop branch
        if (!relaxation.IsFeasible) return (globalBestValue, globalBestVariables);

        // Prune Step 2 (Bounding Check): If relaxation bound cannot beat our current best, drop branch
        if (relaxation.Profit <= globalBestValue) return (globalBestValue, globalBestVariables);

        // Check for fractional values in the current solution vector
        int fractionalIndex = -1;
        double fractionalValue = 0.0;

        for (int i = 0; i < numItems; i++)
        {
            if (Math.Round(relaxation.Variables[i], 10) % 1 != 0)
            {
                fractionalIndex = i;
                fractionalValue = relaxation.Variables[i];
                break;
            }
        }

        // Branch Step: If a fractional coordinate is found, split the search space recursively
        if (fractionalIndex != -1)
        {
            double floorVal = Math.Floor(fractionalValue);
            double ceilVal = Math.Ceiling(fractionalValue);

            // Left Branch: Variable <= Floor (Update Upper Bound)
            double[] leftUpperBounds = (double[])upperBounds.Clone();
            leftUpperBounds[fractionalIndex] = Math.Min(upperBounds[fractionalIndex], floorVal);
            var leftBranch = BranchAndBound(lowerBounds, leftUpperBounds, globalBestValue, globalBestVariables);
            
            if (leftBranch.BestValue > globalBestValue)
            {
                globalBestValue = leftBranch.BestValue;
                globalBestVariables = leftBranch.BestVariables;
            }

            // Right Branch: Variable >= Ceiling (Update Lower Bound)
            double[] rightLowerBounds = (double[])lowerBounds.Clone();
            rightLowerBounds[fractionalIndex] = Math.Max(lowerBounds[fractionalIndex], ceilVal);
            var rightBranch = BranchAndBound(rightLowerBounds, upperBounds, globalBestValue, globalBestVariables);
            
            if (rightBranch.BestValue > globalBestValue)
            {
                globalBestValue = rightBranch.BestValue;
                globalBestVariables = rightBranch.BestVariables;
            }
        }
        // Feasibility Step: Feasible integer solution found
        else
        {
            double currentIntegerProfit = 0;
            for (int i = 0; i < numItems; i++)
            {
                currentIntegerProfit += Math.Round(relaxation.Variables[i]) * profits[i];
            }

            if (currentIntegerProfit > globalBestValue)
            {
                Console.WriteLine($"New Feasible Integer Solution Found: [{string.Join(", ", relaxation.Variables)}] | Profit: {currentIntegerProfit:F1}");
                return (currentIntegerProfit, Array.ConvertAll(relaxation.Variables, Math.Round));
            }
        }

        return (globalBestValue, globalBestVariables);
    }
}