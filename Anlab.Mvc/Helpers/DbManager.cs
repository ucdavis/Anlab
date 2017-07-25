using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace AnlabMvc.Helpers
{
    public class DbManager : IDisposable
    {
        public readonly DbConnection Connection;

        public DbManager(string connectionString)
        {
            Connection = new SqlConnection(connectionString);
            Connection.OpenAsync().Wait();
        }

        public void Dispose()
        {
            Connection.Close();
        }
    }
}
