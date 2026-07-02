# 🕸️ Graph Resource Discovery & Routing Simulator

A high-performance .NET simulation framework designed to model, analyze, and benchmark decentralized asset discovery strategies over complex, undirected topological networks. 

This repository contains an in-memory network routing engine that evaluates the trade-offs between sequential traversal techniques and asynchronous multithreaded broadcast patterns under strict exploration horizons.

---

## 📄 What This Software Does

At its core, this software simulates a decentralized network where nodes function as autonomous routing entities with no central lookup index or global directory server. Each individual node in the graph possesses a collection of local assets (Resources) and a localized memory block (Cache) used to record path shortcuts to resources discovered across the network during runtime operations.

The application reads a custom flat-file topology graph definition template (`input.txt`), maps out the vertices and bidirectional edge boundaries, verifies structural connectivity constraints, and launches an interactive environment where users can dispatch localized search queries. 

Queries are bounded by a **Time-To-Live (TTL)** horizon parameter that emulates packet drop metrics in real-world distributed networking protocols. The system measures search effectiveness using three concrete data counters:
1. **Search Status:** Explicit success or failure resolution within the designated TTL radius.
2. **Message Overhead:** Total number of traversal messages dispatched across edges (network traffic load).
3. **Graph Node Accesses:** Total number of independent vertex frames inspected by the execution path (CPU computational load).

---

## 🎯 Project Intention: Why This Is a Good Demonstration

This framework is built as an educational and engineering reference tool designed to showcase how foundational graph algorithms evolve when modified to solve distributed, real-time synchronization problems. It provides a solid practical demonstration of several core computer science domains:

### 1. Algorithm Evolution & Optimization Continuum
The software demonstrates a clear optimization path across its four algorithms. It showcases how a system progresses from a completely blind sequential search (`Random Walk`) to an intelligent shortcut-driven search (`Random Informed Walk`). It then jumps into structural parallelization (`Flooding`), and finishes with a hybrid concurrent optimization (`Informed Flooding`). This layout serves as an excellent case study in how small, localized metadata caches can radically reduce global computational complexity and network messaging overhead.

### 2. Multi-Threaded Concurrency vs. Sequential Realities
Many classic graph traversal algorithms (such as standard DFS or BFS) are traditionally written as sequential, single-threaded operations. This simulator demonstrates how to parallelize graph traversal over a shared topological structure. It directly contrasts the low-resource, higher-latency characteristics of single-threaded linear walks against the high-throughput, low-latency execution waves of asynchronous thread-pool tasks.

### 3. Practical Multi-Threaded Synchronization Challenges
The flooding algorithms serve as a strong demonstration of thread safety, blocking primitives, and atomic data primitives under concurrent pressure:
* **Countdown Barriers:** Standard multi-threaded routines often fall into the trap of using arbitrary delays (like `Thread.Sleep`) to await task completion, causing race conditions. This framework replaces that anti-pattern with an elegant, deterministic synchronization primitive (`CountdownEvent`), ensuring the main thread blocks safely until all spawned sub-tasks complete.
* **Atomic Telemetry:** Telemetry counters are modified concurrently across multiple thread contexts using low-overhead `Interlocked` operations, preventing data loss and write skew without relying on heavy lock objects.
* **Critical Section Containment:** When a background task locates a resource, it updates the initial node's localized cache map. Because multiple thread lanes can attempt to write to this standard `List<T>` simultaneously, access is synchronized via an explicit mutual-exclusion `lock` primitive, illustrating precise thread boundary protection.

---

## 📂 Detailed Algorithm Breakdowns

### 1. Random Walk (Bounded Randomized DFS)
* **Execution Style:** Sequential, single-threaded.
* **Behavior:** Operates as a Depth-First Search with an added randomization factor. At each vertex, the engine checks local resources. If a miss occurs, it shuffles the array order of its unvisited neighbors to remove layout biases and proceeds down a singular path. If it encounters a network loop or dead end, it backtracks up the stack to try a different neighbor.
* **Termination:** Ends immediately upon matching the resource, when the path depth exceeds the maximum TTL limit, or when all accessible vertices in the connected component have been visited.

