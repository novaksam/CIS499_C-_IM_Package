// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DBInteract.cs" company="Sam Novak">
//   CIS499 - 2013 - IM Server
// </copyright>
// <summary>
//   The database interaction class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CIS499_IM_Server
{
    using System;
    using System.Data.SqlServerCe;
    using System.IO;

    /// <summary>
    /// The d b_ interact.
    /// </summary>
    internal class DBInteract
    {
        /// <summary>
        /// The path to the database.
        /// </summary>
        private string path = "Data Source = " + System.Configuration.ConfigurationSettings.AppSettings["DB_File"]
                              + "; Encrypt Database=True; Max Database Size=512; " +
                              " Password='" + System.Configuration.ConfigurationSettings.AppSettings["DB_Pass"] + "';";

        /// <summary>
        /// Initializes a new instance of the <see cref="DBInteract"/> class.
        /// </summary>
        public DBInteract()
        {
            if (!File.Exists(System.Configuration.ConfigurationSettings.AppSettings["DB_File"]))
            {
                this.CreateDB();
            }
        }

        /// <summary>
        /// Write user to database
        /// </summary>
        /// <param name="user">
        /// The user to write
        /// </param>
        private void WriteUser(UserClass user)
        {

        }

        /// <summary>
        /// Create the initial database
        /// </summary>
        private void CreateDB()
        {
            var connection = new SqlCeConnection(this.path);

            try
            {
                var eng = new SqlCeEngine(this.path);
                var cleanup = new System.Threading.Tasks.Task(eng.Dispose);
                eng.CreateDatabase();
                cleanup.Start();
            }
            catch (Exception e)
            {
                EventLogging.WriteError(e);
            }

            connection.Open();
            var usersDB =
                new SqlCeCommand(
                    "CREATE TABLE Users_DB("
                    + "UserID int IDENTITY (100,1) NOT NULL UNIQUE, "
                    + "UserName nvarchar(128) NOT NULL, "
                    + "PassHash nvarchar(128) NOT NULL, "
                    + "Friends varbinary(5000), "
                    + "PRIMARY KEY (UserID));",
                    connection);
            usersDB.ExecuteNonQuery();
            usersDB.Dispose();
            connection.Dispose();
            connection.Close();
        }
    }
}
