using Microsoft.Data.SqlClient;

namespace APBD;

public interface ISqlConnectionFactory
{
    public Task<SqlConnection> CreateConnectionAsync();
}