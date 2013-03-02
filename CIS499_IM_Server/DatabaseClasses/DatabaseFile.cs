// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Sam Novak" file="DatabaseFile.cs">
//   CIS499 - 2013 - IM Server
// </copyright>
// <summary>
//   Helper class for generating the database file in runtime
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace CIS499_IM_Server.DatabaseClasses
{
    /// <summary>
	/// Helper class for generating the database file in runtime
	/// </summary>
	public static class DatabaseFile
	{
		/// <summary>
		/// Creates the database
		/// </summary>
		/// <returns>
		/// The <see cref="int"/>.
		/// </returns>
		public static int CreateDatabase()
		{
			var resultCount = 0;

		    using (var engine = new System.Data.SqlServerCe.SqlCeEngine(EntityBase.ConnectionString))
		    {
		        engine.CreateDatabase();
		    }

		    using (var transaction = EntityBase.Connection.BeginTransaction())
			using (var command = EntityBase.CreateCommand(transaction))
			{
				command.CommandText = "CREATE TABLE [Users_DB](UserID INT IDENTITY(100,1) PRIMARY KEY NOT NULL, UserName NVARCHAR(128) NOT NULL, PassHash NVARCHAR(128) NOT NULL, Friends VARBINARY)";
				resultCount += command.ExecuteNonQuery();

				transaction.Commit();
			}

			return resultCount;
		}
	}
}
