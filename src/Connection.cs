using DbSyncKit.DB.Enum;
using DbSyncKit.DB.Interface;
using Microsoft.Data.Sqlite;
using System;
using System.Data;

namespace DbSyncKit.SQLite
{
    /// <summary>
    /// Represents a connection to a SQLite database, implementing the <see cref="IDatabase"/> interface.
    /// </summary>
    /// <remarks>
    /// This class provides a generic representation of a database connection and includes methods
    /// defined in the <see cref="IDatabase"/> interface for executing queries and managing transactions.
    /// </remarks>
    public class Connection : IDatabase
    {
        #region Declaration

        private readonly string DataSource;

        /// <summary>
        /// Gets the database provider type, which is SQLite for this class.
        /// </summary>
        public DatabaseProvider Provider => DatabaseProvider.SQLite;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Connection class with SQLite database file details.
        /// </summary>
        /// <param name="dataSource">The file path of the SQLite database.</param>
        public Connection(string dataSource)
        {
            DataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the connection string for the SQLite database using provided file path.
        /// </summary>
        /// <returns>A string representing the SQLite connection string.</returns>
        public string GetConnectionString()
        {
            return $"Data Source={DataSource};";
        }

        /// <summary>
        /// Executes a query against the SQLite database and returns the results as a DataSet.
        /// </summary>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="tableName">The name to assign to the resulting table within the DataSet.</param>
        /// <returns>A DataSet containing the results of the query.</returns>
        public DataSet ExecuteQuery(string query, string tableName)
        {
            try
            {

                using (SqliteConnection sqliteConnection = new(GetConnectionString()))
                {
                    sqliteConnection.Open();

                    using (SqliteCommand command = new(query, sqliteConnection))
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable(tableName);
                        dataTable.Load(reader);
                        DataSet dataSet = new DataSet();
                        dataSet.Tables.Add(dataTable);
                        return dataSet;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, log it, or rethrow a more specific exception.
                throw new Exception("Error executing query: " + ex.Message);
            }
        }

        /// <summary>
        /// Tests the connection to the SQLite database.
        /// </summary>
        /// <returns>True if the connection is successful; otherwise, false.</returns>
        public bool TestConnection()
        {
            try
            {
                using (SqliteConnection sqliteConnection = new(GetConnectionString()))
                {
                    sqliteConnection.Open();
                    sqliteConnection.Close();

                    return true;
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, log it, or rethrow a more specific exception.
                throw new Exception("Error testing connection: " + ex.Message);
            }
        }

        #endregion
    }
}