### 2. Random Informed Walk
* **Execution Style:** Sequential, single-threaded.
* **Behavior:** Processes step-by-step through a linear path loop. At each node, it follows a strict verification order: first, it checks local resources, and second, it checks local cached indices. If a cache hit occurs, the algorithm skips blind exploration and jumps directly to the vertex holding the target asset. If a cache miss occurs, it falls back to picking a random neighbor edge.
* **Termination:** Resolves successfully on a direct hit or cache shortcut resolution, and fails if the step count runs past the path TTL threshold.

### 3. Multithreaded Flooding (Parallel BFS)
* **Execution Style:** Asynchronous, concurrent.
* **Behavior:** Simulates a parallel Breadth-First Search broadcast wave. The calling thread drops an initial task onto the .NET `ThreadPool`. Upon activation, that task evaluates local resources. If it hits a miss, it increments the shared network message count and spawns independent asynchronous sub-tasks for every single unvisited neighbor simultaneously.
* **Termination:** All concurrent background operations cease execution immediately when any worker thread sets a shared `volatile` global success flag.

### 4. Informed Flooding
* **Execution Style:** Asynchronous, concurrent.
* **Behavior:** A parallel broadcast strategy optimized by routing shortcut indexes. Each independent task thread checks local resources and its cached network maps before spawning sub-tasks. If it encounters a cache hit, it completes the discovery loop instantly and prevents the system from spinning up redundant background thread branches.
* **Termination:** Ends on atomic flag changes or when all parallel paths hit their TTL depth limits.

---

## 📊 Computational Complexity Matrix

Let $V$ represent the total number of Vertices (Nodes), $E$ represent the total number of Edges (Connections), and $k$ represent the maximum allowed degree boundary parameter limit (`max_neighbors`).

| Discovery Strategy Pattern | Time Complexity (Worst-Case) | Space Complexity (Worst-Case) | Message Overhead Complexity |
|:---------------------------|:----------------------------:|:-----------------------------:|:---------------------------:|
| 🏃 **Random Walk** | $O(V + E)$                   | $O(V)$                        | $O(E)$                      |
| 🚶 **Random Informed Walk** | $O(\text{TTL})$              | $O(1)$                        | $O(\text{TTL})$             |
| 🌊 **Flooding** | $O(V + E)$                   | $O(V)$                        | $O(E \cdot k)$              |
| ⚡ **Informed Flooding** | $O(V + E)$                   | $O(V)$                        | $O(E \cdot k)$              |

### Matrix Complexity Diagnostics
* **Time Complexity Analysis:** While the absolute graph bounds restrict search operations to a ceiling of $O(V+E)$, the practical execution path is tightly bound by the maximum path depth limit ($O(\text{TTL})$). Flooding approaches process the network structure significantly faster because branches execute in parallel across independent thread frames, though they consume more aggregate CPU clock cycles processing overlapping links.
* **Space Complexity Analysis:** Sequential walks run with minimal resource overhead ($O(1)$ stack allocations for iterative loops, and up to $O(V)$ for recursive states). In contrast, concurrent flooding strategies require up to $O(V)$ space to manage concurrent state dictionaries, active queues, and the execution stack frames inside the .NET ThreadPool.
* **Network Message Overhead:** Unoptimized flooding triggers substantial processing overhead ($O(E \cdot k)$) due to duplicate broadcast packets crossing common links. The informed variations mitigate this overhead by leveraging local caches to truncate redundant execution branches.

---

## 📝 Core Engine Integrated Pseudocode

### 1. Random Walk (Bounded Randomized DFS)
```text
FUNCTION RandomWalk(InitialNode, TargetResource, MaxTTL)
    Initialize VisitedSet
    Initialize MessagesSentCount = 0
    Initialize NodesAccessedCount = 0

    FUNCTION DepthFirstTraversal(CurrentNode, CurrentTTL)
        Add CurrentNode to VisitedSet
        Increment(NodesAccessedCount)

        FOR EACH ResourceEntry IN CurrentNode.Resources DO
            IF ResourceEntry.Id EQUALS TargetResource THEN
                RETURN CurrentNode
            END IF
        END FOR

        IF CurrentTTL <= 0 THEN
            RETURN Null
        END IF

        UnvisitedNeighbors = GetNeighbors(CurrentNode) NOT IN VisitedSet
        ShuffleRandomly(UnvisitedNeighbors)

        FOR EACH Neighbor IN UnvisitedNeighbors DO
            Increment(MessagesSentCount)
            Result = DepthFirstTraversal(Neighbor, CurrentTTL - 1)
            IF Result IS NOT Null THEN
                RETURN Result
            END IF
        END FOR

        RETURN Null
    END FUNCTION

    ResultNode = DepthFirstTraversal(InitialNode, MaxTTL)
    RETURN (ResultNode, MessagesSentCount, NodesAccessedCount)
END FUNCTION
```

