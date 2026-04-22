if (args.Length == 0)
{
    Console.WriteLine("dbperf: No command provided.");
    Console.WriteLine("Usage: dbperf <command> [options]");
    return;
}

var command = args[0].ToLower();

switch (command)
{
    case "missing-index":
        RunMissingIndex(args);
        break;

    case "table-scan":
    RunTableScan(args);
    break;

    case "sniff":
    RunSniff(args);
    break;

    case "sniff-diff":
    RunSniffDiff(args);
    break;

    case "query":
    RunQuery(args);
    break;

    case "all":
    RunAll(args);
    break;

    case "help":
    RunHelp(args);
    break;



    default:
        Console.WriteLine($"dbperf: Unknown command '{command}'");
        break;
}

void RunMissingIndex(string[] args)
{
    string? table = null;
    string? column = null;

    // Parse flags
    for (int i = 1; i < args.Length; i++)
    {
        if (args[i] == "--table" && i + 1 < args.Length)
            table = args[i + 1];

        if (args[i] == "--column" && i + 1 < args.Length)
            column = args[i + 1];
    }

    if (table == null || column == null)
    {
        Console.WriteLine("Usage: dbperf missing-index --table <table> --column <column>");
        return;
    }

    var test = new MissingIndexTest(Config.ConnectionString);
    long ms = test.Run(table, column);

    Console.WriteLine($"Missing Index Test completed in {ms} ms");
}

void RunTableScan(string[] args)
{
    string? table = null;

    for (int i = 1; i < args.Length; i++)
    {
        if (args[i] == "--table" && i + 1 < args.Length)
            table = args[i + 1];
    }

    if (table == null)
    {
        Console.WriteLine("Usage: dbperf table-scan --table <table>");
        return;
    }

    var test = new TableScanTest(Config.ConnectionString);
    long ms = test.Run(table);

    Console.WriteLine($"Table Scan Test completed in {ms} ms");
}

void RunSniff(string[] args)
{
    int? customerId = null;

    for (int i = 1; i < args.Length; i++)
    {
        if (args[i] == "--value" && i + 1 < args.Length)
            customerId = int.Parse(args[i + 1]);
    }

    if (customerId == null)
    {
        Console.WriteLine("Usage: dbperf sniff --value <CustomerId>");
        return;
    }

    var test = new ParameterSniffingTest(Config.ConnectionString);
    long ms = test.Run(customerId.Value);

    Console.WriteLine($"Parameter Sniffing Test completed in {ms} ms");
}

void RunSniffDiff(string[] args)
{
    int? rare = null;
    int? common = null;

    for (int i = 1; i < args.Length; i++)
    {
        if (args[i] == "--rare" && i + 1 < args.Length)
            rare = int.Parse(args[i + 1]);

        if (args[i] == "--common" && i + 1 < args.Length)
            common = int.Parse(args[i + 1]);
    }

    if (rare == null || common == null)
    {
        Console.WriteLine("Usage: dbperf sniff-diff --rare <value> --common <value>");
        return;
    }

    var test = new ParameterSniffingTest(Config.ConnectionString);

    Console.WriteLine("=== Natural Sniffing (No Cache Clear) ===");

    long naturalRare = test.Run(rare.Value);
    long naturalCommon = test.Run(common.Value);

    Console.WriteLine($"Rare ({rare}):   {naturalRare} ms");
    Console.WriteLine($"Common ({common}): {naturalCommon} ms");

    Console.WriteLine();
    Console.WriteLine("=== Controlled Runs (Cache Cleared Between Each) ===");

    SqlHelper.ClearPlanCache(Config.ConnectionString);
    long isolatedRare = test.Run(rare.Value);

    SqlHelper.ClearPlanCache(Config.ConnectionString);
    long isolatedCommon = test.Run(common.Value);

    Console.WriteLine($"Rare isolated ({rare}):   {isolatedRare} ms");
    Console.WriteLine($"Common isolated ({common}): {isolatedCommon} ms");

    Console.WriteLine();
    Console.WriteLine("=== Summary ===");

    Console.WriteLine($"Natural difference:   {naturalCommon - naturalRare} ms");
    Console.WriteLine($"Isolated difference:  {isolatedCommon - isolatedRare} ms");
}

void RunQuery(string[] args)
{
    string? sql = null;

    for (int i = 1; i < args.Length; i++)
    {
        if (args[i] == "--sql" && i + 1 < args.Length)
            sql = args[i + 1];
    }

    if (sql == null)
    {
        Console.WriteLine("Usage: dbperf query --sql \"<SQL statement>\"");
        return;
    }

    var test = new QueryBenchmarkTest(Config.ConnectionString);
    long ms = test.Run(sql);

    Console.WriteLine($"Query Benchmark completed in {ms} ms");
}

