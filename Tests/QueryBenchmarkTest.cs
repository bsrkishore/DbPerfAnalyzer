public class QueryBenchmarkTest
{
    private readonly string _connectionString;

    public QueryBenchmarkTest(string connectionString)
    {
        _connectionString = connectionString;
    }

    public long Run(string sql)
    {
        return SqlHelper.TimeQuery(_connectionString, sql);
    }
}
