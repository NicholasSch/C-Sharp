# 🕸️ Maximum Weight Independent Set Solver

A high-performance graph optimization engine designed to compute the exact Maximum Weight Independent Set (MWIS) from an adjacency matrix natively in C#.

---

## 📄 Project Overview

This repository features an operational research framework built to solve the Maximum Weight Independent Set problem. Given an input network graph layout where vertices carry individual weight metrics, the objective is to select a subset of nodes maximizing cumulative value such that no two selected vertices share an connecting edge.

The engine relies on a discrete space search framework. It compiles connectivity paths into a fast in-memory boolean lookup matrix, then traverses node choices using a recursive backtracking architecture. The engine includes localized maximum weight suffix caching arrays, allowing it to estimate future yields and prune unviable paths early. It features a file fallback parser; if `entrada.txt` is missing, the application automatically mounts a pre-set 7-vertex sample network to keep execution completely uninterrupted.

### Core Objectives
* **Relational Node Isolation:** Structuring vertex selections so no two elements double-book or conflict across an active edge connection.
* **Deterministic Fallback Routing:** Guaranteeing successful system execution by auto-hydrating memory arrays when file assets are detached.
* **Pruning Optimizations:** Employing suffix weight lookups to instantly eliminate sub-trees that cannot beat our global best score.

---

## 🛡️ Algorithmic Complexity Profile

### Global Performance Bounds
* **Total Time Complexity:** $O(2^V)$ worst-case, where $V$ represents the number of vertices in the network graph. Active branch-and-bound suffix pruning ensures the actual processed search tree stays significantly smaller than the theoretical exponential limit.
* **Total Space Complexity:** $O(V^2)$ auxiliary memory allocations, required to map the structural relational network paths inside an evaluation lookup table.

---

## 📝 Formal Pseudocode (Maximum Weight Independent Set Engine)

This structured English pseudocode maps the verification steps, bounding boundaries, and backtracking branches running inside the machine:

```text
CLASS Edge:
    property leftVertex
    property rightVertex
    property weight

CLASS Graph:
    property numVertices
    property adjacencyMatrix

FUNCTION FindMaximumWeightIndependentSet(graph, weights, currentIdx, currentWeight, currentSet, suffixMaxWeights):
    // Base Step: All vertices processed
    IF currentIdx EQUALS graph.numVertices THEN:
        IF currentWeight > globalMaxIndependentWeight THEN:
            globalMaxIndependentWeight = currentWeight
            globalBestIndependentSet = Clone(currentSet)
        RETURN

    // Bounding Step: Prune branch if the remaining potential weight cannot beat our best score
    IF currentWeight + suffixMaxWeights[currentIdx] <= globalMaxIndependentWeight THEN:
        RETURN

    // Branch 1 Evaluation: Test inclusion of current index node
    canInclude = True
    FOR previousVertex FROM 0 TO currentIdx - 1:
        IF currentSet[previousVertex] IS True AND graph.adjacencyMatrix[currentIdx, previousVertex] IS True THEN:
            canInclude = False
            BREAK Loop

    IF canInclude IS True THEN:
        currentSet[currentIdx] = True
        FindMaximumWeightIndependentSet(graph, weights, currentIdx + 1, currentWeight + weights[currentIdx], currentSet, suffixMaxWeights)
        currentSet[currentIdx] = False // Backtrack trace reset

    // Branch 2 Evaluation: Test complete exclusion of the current index node
    FindMaximumWeightIndependentSet(graph, weights, currentIdx + 1, currentWeight, currentSet, suffixMaxWeights)
```

---

## 🚀 Execution Instructions

Compile and run this project natively via standard .NET command line tools:
```bash
dotnet new console
dotnet run
```
