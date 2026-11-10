using System;
using System.Configuration;
using Microsoft.Data.SqlClient;

namespace StudentRegistration.DataAccess.Database
{
    public class DbConnectionFactory
    {
        private readonly string connectionString;

        public DbConnectionFactory()
        {
            connectionString = ConfigurationManager.ConnectionStrings["STUREGConnection"]?.ConnectionString ?? throw new InvalidOperationException(
                    "Database connection string not found."
                );
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