void RunAll(string[] args)
{
    Console.WriteLine("=== Running Full Performance Suite ===");
    Console.WriteLine();

    // 1. Missing Index Test
    Console.WriteLine("[1] Missing Index Test");
    var missingIndex = new MissingIndexTest(Config.ConnectionString);
    long missingIndexMs = missingIndex.Run("LargeTable", "CustomerId");
    Console.WriteLine($"Result: {missingIndexMs} ms");
    Console.WriteLine();

    // 2. Table Scan Test
    Console.WriteLine("[2] Table Scan Test");
    var tableScan = new TableScanTest(Config.ConnectionString);
    long tableScanMs = tableScan.Run("LargeTable");
    Console.WriteLine($"Result: {tableScanMs} ms");
    Console.WriteLine();

    // 3. Sniff-Diff Test
    Console.WriteLine("[3] Parameter Sniffing Test (Sniff-Diff)");
    var sniff = new ParameterSniffingTest(Config.ConnectionString);

    // Natural sniffing
    long naturalRare = sniff.Run(1);
    long naturalCommon = sniff.Run(9999);

    // Isolated runs
    SqlHelper.ClearPlanCache(Config.ConnectionString);
    long isolatedRare = sniff.Run(1);

    SqlHelper.ClearPlanCache(Config.ConnectionString);
    long isolatedCommon = sniff.Run(9999);

    Console.WriteLine($"Natural rare (1):     {naturalRare} ms");
    Console.WriteLine($"Natural common (9999): {naturalCommon} ms");
    Console.WriteLine($"Isolated rare (1):     {isolatedRare} ms");
    Console.WriteLine($"Isolated common (9999): {isolatedCommon} ms");
    Console.WriteLine();

    // 4. Summary
    Console.WriteLine("=== Summary ===");
    Console.WriteLine($"Missing Index: {missingIndexMs} ms");
    Console.WriteLine($"Table Scan:    {tableScanMs} ms");
    Console.WriteLine($"Sniff natural difference:   {naturalCommon - naturalRare} ms");
    Console.WriteLine($"Sniff isolated difference:  {isolatedCommon - isolatedRare} ms");

    Console.WriteLine();
    Console.WriteLine("Full suite completed.");
}

void RunHelp(string[] args)
{
    // If no specific command is requested
    if (args.Length == 1)
    {
        Console.WriteLine("dbperf - SQL Performance Benchmarking Tool");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  dbperf <command> [options]");
        Console.WriteLine();
        Console.WriteLine("Available commands:");
        Console.WriteLine("  missing-index   Benchmark missing index impact");
        Console.WriteLine("  table-scan      Measure raw table scan performance");
        Console.WriteLine("  sniff           Run stored procedure with one parameter");
        Console.WriteLine("  sniff-diff      Compare rare vs common parameter values");
        Console.WriteLine("  query           Benchmark any SQL query");
        Console.WriteLine("  all             Run full performance suite");
        Console.WriteLine("  help            Show help for a command");
        Console.WriteLine();
        Console.WriteLine("Run 'dbperf help <command>' for details.");
        return;
    }

    // Specific command help
    string cmd = args[1].ToLower();

    switch (cmd)
    {
        case "missing-index":
            Console.WriteLine("missing-index");
            Console.WriteLine("Usage: dbperf missing-index --table <table> --column <column>");
            Console.WriteLine("Description: Measures performance before/after adding an index.");
            break;

        case "table-scan":
            Console.WriteLine("table-scan");
            Console.WriteLine("Usage: dbperf table-scan --table <table>");
            Console.WriteLine("Description: Measures raw table scan performance.");
            break;

        case "sniff":
            Console.WriteLine("sniff");
            Console.WriteLine("Usage: dbperf sniff --value <CustomerId>");
            Console.WriteLine("Description: Runs stored procedure once to show sniffing behavior.");
            break;

        case "sniff-diff":
            Console.WriteLine("sniff-diff");
            Console.WriteLine("Usage: dbperf sniff-diff --rare <value> --common <value>");
            Console.WriteLine("Description: Compares cached vs isolated execution plans.");
            break;

        case "query":
            Console.WriteLine("query");
            Console.WriteLine("Usage: dbperf query --sql \"<SQL statement>\"");
            Console.WriteLine("Description: Benchmarks any SQL query or stored procedure.");
            break;

        case "all":
            Console.WriteLine("all");
            Console.WriteLine("Usage: dbperf all");
            Console.WriteLine("Description: Runs the full performance suite.");
            break;

        default:
            Console.WriteLine($"Unknown command '{cmd}'.");
            break;
    }
}


