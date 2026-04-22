public class TableScanTest
{
    private readonly string _connectionString;

    public TableScanTest(string connectionString)
    {
        _connectionString = connectionString;
    }

    public long Run(string table)
    {
        var sql = $"SELECT * FROM {table}";
        return SqlHelper.TimeQuery(_connectionString, sql);
    }
}
