# 🕸️ Graph Traversal Strategy Simulator

A high-performance algorithm tracker designed to parse a hardcoded adjacency matrix into structural graph structures and visually compare Breadth-First Search (BFS) and Depth-First Search (DFS) pathing sequences.

---

## 📄 Project Overview

This application acts as a visualization sandbox for foundational graph theory traversal mechanisms. The network topology is pre-defined using a hardcoded $V \times V$ adjacency matrix configuration representing an interconnected city infrastructure layout. At runtime, the engine transforms this matrix grid into an in-memory object graph composed of explicitly linked structural vertices.

The simulator runs both search strategies side-by-side using deterministic graph states. It tracks tracking variables like ancestor nodes (Predecessors) and hop offsets (Distance metrics) to illustrate how queues and stacks alter network exploration order.

### Core Objectives
* **Pre-Established Matrix Integration:** Eliminating manual runtime matrix input steps by utilizing a fixed structural topology layout.
* **Structural Search Comparison:** Visually comparing the shallow, leveling exploration pattern of BFS against the deep branch dive of DFS.
* **History Path State Tracking:** Populating distance scales and chronological predecessor tracks to support path reconstruction.

---

## 🛡️ Algorithmic Complexity Profile

### Traversal Matrix Bound Limits
* **Total Time Complexity:** $O(V + E)$ for individual search loops, where $V$ represents Vertices and $E$ represents structural child edges. Setting up the fixed matrix structures on initialization scales at a deterministic quadratic boundary of $O(V^2)$.
* **Total Space Complexity:** $O(V^2)$ baseline memory space, necessary to house the multi-dimensional tracking arrays alongside vertex structures and lookup maps.

---

## 📝 Formal Pseudocode (Traversal Execution Matrix)

This structured English pseudocode maps the search steps executing inside the application:

```text
CLASS Vertex:
    property name
    property isExplored
    property vertexChildren
    property predecessor
    property distance

FUNCTION RunBreadthFirstSearch(startVertex):
    startVertex.isExplored = True
    
    FIFO_Queue = New EmptyQueue()
    FIFO_Queue.Enqueue(startVertex)

    WHILE FIFO_Queue IS NOT empty:
        current = FIFO_Queue.Dequeue()
        Print(current.name, current.predecessor, current.distance)

        FOR EACH neighbor IN current.vertexChildren:
            IF neighbor.isExplored IS False THEN:
                neighbor.isExplored = True
                neighbor.predecessor = current
                neighbor.distance = current.distance + 1
                FIFO_Queue.Enqueue(neighbor)

FUNCTION RunDepthFirstSearch(startVertex):
    startVertex.isExplored = True
    
    LIFO_Stack = New EmptyStack()
    LIFO_Stack.Push(startVertex)

    WHILE LIFO_Stack IS NOT empty:
        current = LIFO_Stack.Pop()
        Print(current.name, current.predecessor, current.distance)

        FOR EACH neighbor IN current.vertexChildren:
            IF neighbor.isExplored IS False THEN:
                neighbor.isExplored = True
                neighbor.predecessor = current
                neighbor.distance = current.distance + 1
                LIFO_Stack.Push(neighbor)
```

---

## 🚀 Execution Instructions

Compile and run this project natively via standard .NET command line tools:
```bash
dotnet new console
dotnet run
```

