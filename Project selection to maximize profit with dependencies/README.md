# 📊 Project Selection Dependency Pipeline Solver

An optimized operational research engine designed to solve the Project Selection Problem with precedence constraints natively in C# using graph transitive closures and branch-and-bound backtracking.

---

## 📄 Project Overview

This project tackles the Project Selection Problem with Precedence Constraints. Given a graph layout where vertex nodes represent potential corporate projects (each carrying a positive revenue yield or a negative operational cost) and directed edges represent strict dependency requirements, the objective is to find a closure subset that maximizes overall net profit.

The core engine uses a two-phase optimization process:
1. **Transitive Closure Generation:** It maps out the full tree of downstream dependencies for each project using a Breadth-First Search (BFS) module. If any project transitively relies on itself, the topology is flagged as invalid.
2. **Backtracking Closure Selector:** A recursive pruning matrix evaluates valid combination choices, automatically pulling along all prerequisite tasks when a high-value project is activated.

The simulator also includes a local file fallback tracking architecture. If the primary spreadsheet source (`Project.txt`) is detached, the framework automatically imports a pre-established 6-node network layout to ensure uninterrupted deployment.

---

## 🛡️ Algorithmic Complexity Profile

### Optimization Framework Bound Limits
* **Total Time Complexity:** $O(2^V)$ worst-case, where $V$ represents the number of candidate projects. Pre-calculating transitive dependency sets via BFS runs efficiently in $O(V \cdot (V + E))$ time prior to starting the combinatorial tree traversal.
* **Total Space Complexity:** $O(V^2)$ auxiliary memory structures, required to cache deep copies of the dependency closure chains and track active selection states across recursive call frames.

---

## 📝 Formal Pseudocode (Project Dependency Selection Engine)

This structured English pseudocode maps the dependency parsing loops and exact closure selections running inside the system:

```text
CLASS Edge:
    property source
    property destination
    property weight

CLASS Graph:
    property numVertices
    property vertexWeights
    property dependencyAdjacencyList

FUNCTION ComputeTransitiveClosureBFS(rootVertex, graph):
    visitedDependencies = New EmptyList()
    FIFO_Queue = New EmptyQueue()
    FIFO_Queue.Enqueue(rootVertex)

    WHILE FIFO_Queue IS NOT empty:
        current = FIFO_Queue.Dequeue()
        IF current NOT IN visitedDependencies THEN:
            Append current to visitedDependencies
            
            FOR EACH neighbor IN graph.dependencyAdjacencyList[current]:
                // Check for cyclic dependencies returning to the root node
                IF neighbor EQUALS rootVertex THEN:
                    RETURN (visitedDependencies, IsCycleFree = False)
                FIFO_Queue.Enqueue(neighbor)
                
    RETURN (visitedDependencies, IsCycleFree = True)


FUNCTION SolveProjectSelection(graph, closures, projectIdx, currentSelection):
    // Base Case: All potential options evaluated
    IF projectIdx EQUALS graph.numVertices THEN:
        currentProfit = Sum(graph.vertexWeights[i] FOR EACH i WHERE currentSelection[i] IS True)
        IF currentProfit > globalMaxTotalProfit THEN:
            globalMaxTotalProfit = currentProfit
            globalOptimalProjectSet = GetActiveIndices(currentSelection)
        RETURN

    // Branch 1: Try selecting this project (forcing activation of its complete closure)
    savedStateBackup = Clone(currentSelection)
    validSelectionRule = True

    FOR EACH requiredItem IN closures[projectIdx]:
        currentSelection[requiredItem] = True

    // Verification Step: Ensure closure integrity holds across all selected items
    FOR EACH selectedNode IN graph:
        IF currentSelection[selectedNode] IS True THEN:
            FOR EACH dependency IN closures[selectedNode]:
                IF currentSelection[dependency] IS False THEN:
                    validSelectionRule = False

    IF validSelectionRule IS True THEN:
        SolveProjectSelection(graph, closures, projectIdx + 1, currentSelection)

    // Backtrack Step: Revert state
    currentSelection = Clone(savedStateBackup)

    // Branch 2: Skip selecting this project at this decision node
    SolveProjectSelection(graph, closures, projectIdx + 1, currentSelection)
```

---

## 🚀 Execution Instructions

Compile and execute this dependency solver natively using standard .NET command line utility tools:
```bash
dotnet new console
dotnet run
```
