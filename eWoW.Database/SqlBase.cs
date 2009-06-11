#define MYSQL
using System.Collections.Generic;
using System.Data;
using eWoW.Common;

namespace eWoW.Database
{
    public abstract class SqlBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBase"/> class using the SqlConfig file settings.
        /// </summary>
        protected SqlBase(string configFile, string sqlElement)
            : this(
                Config.GetValue<string>(configFile, sqlElement, "Host"), Config.GetValue<uint>(configFile, sqlElement, "Port"),
                Config.GetValue<string>(configFile, sqlElement, "Username"), Config.GetValue<string>(configFile, sqlElement, "Password"),
                Config.GetValue<string>(configFile, sqlElement, "Database"))
        {}

        protected SqlBase(string host, uint port, string user, string pass, string database)
        {
            ServerHost = host;
            Port = port;
            Username = user;
            Password = pass;
            Database = database;

            Logging.Write("Host: {0}, Port: {1}, Username: {2}, Password: {3}, Database: {4}", host, port,
                                       user, pass, database);
        }

        public string ServerHost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public uint Port { get; set; }

        protected abstract string ConnectionString { get; }
        private static readonly Dictionary<string,SqlBase> Instances = new Dictionary<string, SqlBase>();

        public static SqlBase Instance(string type)
        {
            if (!Instances.ContainsKey(type))
            {
#if MYSQL
                Instances.Add(type, new MySqlQuery(type));
#endif
            }
            return Instances[type];
        }

        public abstract void Execute(string query);

        public abstract DataTable Query(string query);
    }
}