// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Sam Novak" file="EntityBase.cs">
//   CIS499 - 2013 - IM Server
// </copyright>
// <summary>
//   Base class for all data access repositories
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace CIS499_IM_Server.DatabaseClasses
{
    using System.Data.SqlServerCe;

    /// <summary>
	/// Base class for all data access repositories
	/// </summary>
	public static class EntityBase
	{
	    /// <summary>
	    /// The sync lock.
	    /// </summary>
	    private static readonly object SyncLock = new object();

        /// <summary>
        /// The connection instance.
        /// </summary>
        private static SqlCeConnection connectionInstance;

	    /// <summary>
	    /// Initializes static members of the <see cref="EntityBase"/> class.
	    /// </summary>
	    static EntityBase()
		{
			ConnectionString = "Data Source='Users.sdf'";
		}

		/// <summary>
		/// Gets or sets the global SQL CE Connection String to be used
		/// </summary>
		public static string ConnectionString { get; set; }

		/// <summary>
		/// Gets or sets the global SQL CE Connection instance
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

				    if (connectionInstance.State != System.Data.ConnectionState.Open)
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
		/// Create a SqlCeCommand instance using the global Connection instance
		/// </summary>
		/// <returns>
		/// The <see cref="SqlCeCommand"/>.
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
			var command = Connection.CreateCommand();
			command.Transaction = transaction;
			return command;
		}
	}
}
