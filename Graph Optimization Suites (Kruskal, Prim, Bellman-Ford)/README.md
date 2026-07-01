# 📉 Graph Optimization Suites (Kruskal, Prim, Bellman-Ford)

An optimized computational graph library executing Minimum Spanning Tree (MST) extractions and shortest path optimizations natively in C#.

---

## 📄 Project Overview

This project provides high-fidelity implementations of foundational graph network algorithms designed to process structural topology schemas efficiently. It evaluates structures through three specialized computational routines:
1. **Kruskal's Algorithm:** Evaluates Minimum Spanning Trees across forest nodes using disjoint array indices.
2. **Prim's Algorithm:** Generates Minimum Spanning Trees using local cluster boundary expansions.
3. **Bellman-Ford Algorithm:** Solves Single-Source Shortest Paths while detecting negative-weight dependency cycles.

The toolkit features an automated structural asset fallback mechanism. At runtime, the parsing loops look for asset tables (`kruskal.txt`, `prim.txt`, `BellmanFord.txt`). If any are detached, the program auto-hydrates memory structures with pre-set topologies to keep execution uninterrupted.

---

## 🛡️ Algorithmic Complexity Profile

### 1. Kruskal's MST
* **Time Complexity:** $O(E \log E)$ or $O(E \log V)$ to sort the edges by total path weight. Iterating through the sorted collection to evaluate elements against the disjoint lookup array scales at near-linear bounds of $O(E \cdot \alpha(V))$.
* **Space Complexity:** $O(V + E)$ auxiliary memory space, required to manage the disjoint coordinate structures alongside the resulting tree arrays.

### 2. Prim's MST
* **Time Complexity:** $O(E \log V)$ worst-case. Utilizing an internal binary min-priority queue replaces linear vertex sweeps with logarithmic extractions when pulling minimum boundaries from the fringe frontier.
* **Space Complexity:** $O(V + E)$ memory footprint, needed to maintain adjacency list structures and priority queue records.

### 3. Bellman-Ford Pathfinding
* **Time Complexity:** $O(V \cdot E)$ worst-case. The routine loops over all edges systematically $V-1$ times to guarantee weight relaxation bounds reach every node across structural paths.
* **Space Complexity:** $O(V)$ auxiliary tracking space, required to record distance parameters and tracking histories.

---

## 📝 Formal Pseudocode (Algorithm Core Engines)

### 1. Kruskal's Engine
```text
CLASS DisjointSet:
    property parentArray
    property rankArray

    FUNCTION Find(element):
        IF parentArray[element] != element THEN:
            parentArray[element] = Find(parentArray[element]) // Path Compression
        RETURN parentArray[element]

    FUNCTION Union(element1, element2):
        root1 = Find(element1)
        root2 = Find(element2)
        IF root1 == root2 THEN: RETURN False
        // Balance trees using Rank Optimization
        IF rankArray[root1] < rankArray[root2] THEN: parentArray[root1] = root2
        ELSE IF rankArray[root1] > rankArray[root2] THEN: parentArray[root2] = root1
        ELSE:
            parentArray[root2] = root1
            rankArray[root1] = rankArray[root1] + 1
        RETURN True

FUNCTION KruskalMST(graph):
    Sort(graph.edges) // Prioritize lower weights
    mstList = New EmptyList()
    ds = New DisjointSet(graph.vertices)

    FOR EACH edge IN graph.edges:
        IF ds.Union(edge.source, edge.destination) IS True THEN:
            mstList.Append(edge)
            
    RETURN mstList
```

### 2. Prim's Engine
```text
FUNCTION PrimMST(graph, rootNode):
    predecessors = FillArray(size = graph.vertices, value = -2)
    visited = FillArray(size = graph.vertices, value = False)
    minWeightToCluster = FillArray(size = graph.vertices, value = Infinity)
    mstList = New EmptyList()

    minHeap = New PriorityQueue()
    minWeightToCluster[rootNode] = 0
    minHeap.Enqueue((rootNode, parent = -1, weight = 0.0), priority = 0.0)

    WHILE minHeap IS NOT empty:
        current = minHeap.Dequeue()
        IF visited[current.node] IS True THEN: Continue
        visited[current.node] = True
        
        predecessors[current.node] = current.parent
        IF current.parent != -1 THEN:
            mstList.Append(New Edge(current.parent, current.node, current.weight))

        FOR EACH edge IN graph.adjacencyList[current.node]:
            IF visited[edge.neighbor] IS False AND edge.weight < minWeightToCluster[edge.neighbor] THEN:
                minWeightToCluster[edge.neighbor] = edge.weight
                minHeap.Enqueue((edge.neighbor, current.node, edge.weight), priority = edge.weight)

    RETURN (predecessors, mstList)
```

### 3. Bellman-Ford Engine
```text
FUNCTION BellmanFord(graph, sourceVertex):
    predecessors = FillArray(size = graph.vertices, value = -1)
    distances = FillArray(size = graph.vertices, value = Infinity)
    distances[sourceVertex] = 0

    // Core relaxation sweeps
    FOR iteration FROM 1 TO graph.vertices - 1:
        FOR EACH edge IN graph.edges:
            IF distances[edge.source] + edge.weight < distances[edge.destination] THEN:
                distances[edge.destination] = distances[edge.source] + edge.weight
                predecessors[edge.destination] = edge.source

    // Verification sweep for negative weight cycle tracking
    FOR EACH edge IN graph.edges:
        IF distances[edge.source] + edge.weight < distances[edge.destination] THEN:
            RETURN (predecessors, distances, hasNegativeCycle = True)

    RETURN (predecessors, distances, hasNegativeCycle = False)
```

---

## 🚀 Execution Instructions

Compile and run this project suite natively using standard CLI utility tools:
```bash
dotnet new console
dotnet run
```
