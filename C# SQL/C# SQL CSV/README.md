# 🛒 Supermarket Data Pipeline Console

A comprehensive C# Console Application demonstrating relational database interactions, transactional workflows, and flat-file data processing using MySQL, Docker, and standard CSV pipelines.

---

## 📄 Project Overview

This repository features an advanced database engine controller built with **C#** and **.NET**. Modeled around a retail supermarket loyalty system, the application handles relational records mapping customer identification records cleanly to localized shipping and billing address profiles. 

Unlike basic singular database actions, this tool shows real-world development workflows by handling batch operations via file streaming utilities, ensuring system state recovery with transactional rollback boundaries, and optimizing cross-table reads using foreign keys.

### Core Objectives
* **Relational Engineering:** Managing relational data records across dependency boundaries using database structural logic (`INNER JOIN`).
* **ACID Integrity Management:** Protecting database operations against partial batch data failures by wrapping execution tasks inside isolated transactions (`MySqlTransaction`).
* **File Stream Integration:** Implementing custom flat-file handlers (`StreamReader`/`StreamWriter`) to parse bulk inbound spreadsheet documents and generate text-delimited files safely.

---

## 🛡️ Key System Architecture Features

1. **Dockerized Environment Infrastructure:** Operates via a fully containerized architecture, allowing quick testing runs without manual environmental variables or local operating system overhead.
2. **Batch Transaction Operations:** Implements structured commit-or-rollback architectures, guaranteeing that database data remains completely clean if bulk operations throw formatting errors mid-process.
3. **Data Hydration & Extraction:** Equipped with structural input/output file handlers designed to cleanly process inbound source CSV rows or build localized reports.
4. **Normalized Database Model Structure:** Employs explicit structural referencing logic linking entities together securely without duplication of critical records.

---

## 🛠️ Tech Stack

* **Language:** C# (C-Sharp)
* **Runtime:** .NET SDK
* **Database Driver:** `MySql.Data` NuGet Package
* **Database Infrastructure Engine:** MySQL Engine Server (Deployed via Docker)
* **IDE/Tools:** VS Code & GitHub Desktop

---

## 🧩 Relational Database Structure

The ecosystem references two linked entities within the `supermercado` relational environment. The console application executes queries assuming the following underlying table relations:

```sql
USE supermercado;

DROP TABLE IF EXISTS Cliente;
DROP TABLE IF EXISTS Endereco;

CREATE TABLE Endereco (
    Endereco_ID INT AUTO_INCREMENT PRIMARY KEY,
    Rua VARCHAR(255) NOT NULL,
    CEP VARCHAR(20) NOT NULL
);

CREATE TABLE Cliente (
    Cliente_ID INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    Endereco_ID INT NOT NULL,
    FOREIGN KEY (Endereco_ID) REFERENCES Endereco(Endereco_ID) ON DELETE CASCADE
);
```

---

## 🚀 Setup & Execution Guide

### 1. Initialize the Database Container
Ensure Docker Desktop is running on your host machine, then run the following single-line command in your terminal shell to spin up the container environment:

```powershell
docker run --name supermercado-db -e MYSQL_ROOT_PASSWORD=supersecretrootpass -e MYSQL_DATABASE=supermercado -e MYSQL_USER=User1 -e MYSQL_PASSWORD=Password -p 3306:3306 -d mysql:latest
```

### 2. Add Project Source Files & Dependencies
Ensure your code is located inside your `Program.cs` file. Run the following command inside your project directory terminal to attach the relational driver library extensions:

```bash
dotnet new console
dotnet add package MySql.Data
```
*(Note: If you already initialized your `.csproj` file previously, you only need to run the `dotnet add package` command).*

### 3. Provide Batch Source Documents (Optional)
To test the bulk data parsing capabilities, make sure to place your spreadsheet resource source objects directly inside the root execution directory alongside `Program.cs`:
* `Supermercado_Clientes.csv`
* `Supermercado_Enderecos.csv`

### 4. Build and Run the Pipeline
Compile and execute the console ecosystem directly from your machine terminal window:
```bash
dotnet run
```
