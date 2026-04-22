using Microsoft.Data.SqlClient;
using System.Diagnostics;

public static class SqlHelper
{
    public static long TimeQuery(string connectionString, string sql)
    {
        using var conn = new SqlConnection(connectionString);
        using var cmd = new SqlCommand(sql, conn);

        conn.Open();

        var sw = Stopwatch.StartNew();
        cmd.ExecuteNonQuery();
        sw.Stop();

        return sw.ElapsedMilliseconds;
    }

    public static long TimeQuery(string connectionString, string sql, params SqlParameter[] parameters)
{
    var sw = System.Diagnostics.Stopwatch.StartNew();

    using var conn = new SqlConnection(connectionString);
    conn.Open();

    using var cmd = new SqlCommand(sql, conn);

    if (parameters != null)
        cmd.Parameters.AddRange(parameters);

    using var reader = cmd.ExecuteReader();
    while (reader.Read()) { }

    sw.Stop();
    return sw.ElapsedMilliseconds;
}

public static void ClearPlanCache(string connectionString)
{
    using var conn = new SqlConnection(connectionString);
    conn.Open();

    using var cmd = new SqlCommand("DBCC FREEPROCCACHE;", conn);
    cmd.ExecuteNonQuery();
}


}
