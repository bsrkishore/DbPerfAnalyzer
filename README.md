# 📘 DbPerfAnalyzer  
### A Developer‑Friendly SQL Server Performance Benchmarking CLI

DbPerfAnalyzer is a lightweight, educational, and highly transparent CLI tool for benchmarking SQL Server performance.  
It demonstrates real SQL Server behaviors such as:

- 🔍 Missing index impact  
- 📊 Table scan performance  
- 🧠 Parameter sniffing  
- ⚡ Plan cache effects  
- 📝 Ad‑hoc query benchmarking  
- 🧪 Full performance suite execution  

This tool is intentionally simple and perfect for learning, debugging, and showcasing SQL Server internals.

---

## ✨ Features

### 🔧 Missing Index Benchmark  
Measure the performance difference before and after adding an index.

```bash
dbperf missing-index --table LargeTable --column CustomerId
```

---

### 📂 Table Scan Benchmark  
Measure raw I/O performance by scanning an entire table.

```bash
dbperf table-scan --table LargeTable
```

---

### 🧠 Parameter Sniffing Test  
Run a stored procedure with a single parameter to observe sniffing behavior.

```bash
dbperf sniff --value 1
```

---

### ⚖️ Sniff‑Diff (Rare vs Common Values)  
Compare cached vs isolated execution plans — the clearest way to demonstrate parameter sniffing.

```bash
dbperf sniff-diff --rare 1 --common 9999
```

---

### 📝 Query Benchmark  
Benchmark **any** SQL query or stored procedure.

```bash
dbperf query --sql "SELECT TOP 1000 * FROM LargeTable"
```

---

### 🧪 Full Performance Suite  
Run all tests in one command.

```bash
dbperf all
```

---

### ❓ Help System  
Built‑in help for all commands.

```bash
dbperf help
dbperf help sniff-diff
```

---

## 🏗️ Architecture Overview

```
/DbPerfAnalyzer
  /Tests
    MissingIndexTest.cs
    TableScanTest.cs
    ParameterSniffingTest.cs
    QueryBenchmarkTest.cs
  SqlHelper.cs
  Program.cs (CLI Router)
  Config.cs
```

### Design Principles

- No external dependencies  
- Simple argument parsing  
- Modular test classes  
- Reusable timing engine  
- Clear, readable output  
- Easy to extend  

---

## 📊 Example Output (Sniff‑Diff)

```
=== Natural Sniffing (No Cache Clear) ===
Rare (1):            359 ms
Common (9999):       6112 ms

=== Controlled Runs (Cache Cleared Between Each) ===
Rare isolated (1):     5 ms
Common isolated (9999): 5932 ms

=== Summary ===
Natural difference:   5753 ms
Isolated difference:  5927 ms
```

This demonstrates:

- How SQL Server caches execution plans  
- How the first parameter value can “poison” performance  
- How isolated runs reveal the true cost of each value  

---

## ⚙️ Configuration

Update your connection string in `Config.cs`:

```csharp
public static string ConnectionString =>
    "Server=localhost;Database=PerfTestDB;Trusted_Connection=True;TrustServerCertificate=True;";
```

---

## 🧰 Prerequisites

- .NET 8 SDK  
- SQL Server (local or remote)  
- A test database (PerfTestDB)  
- A table named `LargeTable` with skewed data for sniffing tests  

### Example Schema

```sql
CREATE TABLE LargeTable (
    Id INT IDENTITY PRIMARY KEY,
    CustomerId INT,
    Amount DECIMAL(10,2)
);
```

---

## 🧪 Creating Skewed Data (for Sniffing Tests)

```sql
TRUNCATE TABLE LargeTable;

-- Rare values
INSERT INTO LargeTable (CustomerId, Amount)
SELECT 1, RAND() * 1000
FROM sys.all_objects
WHERE object_id % 5000 = 0;

-- Common values
INSERT INTO LargeTable (CustomerId, Amount)
SELECT 9999, RAND() * 1000
FROM sys.all_objects a
CROSS JOIN sys.all_objects b;
```

---

## 🧩 Extending the CLI

Adding a new command is simple:

1. Add a new test class in `/Tests`  
2. Add a new case in the router  
3. Implement the handler  

---

## 🎯 Why This Project Exists

DbPerfAnalyzer was built to:

- Understand SQL Server internals  
- Demonstrate real performance behaviors  
- Provide a hands‑on learning tool  
- Help debug real production issues  

---

## 📄 License

MIT License