# 🗺️ Dijkstra Shortest Path Router & Path Tracker

A high-performance navigation routing engine demonstrating graph theory mechanics through an optimized implementation of Dijkstra's Shortest Path Algorithm across an interconnected 8-city map layout.

---

## 📄 Project Overview

This project implements an optimized pathfinding engine designed to evaluate structural vertex nodes (Cities) connected via fixed-weighted directional vectors (Edges representing exact travel times). 

By leveraging an explicit greedy evaluation strategy paired with a binary min-priority heap structure, the application guarantees deterministic shortest-path evaluation. It features a path-reconstruction mechanism that backtracks through historical node links to output the absolute quickest sequence of travel coordinates.

### Core Objectives
* **Route Reconstruction:** Reversing back through node dependency branches to provide full step-by-step navigation tracking strings (e.g., `A -> C -> D -> F -> G -> H`).
* **Deterministic Modeling:** Utilizing hardcoded, fixed edge weights to guarantee reproducible and verifiable routing pathways.
* **Heap Optimization:** Minimizing node lookup times from a linear search $O(V)$ down to a logarithmic extraction time $O(\log V)$.

---

## 🛡️ Algorithmic Complexity

### Global Analysis
* **Total Time Complexity:** $O((V + E) \log V)$, where $V$ represents the number of Vertices (Cities) and $E$ represents the number of Edges. Reconstructing the final route map adds a minor linear pass $O(V)$, keeping the overall time complexity bound tightly to logarithmic heap speeds.
* **Total Space Complexity:** $O(V + E)$ to store the fixed graph networks, min-priority queues, and sequential path arrays inside system memory.

---

## 📝 Formal Pseudocode (Dijkstra Engine + Backtracking)

This structured English pseudocode covers both the calculation stage and the route backtracking array build logic:

```text
CLASS City:
    property name
    property shortestTimeTo
    property explored
    property edgeList
    property previousCity

CLASS Edge:
    property targetCity
    property pathTime

FUNCTION CalculateShortestPaths(startingCity):
    FOR EACH city IN map:
        city.shortestTimeTo = Infinity
        city.explored = False
        city.previousCity = Null

    startingCity.shortestTimeTo = 0
    minHeap = New PriorityQueue()
    minHeap.Enqueue(startingCity, 0)

    WHILE minHeap IS NOT empty:
        currentCity = minHeap.Dequeue()
        IF currentCity.explored IS True THEN: Continue
        currentCity.explored = True

        FOR EACH edge IN currentCity.edgeList:
            neighbor = edge.targetCity
            IF neighbor.explored IS NOT True THEN:
                calculatedTime = currentCity.shortestTimeTo + edge.pathTime
                IF calculatedTime < neighbor.shortestTimeTo THEN:
                    neighbor.shortestTimeTo = calculatedTime
                    neighbor.previousCity = currentCity
                    minHeap.Enqueue(neighbor, calculatedTime)

FUNCTION GetShortestPathRoute(destinationCity):
    pathList = New EmptyList()
    current = destinationCity
    
    WHILE current IS NOT Null:
        pathList.Append(current.name)
        current = current.previousCity
        
    pathList.Reverse()
    RETURN pathList
```

---

## 🚀 Execution Instructions

Compile and run this project using standard CLI tools:
```bash
dotnet new console
dotnet run
```
