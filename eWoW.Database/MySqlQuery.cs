using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace eWoW.Database
{
    public class MySqlQuery : SqlBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBase"/> class using the SqlConfig file settings.
        /// </summary>
        public MySqlQuery(string type) : base("MySqlConfig.xml", type)
        {}

        public MySqlQuery(string host, uint port, string user, string pass, string database) : base(host, port, user, pass, database)
        {
            
        }

        private MySqlConnectionStringBuilder _connString;

        protected override string ConnectionString
        {
            get
            {
                if (_connString == null)
                {
                    _connString = new MySqlConnectionStringBuilder
                                      {
                                          Server = ServerHost,
                                          Port = Port,
                                          UserID = Username,
                                          Password = Password,
                                          Database = Database
                                      };
                }
                return _connString.ToString();
            }
        }

        private MySqlConnection _conn;
        private MySqlConnection Connection
        {
            get
            {
                // TODO: Add the proper logic to make sure we're either null or disconnected.
                if (_conn == null || _conn.State == ConnectionState.Closed)
                {
                    _conn = new MySqlConnection(ConnectionString);
                    _conn.Open();
                }
                return _conn;
            }
        }

        public override DataTable Query(string quer)
        {
            var sqlConnection = new MySqlConnection();
            var sqlCommand = new MySqlCommand();
            var sqlAdapter = new MySqlDataAdapter();

            sqlConnection.ConnectionString = ConnectionString;

            var userData = new DataTable();

            try
            {
                sqlConnection.Open();

                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandText = quer;

                sqlAdapter.SelectCommand = sqlCommand;
                sqlAdapter.Fill(userData);
            }
            catch (MySqlException sqlError)
            {
                Console.WriteLine(
                    "------------------------------------------\nMySql Error: \n{0}\n------------------------------------------",
                    sqlError);
            }
            finally
            {
                sqlConnection.Close();
            }

            return userData;
        }

        public override void Execute(string quer)
        {
            var sqlConnection = new MySqlConnection();
            var sqlCommand = new MySqlCommand();

            sqlConnection.ConnectionString = ConnectionString;

            try
            {
                sqlConnection.Open();

                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandText = quer;
                sqlCommand.ExecuteScalar();
            }
            catch (MySqlException sqlError)
            {
                Console.WriteLine(
                    "------------------------------------------\nMySql Error: \n{0}\n------------------------------------------",
                    sqlError);
            }
            finally
            {
                sqlConnection.Close();
            }
        }
    }
}