# 🎒Bounded Knapsack Optimization Engine

A high-performance operational research simulator executing an exact recursive Branch-and-Bound algorithm to solve the Bounded Knapsack Problem natively in C#.

---

## 📄 Project Overview

This repository features an exact mathematical optimization engine designed to maximize item payload utility within a strict resource capacity threshold. The program systematically determines the optimal integer quantities of distinct items to select, balancing individual item weights against total container constraints.

The architecture uses a multi-layered design. It features a fast continuous relaxation module that utilizes a greedy allocation strategy based on item efficiencies to establish mathematical upper bounds. If fractional values occur, the engine triggers a recursive binary space segmentation tree (Branch and Bound). Additionally, it contains a file fallback parsing asset strategy, allowing it to transition seamlessly to an internal pre-established dataset if the external data sheet (`entrada.txt`) is missing.

### Core Objectives
* **Algorithmic Precision:** Maximizing resource utility within tight constraint boundaries without discarding absolute mathematical proof.
* **Hybrid Search Framework:** Blending a fast greedy continuous relaxation module with a recursive binary space segmentation strategy.
* **Robust Fallback Architecture:** Ensuring execution stability through localized default data profiles when external input streams are detached.

---

## 🛡️ Algorithmic Complexity Profile

### Global Bound Limits
* **Total Time Complexity:** $O(2^N \cdot N \log N)$ worst-case, where $N$ represents the number of unique items. Each subproblem node computes continuous fractional relaxations in $O(N \log N)$ time due to item efficiency sorting.
* **Total Space Complexity:** $O(N \cdot D)$ auxiliary memory structures, required to store localized branch boundary deep copies across the stack allocation layers (where $D$ represents maximum recursion branch tree depth).

---

## 📝 Formal Pseudocode (Branch & Bound Optimization Engine)

This structured pseudocode maps the allocation steps, boundary controls, and pruning rules running inside the system:

```text
FUNCTION SolveContinuousKnapsack(lowerBounds, upperBounds, capacity):
    Initialize allocationVector with lowerBounds
    remainingCapacity = capacity - Sum(lowerBounds * itemWeights)
    
    IF remainingCapacity < 0 THEN:
        RETURN (IsFeasible = False, Profit = 0)

    // Calculate item efficiency (Profit / Weight)
    sortedItemIndices = SortIndicesByEfficiencyDescending()

    FOR EACH index IN sortedItemIndices:
        availableSpace = upperBounds[index] - lowerBounds[index]
        IF availableSpace <= 0 THEN: Continue
        
        neededCapacity = availableSpace * itemWeights[index]

        IF remainingCapacity >= neededCapacity THEN:
            allocationVector[index] += availableSpace
            remainingCapacity -= neededCapacity
        ELSE:
            fractionalPart = remainingCapacity / itemWeights[index]
            allocationVector[index] += fractionalPart
            remainingCapacity = 0
            BREAK Loop

    totalProfit = Sum(allocationVector * itemProfits)
    RETURN (IsFeasible = True, totalProfit, allocationVector)


FUNCTION BranchAndBound(lowerBounds, upperBounds, currentBestValue, currentBestVariables):
    relaxation = SolveContinuousKnapsack(lowerBounds, upperBounds, maxCapacity)

    // Prune Step 1: Subproblem config is mathematically impossible
    IF relaxation.IsFeasible IS False THEN:
        RETURN (currentBestValue, currentBestVariables)

    // Prune Step 2 (Bounding): Branch cannot beat our global best integer record
    IF relaxation.Profit <= currentBestValue THEN:
        RETURN (currentBestValue, currentBestVariables)

    fractionalIndex = FindFirstFractionalIndex(relaxation.allocationVector)

    // Branch Step: Split continuous space on a fractional item coordinate
    IF fractionalIndex IS Valid THEN:
        floorValue = Floor(relaxation.allocationVector[fractionalIndex])
        ceilingValue = Ceiling(relaxation.allocationVector[fractionalIndex])

        // Branch Left Path: Enforce constraint (Variable <= Floor)
        leftUpperBounds = Clone(upperBounds)
        leftUpperBounds[fractionalIndex] = Min(upperBounds[fractionalIndex], floorValue)
        leftResult = BranchAndBound(lowerBounds, leftUpperBounds, currentBestValue, currentBestVariables)
        
        IF leftResult.value > currentBestValue THEN:
            currentBestValue = leftResult.value
            currentBestVariables = leftResult.variables

        // Branch Right Path: Enforce constraint (Variable >= Ceiling)
        rightLowerBounds = Clone(lowerBounds)
        rightLowerBounds[fractionalIndex] = Max(lowerBounds[fractionalIndex], ceilingValue)
        rightResult = BranchAndBound(rightLowerBounds, upperBounds, currentBestValue, currentBestVariables)
        
        IF rightResult.value > currentBestValue THEN:
            currentBestValue = rightResult.value
            currentBestVariables = rightResult.variables
            
    // Feasibility Step: All coordinates are whole integers, record output path
    ELSE:
        IF relaxation.Profit > currentBestValue THEN:
            RETURN (relaxation.Profit, relaxation.allocationVector)

    RETURN (currentBestValue, currentBestVariables)
```

---

## 🚀 Execution Instructions

Compile and execute this performance tracker directly from your machine terminal window:
```bash
dotnet new console
dotnet run
```
