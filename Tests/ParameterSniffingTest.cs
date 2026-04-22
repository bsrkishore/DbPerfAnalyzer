public class ParameterSniffingTest
{
    private readonly string _connectionString;

    public ParameterSniffingTest(string connectionString)
    {
        _connectionString = connectionString;
    }

    public long Run(int customerId)
    {
        var sql = "EXEC GetByCustomerId @CustomerId";

        return SqlHelper.TimeQuery(
            _connectionString,
            sql,
            new Microsoft.Data.SqlClient.SqlParameter("@CustomerId", customerId)
        );
    }
}
