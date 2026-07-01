# 📈 Greedy Project Selection Optimizer

A lightweight C# optimization simulator implementing a Greedy Algorithm strategy to maximize business profits by resolving resource allocation scheduling without timeline overlaps.

---

## 📄 Project Overview

This system handles a variant of the classical Interval Scheduling Maximization Problem. Given a collection of standalone business ventures or corporate assignments containing custom implementation schedules and projected financial yields, the execution matrix isolates the most profitable combinations possible.

The application operates on a strict **Greedy Choice Property**. It evaluates options by sorting allocations based on raw immediate yields, choosing the highest earners first, and systematically dropping subsequent entries that present logistical schedule collisions.

### Core Objectives
* **Profit Maximization:** Isolating non-overlapping scheduling configurations to secure high monetary returns.
* **Conflict Resolution:** Computing real-time date intersection boundaries to make sure an enterprise doesn't double-book operational timelines.
* **Algorithmic Profiling:** Documenting structural line-by-line resource costs across standard collections.

---

## 🛡️ Algorithmic Complexity Profile

### Global Bound Limits
* **Total Time Complexity:** O(N^2) worst-case. While ordering the dataset takes a quick logarithmic baseline of O(N log N), checking for scheduling overlaps uses a nested loop design. Evaluating every element against the growing list of accepted projects yields an execution profile that resolves to quadratic scales.
* **Total Space Complexity:** O(N) auxiliary memory space, required to store the sorted reference indices and hold the final filtered output collection safely.

---

## 📝 Formal Pseudocode (Greedy Scheduler Engine)

This structured English pseudocode maps the timeline validation loops and sorting choices executing behind the system:

```text
CLASS Project:
    property startDate
    property endDate
    property profit

FUNCTION SelectMaxProfitProjects(projectList):
    // Greedy Choice: Sort collections prioritizing descending profit margins
    sortedList = SortByProfitDescending(projectList)
    
    selectedProjects = New EmptyList()

    FOR EACH project IN sortedList:
        conflictDetected = False
        
        // Inner loop scan for calendar collisions
        FOR EACH approved IN selectedProjects:
            IF (project.startDate < approved.endDate) AND (project.endDate > approved.startDate) THEN:
                conflictDetected = True
                Break inner loop
                
        // If the schedule timeline is free, the project is permanently accepted
        IF conflictDetected IS False THEN:
            selectedProjects.Append(project)

    RETURN selectedProjects 
```

---

## 🚀 Execution Instructions

Compile and execute this performance tracker directly from your machine terminal window:
```bash
dotnet new console
dotnet run
```
