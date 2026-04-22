public class MissingIndexTest
{
    private readonly string _connectionString;

    public MissingIndexTest(string connectionString)
    {
        _connectionString = connectionString;
    }

    public long Run(string table, string column)
    {
        var sql = $"SELECT * FROM {table} WHERE {column} = 1";
        return SqlHelper.TimeQuery(_connectionString, sql);
    }
}
