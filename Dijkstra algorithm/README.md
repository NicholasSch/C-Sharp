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

This structured pseudocode covers both the calculation stage and the route backtracking array build logic:

```text
CLASSE Cidade:
    propriedade nome
    propriedade menor_tempo_ate_aqui
    propriedade explorada
    propriedade lista_de_arestas
    propriedade cidade_anterior

CLASSE Aresta:
    propriedade cidade_destino
    propriedade tempo_caminho

FUNÇÃO CalcularCaminhoMinimo(cidade_inicial):
    Para cada cidade no mapa:
        cidade.menor_tempo_ate_aqui = Infinito
        cidade.explorada = Falso
        cidade.cidade_anterior = Nulo

    cidade_inicial.menor_tempo_ate_aqui = 0
    FilaMinima = Nova FilaDePrioridade()
    FilaMinima.Inserir(cidade_inicial, 0)

    ENQUANTO FilaMinima NÃO estiver vazia:
        cidade_atual = FilaMinima.ExtrairMinimo()
        SE cidade_atual.explorada SEJA Verdadeiro: Continuar
        cidade_atual.explorada = Verdadeiro

        PARA CADA aresta EM cidade_atual.lista_de_arestas:
            vizinho = aresta.cidade_destino
            SE vizinho.explorada NÃO SEJA Verdadeiro:
                tempo_calculado = cidade_atual.menor_tempo_ate_aqui + aresta.tempo_caminho
                SE tempo_calculado < vizinho.menor_tempo_ate_aqui:
                    vizinho.menor_tempo_ate_aqui = tempo_calculado
                    vizinho.cidade_anterior = cidade_atual
                    FilaMinima.Inserir(vizinho, tempo_calculado)

FUNÇÃO ReconstruirRota(cidade_destino):
    ListaCaminho = Nova ListaVazia()
    atual = cidade_destino
    
    ENQUANTO atual NÃO FOR Nulo:
        ListaCaminho.AdicionarNoFinal(atual.nome)
        atual = atual.cidade_anterior
        
    ListaCaminho.InverterOrdem()
    Retornar ListaCaminho
```

---

## 🚀 Execution Instructions

Compile and run this project using standard CLI tools:
```bash
dotnet new console
dotnet run
```
