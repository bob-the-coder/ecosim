using System;
using System.Configuration;
using System.Data.SqlClient;
using Logging;

namespace DataLayer
{
    internal class DatabaseConnection
    {
        public static ConnectionStringSettings ConnectionStringSettings { get; private set; }

        /// <summary>
        ///     This property always returns a new connection on each call. CLOSE the connection when you are done with it, it is
        ///     YOUR responsability!
        /// </summary>
        internal static SqlConnection NewConnection
        {
            get { return SetupConnection(); }
        }

        private static SqlConnection SetupConnection()
        {
            SqlConnection result = null;

            if (ConnectionStringSettings == null) // setting the connection string one time only
            {
                if (!String.IsNullOrWhiteSpace(LocalConstants.DBConnectionName))
                {
                    ConnectionStringSettings conS =
                        ConfigurationManager.ConnectionStrings[LocalConstants.DBConnectionName];
                    if (conS != null)
                    {
                        ConnectionStringSettings = conS;
                    }
                    else
                    {
                        LogHelper.LogException<DatabaseConnection>(
                            "There is no connection string with the given name OR there is a formatting error in the config file");
                    }
                }
                else
                {
                    LogHelper.LogException<DatabaseConnection>(
                        "Connection name constant is invalid (null or white space)");
                }
            }

            if (ConnectionStringSettings != null)
            {
                try
                {
                    var newConnection = new SqlConnection(ConnectionStringSettings.ConnectionString);
                    newConnection.Open();
                    result = newConnection;
                }
                catch (Exception e)
                {
                    LogHelper.LogException<DatabaseConnection>(e,
                        "A problem occurred when trying to create the connection");
                }
            }
            return result;
        }

        internal static bool CloseConnection(SqlConnection conn)
        {
            bool result = false;
            if (conn != null)
            {
                conn.Close();
                conn.Dispose();
                conn = null;
                result = true;
            }
            return result;
        }
    }
}