# 🚗 Concessionaria CRUD Console

A lightweight C# Console Application demonstrating secure database connectivity and fundamental CRUD (Create, Read, Update, Delete) operations using MySQL and Docker.

---

## 📄 Project Overview

This repository contains a standalone database interaction application built using **C#** and **.NET**. The primary goal of this project is to demonstrate how to safely establish a connection to a relational database, perform standard data manipulations, and follow industry best practices for security and resource management.

In basic database scripts, unmanaged connections can lead to memory leaks, and raw query strings leave applications vulnerable. This project addresses those concerns by emphasizing automated lifecycle management and parameterized queries.

### Core Objectives
* **Data Persistence:** Successfully establishing a connection to a MySQL instance to persist, retrieve, alter, and remove records.
* **SQL Injection Prevention:** Utilizing strict query parameterization to sanitize user inputs and secure database commands.
* **Efficient Resource Management:** Leveraging structural blocks to guarantee that database connections and readers are safely disposed of after execution.

---

## 🛡️ Key Features

1. **Dockerized Environment:** Uses an isolated container instance, completely eliminating the need for a heavy, complex native MySQL local installation.
2. **Parameterized Commands:** Protects the database infrastructure against SQL injection attacks by treating inputs as literal values rather than executable code.
3. **Automated Connection Scope:** Implements C# `using` blocks to automatically close and dispose of database connections, ensuring no lingering connection locks.
4. **Graceful Error Handling:** Wrapped inside robust `try-catch` architectures to cleanly intercept and report database exceptions without crashing the execution environment.

---

## 🛠️ Tech Stack

* **Language:** C# (C-Sharp)
* **Runtime:** .NET SDK
* **Database Driver:** `MySql.Data` NuGet Package
* **Database Engine:** MySQL Server (Deployed via Docker)
* **IDE/Tools:** VS Code & Docker

---

## 🧩 Database Structure

The application interacts with a relational schema representing a car dealership ecosystem (`concessionaria`). The console application targets the following `Cliente` table structure:

```sql
USE concessionaria;

CREATE TABLE Cliente (
    Cliente_ID INT PRIMARY KEY,
    Nome VARCHAR(100),
    Endereco_ID INT
);
```

---

## 🚀 Setup & Execution Guide

### 1. Initialize the Database Container
Ensure Docker Desktop is running on your machine, then execute the following single-line command in your terminal to deploy the pre-configured MySQL instance. 

```powershell
docker run --name concessionaria-db -e MYSQL_ROOT_PASSWORD=supersecretrootpass -e MYSQL_DATABASE=concessionaria -e MYSQL_USER=User1 -e MYSQL_PASSWORD=Password -p 3306:3306 -d mysql:latest
```

### 2. Add Project Source Files & Dependencies
Ensure your code is located inside your `Program.cs` file. Run the following command inside your project directory terminal to attach the relational driver library extensions:

```bash
dotnet new console
dotnet add package MySql.Data
```
*(Note: If you already initialized your `.csproj` file previously, you only need to run the `dotnet add package` command).*

### 3. Launch the Application
Compile and execute the console application with a single command:
```bash
dotnet run
```
