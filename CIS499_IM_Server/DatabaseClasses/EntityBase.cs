// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Sam Novak" file="EntityBase.cs">
//   CIS499 - 2013 - IM Server
// </copyright>
// <summary>
//   Base class for all data access repositories
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CIS499_IM_Server.DatabaseClasses
{
    using System.Configuration;
    using System.Data;
    using System.Data.SqlServerCe;
    using System.IO;

    /// <summary>
    ///     Base class for all data access repositories
    /// </summary>
    public static class EntityBase
    {
        #region Static Fields

        /// <summary>
        ///     The sync lock.
        /// </summary>
        private static readonly object SyncLock = new object();

        /// <summary>
        ///     The connection instance.
        /// </summary>
        private static SqlCeConnection connectionInstance;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="EntityBase" /> class.
        /// </summary>
        static EntityBase()
        {
            ConnectionString = "Data Source='" 
                + ConfigurationSettings.AppSettings["DB_File"] + "';Password=" 
                + ConfigurationSettings.AppSettings["DB_Pass"]
                               + ";Persist Security Info=True";
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the global SQL CE Connection instance
        /// </summary>
        public static SqlCeConnection Connection
        {
            get
            {
                lock (SyncLock)
                {
                    if (connectionInstance == null)
                    {
                        connectionInstance = new SqlCeConnection(ConnectionString);
                    }

                    if (connectionInstance.State != ConnectionState.Open)
                    {
                        connectionInstance.Open();
                    }

                    return connectionInstance;
                }
            }

            set
            {
                lock (SyncLock)
                {
                    connectionInstance = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the global SQL CE Connection String to be used
        /// </summary>
        public static string ConnectionString { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Create a SqlCeCommand instance using the global Connection instance
        /// </summary>
        /// <returns>
        ///     The <see cref="SqlCeCommand" />.
        /// </returns>
        public static SqlCeCommand CreateCommand()
        {
            return CreateCommand(null);
        }

        /// <summary>
        /// Create a SqlCeCommand instance using the global SQL CE Connection instance and associate this with a transaction
        /// </summary>
        /// <param name="transaction">
        /// SqlCeTransaction to be used for the SqlCeCommand
        /// </param>
        /// <returns>
        /// The <see cref="SqlCeCommand"/>.
        /// </returns>
        public static SqlCeCommand CreateCommand(SqlCeTransaction transaction)
        {
            // TODO come up with a better way to check for the database file
            SqlCeCommand command;
                
                if (!File.Exists(ConfigurationSettings.AppSettings["DB_File"]))
                {
                    DatabaseFile.CreateDatabase();
                }
                command = Connection.CreateCommand();
            //try
            //{
            //    command = Connection.CreateCommand();
            //}
            //catch (SqlCeException notFound)
            //{
                
            //    if (notFound.Message.Contains("be found"))
            //    {
                    
            //    }

            //    command = Connection.CreateCommand();
            //}

            command.Transaction = transaction;
            return command;
        }

        #endregion
    }
}