### 2. Random Informed Walk
```text
FUNCTION RandomInformedWalk(InitialNode, TargetResource, MaxTTL)
    Initialize CurrentNode = InitialNode
    Initialize MessagesSentCount = 0
    Initialize NodesAccessedCount = 0
    Initialize StepsTaken = 0

    WHILE StepsTaken LESS THAN MaxTTL DO
        Increment(NodesAccessedCount)

        // 1. Check local resources directly
        FOR EACH ResourceEntry IN CurrentNode.Resources DO
            IF ResourceEntry.Id EQUALS TargetResource THEN
                IF CurrentNode NOT EQUALS InitialNode THEN
                    AcquireLock(InitialNode.CacheLock)
                    Add TargetResource and CurrentNode to InitialNode.Cache
                    ReleaseLock(InitialNode.CacheLock)
                END IF
                RETURN (CurrentNode, MessagesSentCount, NodesAccessedCount)
            END IF
        END FOR

        // 2. Evaluate local routing cache indexes
        AcquireLock(CurrentNode.CacheLock)
        FOR EACH CachedEntry IN CurrentNode.Cache DO
            IF CachedEntry.Id EQUALS TargetResource THEN
                IF CurrentNode NOT EQUALS InitialNode THEN
                    AcquireLock(InitialNode.CacheLock)
                    Add TargetResource and CachedEntry.Node to InitialNode.Cache
                    ReleaseLock(InitialNode.CacheLock)
                END IF
                ReleaseLock(CurrentNode.CacheLock)
                RETURN (CachedEntry.Node, MessagesSentCount, NodesAccessedCount)
            END IF
        END FOR
        ReleaseLock(CurrentNode.CacheLock)

        // 3. Fallback routing to blind random neighbor
        IF CurrentNode.Edges.Count EQUALS 0 THEN
            RETURN (Null, MessagesSentCount, NodesAccessedCount)
        END IF

        Increment(MessagesSentCount)
        RandomIndex = GenerateRandomInteger(0, CurrentNode.Edges.Count)
        CurrentNode = CurrentNode.Edges[RandomIndex].Target
        Increment(StepsTaken)
    END WHILE

    RETURN (Null, MessagesSentCount, NodesAccessedCount)
END FUNCTION
```

### 3. Multithreaded Flooding Search (Concurrent Parallel BFS)
```text
FUNCTION FloodingMultithreaded(OriginNode, TargetResource, MaxTTL)
    Initialize ConcurrentVisitedMap
    Initialize ResourceFoundFlag = False
    Initialize TaskCounter Barrier
    Initialize MessagesSentCount = 0
    Initialize NodesAccessedCount = 0
    Initialize ResultNode = Null

    FUNCTION BroadcastTask(CurrentNode, CurrentTTL)
        TRY
            IF ResourceFoundFlag IS True OR CurrentNode is inside ConcurrentVisitedMap THEN
                RETURN
            END IF

            Add CurrentNode to ConcurrentVisitedMap
            AtomicallyIncrement(NodesAccessedCount)

            FOR EACH ResourceEntry IN CurrentNode.Resources DO
                IF ResourceEntry.Id EQUALS TargetResource THEN
                    ResourceFoundFlag = True
                    ResultNode = CurrentNode
                    RETURN
                END IF
            END FOR

            IF CurrentTTL <= 0 OR ResourceFoundFlag IS True THEN
                RETURN
            END IF

            FOR EACH Edge IN CurrentNode.Edges DO
                Neighbor = Edge.Target
                IF Neighbor is NOT inside ConcurrentVisitedMap AND ResourceFoundFlag IS False THEN
                    AtomicallyIncrement(MessagesSentCount)
                    IncrementBarrier(TaskCounter)
                    QueueToThreadPool(BroadcastTask(Neighbor, CurrentTTL - 1))
                END IF
            END FOR
        FINALLY
            DecrementBarrier(TaskCounter)
        END TRY
    END FUNCTION

    IncrementBarrier(TaskCounter)
    QueueToThreadPool(BroadcastTask(OriginNode, MaxTTL))
    BlockUntilZero(TaskCounter)

    RETURN (ResultNode, MessagesSentCount, NodesAccessedCount)
END FUNCTION
```

