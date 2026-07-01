# 🚓 Tactical Graph Evasion Simulator (Police vs. Thief)

A terminal-based operational research game that uses real-time graph traversal calculations to drive an evasive artificial intelligence engine across an interconnected grid network.

---

## 📄 Project Overview

This project implements a turn-based strategic chase simulation on an undirected, unweighted network graph. The game map consists of 10 structural zones (Vertices $A$ through $J$) linked by bidirectional transit pathways (Edges). Players take control of a coordinated squad of two police officers trying to box in a single automated thief controlled by an adversarial pathfinding algorithm.

The core game engine runs an asymmetric loop where both sides utilize structural graph properties to achieve their objectives:
* **The Police Strategy:** Requires the player to cooperatively maneuver units across adjacent nodes to limit the thief's escape options and force a tactical cornering layout.
* **The Thief AI Strategy:** Employs an exact **Maximin Evasion Rule**. At the start of its turn, the thief evaluates the shortest path distance to all active pursuers, isolates the most immediate threat, and moves to the neighboring node that maximizes its distance away from that specific threat.

---

## 🗺️ Network Topology Matrix

The gameplay board operates on a strict, hardcoded relational network layout. The following connectivity table outlines every valid travel pathway compiled inside the system:

| Node Source | Valid Adjacent Move Targets | Total Degree |
|:-----------:|-----------------------------|:------------:|
| **A** | E, J, I                     | 3            |
| **B** | D, G, F, C                  | 4            |
| **C** | E, F, B                     | 3            |
| **D** | B, H                        | 2            |
| **E** | A, C                        | 2            |
| **F** | J, H, B, C                  | 4            |
| **G** | B, I                        | 2            |
| **H** | F, D                        | 2            |
| **I** | G, A                        | 2            |
| **J** | A, F                        | 2            |

---

## 🧠 Mechanics & AI Engine Behavior

### 1. The Shortest Path Matrix (Dijkstra's Relaxation)
To make accurate tactical decisions, the game must understand real-time graph distances. The algorithm measures distance via edge weights (where each hop costs a uniform value of `1`). It runs a stateless single-source Dijkstra routine that updates tentative costs through node relaxation loops before immediately clearing weights between turns.

### 2. The Maximin Evasion Property
The thief AI does not make random movements; it runs a predictive defense sweep using two clear mathematical phases:
1. **Threat Assessment:** It evaluates the shortest path distance from its current position to all police positions $P_n$. The officer with the minimum path weight is flagged as the primary threat:
   $$\text{Target Threat} = \min(\text{Dijkstra}(\text{Thief}, P_1), \text{Dijkstra}(\text{Thief}, P_2))$$
2. **Evasive Choice:** The thief checks all available adjacent exit edges. It calculates the hypothetical threat distance for each option and chooses the vertex that offers the maximum breathing room:
   $$\text{Optimal Move} = \max(\text{Dijkstra}(\text{Neighbor}_i, \text{Target Threat}))$$

---

## 🛡️ Algorithmic Complexity Profile

### Operational Boundaries
* **Path Generation (Dijkstra's Routine):** $O(V^2)$ via standard array-sweeps. Given the fixed dimension size of $V = 10$, this look-ahead calculation evaluates instantly.
* **AI Decision Step Time Complexity:** $O(P \cdot V^2 + E \cdot V^2)$ worst-case per turn, where $P$ is the number of active police units and $E$ is the connection degree of the active vertex. The algorithm runs distance checks across all neighboring exits to find the safest route before committing to a move.
* **Total Space Complexity:** $O(V + E)$ to maintain the graph network's structural layout and track active position indexes in system memory.

---

## 📝 Formal Pseudocode (Evasion & Traversal Engines)

This structured English pseudocode maps out the path calculation and evasive decision-making processes running inside the game:

```text
CLASS Vertex:
    property name
    property shortestTimeTo
    property isExplored
    property edgeList

CLASS Edge:
    property leftVertex
    property rightVertex
    property timeCost

FUNCTION CalculateShortestDistance(startVertex, finalVertex, graphNodes):
    // Initialize standard single-source path parameters
    startVertex.shortestTimeTo = 0

    WHILE (current = GetUnexploredVertexWithSmallestDistance(graphNodes)) IS NOT Null:
        current.isExplored = True

        FOR EACH edge IN current.edgeList:
            neighbor = edge.rightVertex
            
            // Standard edge relaxation loop pass
            IF neighbor.shortestTimeTo > current.shortestTimeTo + edge.timeCost THEN:
                neighbor.shortestTimeTo = current.shortestTimeTo + edge.timeCost

    shortestDistanceValue = finalVertex.shortestTimeTo
    
    // Clear tracking attributes immediately to keep the graph database stateless
    ResetGraphStateToDefaults(graphNodes)
    RETURN shortestDistanceValue


FUNCTION GetBestThiefEvasiveMove(thiefNode, graphNodes, policePositions):
    nearestOfficerNode = Null
    minDistanceToOfficer = Infinity

    // Phase 1: Scan the network and locate the closest chasing threat
    FOR EACH officerNode IN policePositions:
        distanceToTarget = CalculateShortestDistance(thiefNode, officerNode, graphNodes)
        IF distanceToTarget < minDistanceToOfficer THEN:
            minDistanceToOfficer = distanceToTarget
            nearestOfficerNode = officerNode

    // If the path tracking map returns null, hold current position safely
    IF nearestOfficerNode IS Null THEN:
        RETURN thiefNode

    safestEvasionVertex = thiefNode
    maxEvasionDistance = -Infinity

    // Phase 2: Select the neighboring escape link that maximizes your survival safety margin
    FOR EACH edge IN thiefNode.edgeList:
        escapeNodeOption = edge.rightVertex
        distanceValue = CalculateShortestDistance(escapeNodeOption, nearestOfficerNode, graphNodes)
        
        IF distanceValue > maxEvasionDistance THEN:
            maxEvasionDistance = distanceValue
            safestEvasionVertex = escapeNodeOption

    RETURN safestEvasionVertex
```

---

## 🚀 Execution Instructions

Compile and run this project suite natively using standard CLI utility tools:
```bash
dotnet new console
dotnet run
```
