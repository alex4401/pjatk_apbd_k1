using Microsoft.Data.SqlClient;

namespace APBD;

public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connString =
        "Data Source=db-mssql16.pjwstk.edu.pl;Initial Catalog=2019SBD;Integrated Security=True;Trust Server Certificate=True";

    public async Task<SqlConnection> CreateConnectionAsync()
    {
        SqlConnection result = new SqlConnection(_connString);
        await result.OpenAsync();
        return result;
    }
}