### 4. Informed Flooding Search (Concurrent Caching BFS)
```text
FUNCTION FloodingInformedMultithreaded(OriginNode, TargetResource, MaxTTL)
    Initialize ConcurrentVisitedMap
    Initialize ResourceFoundFlag = False
    Initialize TaskCounter Barrier
    Initialize MessagesSentCount = 0
    Initialize NodesAccessedCount = 0
    Initialize ResultNode = Null

    FUNCTION BroadcastTask(CurrentNode, CurrentTTL)
        TRY
            IF ResourceFoundFlag IS True OR CurrentNode is inside ConcurrentVisitedMap THEN
                RETURN
            END IF

            Add CurrentNode to ConcurrentVisitedMap
            AtomicallyIncrement(NodesAccessedCount)

            // 1. Direct local resource verification pass
            FOR EACH ResourceEntry IN CurrentNode.Resources DO
                IF ResourceEntry.Id EQUALS TargetResource THEN
                    ResourceFoundFlag = True
                    ResultNode = CurrentNode
                    IF CurrentNode NOT EQUALS OriginNode THEN
                        AcquireLock(OriginNode.CacheLock)
                        Add TargetResource and CurrentNode to OriginNode.Cache
                        ReleaseLock(OriginNode.CacheLock)
                    END IF
                    RETURN
                END IF
            END FOR

            // 2. Local index cache optimization check
            AcquireLock(CurrentNode.CacheLock)
            FOR EACH CachedEntry IN CurrentNode.Cache DO
                IF CachedEntry.Id EQUALS TargetResource THEN
                    ResourceFoundFlag = True
                    ResultNode = CachedEntry.Node
                    IF CurrentNode NOT EQUALS OriginNode THEN
                        AcquireLock(OriginNode.CacheLock)
                        Add TargetResource and CachedEntry.Node to OriginNode.Cache
                        ReleaseLock(OriginNode.CacheLock)
                    END IF
                    ReleaseLock(CurrentNode.CacheLock)
                    RETURN
                END IF
            END FOR
            ReleaseLock(CurrentNode.CacheLock)

            IF CurrentTTL <= 0 OR ResourceFoundFlag IS True THEN
                RETURN
            END IF

            // 3. Neighbor broadcast distribution step
            FOR EACH Edge IN CurrentNode.Edges DO
                Neighbor = Edge.Target
                IF Neighbor is NOT inside ConcurrentVisitedMap AND ResourceFoundFlag IS False THEN
                    AtomicallyIncrement(MessagesSentCount)
                    IncrementBarrier(TaskCounter)
                    QueueToThreadPool(BroadcastTask(Neighbor, CurrentTTL - 1))
                END IF
            END FOR
        FINALLY
            DecrementBarrier(TaskCounter)
        END TRY
    END FUNCTION

    IncrementBarrier(TaskCounter)
    QueueToThreadPool(BroadcastTask(OriginNode, MaxTTL))
    BlockUntilZero(TaskCounter)

    RETURN (ResultNode, MessagesSentCount, NodesAccessedCount)
END FUNCTION
```

---

## 📂 Project Architecture Layout

The source workspace files are organized into decoupled operational segments:

```text
GraphResourceDiscovery/
│
├── Models/
│   ├── Node.cs                 # Vertices data structure & Cache isolation locks
│   └── Graph.cs                # Global topological mapping descriptor schema
│
├── Core/
│   ├── Algorithms.cs           # Thread-safe graph routing and search operations
│   └── GraphBuilder.cs         # Flat-file parser, connectivity rules & error checker
│
├── Program.cs                  # Interactive User Console runtime loop interface
└── input.txt                   # Topology layout file containing nodes and edges
```

---

## 🚀 Execution Instructions

Compile and run this project natively via standard .NET command line tools:
```bash
dotnet new console
dotnet run
```
