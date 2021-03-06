// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Sam Novak" file="UsersDBRepository.cs">
//   CIS499 - 2013 - IM Server
// </copyright>
// <summary>
//   Default IUsers_DBRepository implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CIS499_IM_Server.DatabaseClasses
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlServerCe;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    using UserClass;

    /// <summary>
    ///     Default IUsers_DBRepository implementation
    /// </summary>
    public class UsersDBRepository : IUsersDBRepository
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the transaction.
        /// </summary>
        public SqlCeTransaction Transaction { get; set; }

        /// <summary>
        /// The mutex.
        /// </summary>
        private static Mutex mutex = new Mutex(false, "Database_Mutex");

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the number of records in the table
        /// </summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public int Count()
        {
            using (var command = EntityBase.CreateCommand(this.Transaction))
            {
                command.CommandText = "SELECT COUNT(*) FROM Users_DB";
                return (int)command.ExecuteScalar();
            }
        }

        /// <summary>
        /// Inserts the item to the table
        /// </summary>
        /// <param name="item">
        /// Item to insert to the database
        /// </param>
        public void Create(UsersDB item)
        {
            this.Create(item.UserName, item.PassHash, item.Friends);
        }

        /// <summary>
        /// Inserts a new record to the table without specifying the primary key
        /// </summary>
        /// <param name="userName">
        /// UserName value
        /// </param>
        /// <param name="passHash">
        /// PassHash value
        /// </param>
        /// <param name="friends">
        /// Friends value
        /// </param>
        public void Create(string userName, string passHash, List<UserClass> friends)
        {
            if (userName != null && userName.Length > 128)
            {
                throw new ArgumentException("Max length for UserName is 128");
            }

            if (passHash != null && passHash.Length > 128)
            {
                throw new ArgumentException("Max length for PassHash is 128");
            }

            using (var command = EntityBase.CreateCommand(this.Transaction))
            {
                command.CommandText =
                    "INSERT INTO Users_DB (UserName, PassHash, Friends)  VALUES (@UserName, @PassHash, @Friends)";

                command.Parameters.Add("@UserName", SqlDbType.NVarChar);
                // command.Parameters["@UserName"].Value = userName != null ? (object)userName : DBNull.Value;
                command.Parameters["@UserName"].Value = userName;
                
                command.Parameters.Add("@PassHash", SqlDbType.NVarChar);
                // command.Parameters["@PassHash"].Value = passHash != null ? (object)passHash : DBNull.Value;
                command.Parameters["@PassHash"].Value = passHash;

                command.Parameters.Add("@Friends", SqlDbType.VarBinary);
                // command.Parameters["@Friends"].Value = friends != null ? (object)UserClass.StoreFriends(friends) : DBNull.Value;
                command.Parameters["@Friends"].Value = (object)UserClass.StoreFriends(friends);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Inserts a new record to the table specifying all fields
        /// </summary>
        /// <param name="userId">
        /// UserID value
        /// </param>
        /// <param name="userName">
        /// UserName value
        /// </param>
        /// <param name="passHash">
        /// PassHash value
        /// </param>
        /// <param name="friends">
        /// Friends value
        /// </param>
        public void Create(int? userId, string userName, string passHash, byte[] friends)
        {
            if (userName != null && userName.Length > UsersDB.UserNameMaxLength)
            {
                throw new ArgumentException("Max length for UserName is 128");
            }

            if (passHash != null && passHash.Length > UsersDB.PassHashMaxLength)
            {
                throw new ArgumentException("Max length for PassHash is 128");
            }

            using (SqlCeCommand command = EntityBase.CreateCommand(this.Transaction))
            {
                command.CommandText =
                    "INSERT INTO [Users_DB] (UserID, UserName, PassHash, Friends)  VALUES (@UserID, @UserName, @PassHash, @Friends)";

                command.Parameters.Add("@UserID", SqlDbType.Int);
                command.Parameters["@UserID"].Value = userId != null ? (object)userId : DBNull.Value;
                command.Parameters.Add("@UserName", SqlDbType.NVarChar);
                command.Parameters["@UserName"].Value = userName != null ? (object)userName : DBNull.Value;
                command.Parameters.Add("@PassHash", SqlDbType.NVarChar);
                command.Parameters["@PassHash"].Value = passHash != null ? (object)passHash : DBNull.Value;
                command.Parameters.Add("@Friends", SqlDbType.VarBinary);
                command.Parameters["@Friends"].Value = friends != null ? (object)friends : DBNull.Value;
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Populates the table with a collection of items
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        public void Create(IEnumerable<UsersDB> items)
        {
            using (SqlCeCommand command = EntityBase.CreateCommand(this.Transaction))
            {
                command.CommandType = CommandType.TableDirect;
                command.CommandText = "Users_DB";

                using (SqlCeResultSet resultSet = command.ExecuteResultSet(ResultSetOptions.Updatable))
                {
                    SqlCeUpdatableRecord record = resultSet.CreateRecord();
                    foreach (UsersDB item in items)
                    {
                        record.SetValue(1, item.UserName);
                        record.SetValue(2, item.PassHash);
                        record.SetValue(3, item.Friends);
                        resultSet.Insert(record);
                    }
                }
            }
        }

        /// <summary>
        /// Deletes the item
        /// </summary>
        /// <param name="item">
        /// Item to delete
        /// </param>
        public void Delete(UsersDB item)
        {
            using (var command = EntityBase.CreateCommand(this.Transaction))
            {
                command.CommandText = "DELETE FROM [Users_DB] WHERE UserID = @UserID";

                command.Parameters.Add("@UserID", SqlDbType.Int);
                command.Parameters["@UserID"].Value = item.UserId;
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes a collection of item
        /// </summary>
        /// <param name="items">
        /// Items to delete
        /// </param>
        public void Delete(IEnumerable<UsersDB> items)
        {
            using (var command = EntityBase.CreateCommand(this.Transaction))
            {
                command.CommandText = "DELETE FROM [Users_DB] WHERE UserID = @UserID";
                command.Parameters.Add("@UserID", SqlDbType.Int);
                command.Prepare();

                foreach (UsersDB item in items)
                {
                    command.Parameters["@UserID"].Value = item.UserId;

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Delete records by Friends
        /// </summary>
        /// <param name="friends">
        /// Friends value
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int DeleteByFriends(byte[] friends)
        {
            using (var command = EntityBase.CreateCommand(this.Transaction))
            {
                command.CommandText = "DELETE FROM [Users_DB] WHERE Friends=@Friends";
                command.Parameters.Add("@Friends", SqlDbType.Image);
                command.Parameters["@Friends"].Value = friends != null ? (object)friends : DBNull.Value;

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Delete records by PassHash
        /// </summary>
        /// <param name="passHash">
        /// PassHash value
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int DeleteByPassHash(string passHash)
        {
            using (SqlCeCommand command = EntityBase.CreateCommand(this.Transaction))
            {
                command.CommandText = "DELETE FROM [Users_DB] WHERE PassHash=@PassHash";
                command.Parameters.Add("@PassHash", SqlDbType.NVarChar);
                command.Parameters["@PassHash"].Value = passHash != null ? (object)passHash : DBNull.Value;

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Delete records by UserID
        /// </summary>
        /// <param name="userId">
        /// UserID value
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int DeleteByUserId(int? userId)
        {
            using (SqlCeCommand command = EntityBase.CreateCommand(this.Transaction))
            {
                command.CommandText = "DELETE FROM [Users_DB] WHERE UserID=@UserID";
                command.Parameters.Add("@UserID", SqlDbType.Int);
                command.Parameters["@UserID"].Value = userId != null ? (object)userId : DBNull.Value;

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Delete records by UserName
        /// </summary>
        /// <param name="userName">
        /// UserName value
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int DeleteByUserName(string userName)
        {
            using (SqlCeCommand command = EntityBase.CreateCommand(this.Transaction))
            {
                command.CommandText = "DELETE FROM [Users_DB] WHERE UserName=@UserName";
                command.Parameters.Add("@UserName", SqlDbType.NVarChar);
                command.Parameters["@UserName"].Value = userName != null ? (object)userName : DBNull.Value;

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        ///     Purges the contents of the table
        /// </summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public int Purge()
        {
            using (SqlCeCommand command = EntityBase.CreateCommand(this.Transaction))
            {
                command.CommandText = "DELETE FROM [Users_DB]";
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Retrieves a collection of items by Friends
        /// </summary>
        /// <param name="friends">
        /// Friends value
        /// </param>
        /// <returns>
        /// The
        ///     <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<UsersDB> SelectByFriends(byte[] friends)
        {
            var list = new List<UsersDB>();
            using (SqlCeCommand command = EntityBase.CreateCommand(this.Transaction))
            {
                if (friends != null)
                {
                    command.CommandText = "SELECT * FROM Users_DB WHERE Friends=@Friends";
                    command.Parameters.Add("@Friends", SqlDbType.Image);
                    command.Parameters["@Friends"].Value = friends;
                }
                else
                {
                    command.CommandText = "SELECT * FROM Users_DB WHERE Friends IS NULL";
                }

                using (SqlCeDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new UsersDB
                                       {
                                           UserId = (int)reader["UserID"], 
                                           UserName = reader.IsDBNull(1) ? null : reader["UserName"] as string, 
                                           PassHash = reader.IsDBNull(2) ? null : reader["PassHash"] as string, 
                                           Friends =
                                               reader.IsDBNull(3) ? null : reader["Friends"] as List<UserClass>
                                       };
                        list.Add(item);
                    }
                }
            }

            return list.Count > 0 ? list : null;
        }

        /// <summary>
        /// Retrieves the first set of items specified by count by Friends
        /// </summary>
        /// <param name="friends">
        /// Friends value
        /// </param>
        /// <param name="count">
        /// Number of records to be retrieved
        /// </param>
        /// <returns>
        /// The
        ///     <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<UsersDB> SelectByFriends(byte[] friends, int count)
        {
            var list = new List<UsersDB>();
            using (SqlCeCommand command = EntityBase.CreateCommand(this.Transaction))
            {
                if (friends != null)
                {
                    command.CommandText = "SELECT TOP(" + count + ") * FROM Users_DB WHERE Friends=@Friends";
                    command.Parameters.Add("@Friends", SqlDbType.Image);
                    command.Parameters["@Friends"].Value = friends;
                }
                else
                {
                    command.CommandText = "SELECT TOP(" + count + ") * FROM Users_DB WHERE Friends IS NULL";
                }

                using (SqlCeDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new UsersDB
                                       {
                                           UserId = (int)reader["UserID"], 
                                           UserName = reader.IsDBNull(1) ? null : reader["UserName"] as string, 
                                           PassHash = reader.IsDBNull(2) ? null : reader["PassHash"] as string, 
                                           Friends =
                                               reader.IsDBNull(3) ? null : reader["Friends"] as List<UserClass>
                                       };
                        list.Add(item);
                    }
                }
            }

            return list.Count > 0 ? list : null;
        }

        /// <summary>
        /// Retrieves a collection of items by PassHash
        /// </summary>
        /// <param name="passHash">
        /// PassHash value
        /// </param>
        /// <returns>
        /// The
        ///     <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<UsersDB> SelectByPassHash(string passHash)
        {
            var list = new List<UsersDB>();
            using (SqlCeCommand command = EntityBase.CreateCommand(this.Transaction))
            {
                if (passHash != null)
                {
                    command.CommandText = "SELECT * FROM Users_DB WHERE PassHash=@PassHash";
                    command.Parameters.Add("@PassHash", SqlDbType.NVarChar);
                    command.Parameters["@PassHash"].Value = passHash;
                }
                else
                {
                    command.CommandText = "SELECT * FROM Users_DB WHERE PassHash IS NULL";
                }

                using (SqlCeDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new UsersDB
                                       {
                                           UserId = (int)reader["UserID"], 
                                           UserName = reader.IsDBNull(1) ? null : reader["UserName"] as string, 
                                           PassHash = reader.IsDBNull(2) ? null : reader["PassHash"] as string, 
                                           Friends =
                                               reader.IsDBNull(3) ? null : reader["Friends"] as List<UserClass>
                                       };
                        list.Add(item);
                    }
                }
            }

            return list.Count > 0 ? list : null;
        }

        /// <summary>
        /// Retrieves the first set of items specified by count by PassHash
        /// </summary>
        /// <param name="passHash">
        /// PassHash value
        /// </param>
        /// <param name="count">
        /// Number of records to be retrieved
        /// </param>
        /// <returns>
        /// The
        ///     <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<UsersDB> SelectByPassHash(string passHash, int count)
        {
            var list = new List<UsersDB>();
            using (SqlCeCommand command = EntityBase.CreateCommand(this.Transaction))
            {
                if (passHash != null)
                {
                    command.CommandText = "SELECT TOP(" + count + ") * FROM Users_DB WHERE PassHash=@PassHash";
                    command.Parameters.Add("@PassHash", SqlDbType.NVarChar);
                    command.Parameters["@PassHash"].Value = passHash;
                }
                else
                {
                    command.CommandText = "SELECT TOP(" + count + ") * FROM Users_DB WHERE PassHash IS NULL";
                }

                using (SqlCeDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new UsersDB
                                       {
                                           UserId = (int)reader["UserID"], 
                                           UserName = reader.IsDBNull(1) ? null : reader["UserName"] as string, 
                                           PassHash = reader.IsDBNull(2) ? null : reader["PassHash"] as string, 
                                           Friends =
                                               reader.IsDBNull(3) ? null : reader["Friends"] as List<UserClass>
                                       };
                        list.Add(item);
                    }
                }
            }

            return list.Count > 0 ? list : null;
        }

        /// <summary>
        /// Retrieves a collection of items by UserID
        /// </summary>
        /// <param name="userID">
        /// UserID value
        /// </param>
        /// <returns>
        /// The
        ///     <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<UsersDB> SelectByUserID(int? userID)
        {
            mutex.WaitOne();
            var list = new List<UsersDB>();
            using (SqlCeCommand command = EntityBase.CreateCommand(this.Transaction))
            {
                if (userID != null)
                {
                    command.CommandText = "SELECT * FROM Users_DB WHERE UserID=@UserID";
                    command.Parameters.Add("@UserID", SqlDbType.Int);
                    command.Parameters["@UserID"].Value = userID;
                }
                else
                {
                    command.CommandText = "SELECT * FROM Users_DB WHERE UserID IS NULL";
                }

                using (SqlCeDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new UsersDB
                                       {
                                           UserId = (int)reader["UserID"], 
                                           UserName = reader.IsDBNull(1) ? null : reader["UserName"] as string, 
                                           PassHash = reader.IsDBNull(2) ? null : reader["PassHash"] as string, 
                                           Friends =
                                               reader.IsDBNull(3) ? null : reader["Friends"] as List<UserClass>
                                       };
                        list.Add(item);
                    }
                }
            }

            mutex.ReleaseMutex();
            return list.Count > 0 ? list : null;
        }

        /// <summary>
        /// The select by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The
        ///     <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1627:DocumentationTextMustNotBeEmpty", 
            Justification = "Reviewed. Suppression is OK here.")]
        [Obsolete]
        public List<UsersDB> SelectByUserId(int userId)
        {
            return this.SelectByUserID(userId);
        }

        /// <summary>
        /// The select by user id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The
        ///     <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1627:DocumentationTextMustNotBeEmpty", 
            Justification = "Reviewed. Suppression is OK here.")]
        [Obsolete]
        public List<UsersDB> SelectByUserId(int userId, int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the first set of items specified by count by UserID
        /// </summary>
        /// <param name="userID">
        /// UserID value
        /// </param>
        /// <param name="count">
        /// Number of records to be retrieved
        /// </param>
        /// <returns>
        /// The
        ///     <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [Obsolete]
        public List<UsersDB> SelectByUserId(int? userID, int count)
        {
            var list = new List<UsersDB>();
            using (SqlCeCommand command = EntityBase.CreateCommand(this.Transaction))
            {
                if (userID != null)
                {
                    command.CommandText = "SELECT TOP(" + count + ") * FROM Users_DB WHERE UserID=@UserID";
                    command.Parameters.Add("@UserID", SqlDbType.Int);
                    command.Parameters["@UserID"].Value = userID;
                }
                else
                {
                    command.CommandText = "SELECT TOP(" + count + ") * FROM Users_DB WHERE UserID IS NULL";
                }

                using (SqlCeDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new UsersDB
                                       {
                                           UserId = (int)reader["UserID"], 
                                           UserName = reader.IsDBNull(1) ? null : reader["UserName"] as string, 
                                           PassHash = reader.IsDBNull(2) ? null : reader["PassHash"] as string, 
                                           Friends =
                                               reader.IsDBNull(3) ? null : reader["Friends"] as List<UserClass>
                                       };
                        list.Add(item);
                    }
                }
            }

            return list.Count > 0 ? list : null;
        }

        /// <summary>
        /// Search for users in the database
        /// </summary>
        /// <param name="userName">Username to search for</param>
        /// <param name="user">User doing the search</param>
        /// <returns>List of users</returns>
        public List<UserClass> SearchByUserName(string userName, UserClass user)
        {
            mutex.WaitOne();
            var list = new List<UserClass>();
            using (SqlCeCommand command = EntityBase.CreateCommand(this.Transaction))
            {
                if (userName != null && userName.Length < 1)
                {
                    command.CommandText = "SELECT UserID, UserName FROM Users_DB WHERE (UserName LIKE @UserName) AND (NOT (UserName = @Exclude))";
                    
                    // command.Parameters.AddWithValue("@UserName", "'%" + userName + "%'");
                    command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = "%" + userName + "%";
                    
                    // command.Parameters["@UserName"].Value = userName;
                    // TODO excluded searcher
                }
                else
                {
                    command.CommandText = "SELECT UserID, UserName FROM Users_DB WHERE (NOT (UserName = @Exclude))";
                }

                command.Parameters.AddWithValue("@Exclude", user.UserName);

                using (SqlCeDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new UserClass
                                       {
                                           UserId = (int)reader["UserID"],
                                           UserName = reader["UserName"] as string
                                       };
                        list.Add(item);
                    }
                }
            }

            mutex.ReleaseMutex();
            return list;
        }

        /// <summary>
        /// Retrieves a collection of items by UserName
        /// </summary>
        /// <param name="userName">
        /// UserName value
        /// </param>
        /// <returns>
        /// The
        ///     <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<UsersDB> SelectByUserName(string userName)
        {
            mutex.WaitOne();
            var list = new List<UsersDB>();
            using (SqlCeCommand command = EntityBase.CreateCommand(this.Transaction))
            {
                if (userName != null)
                {
                    command.CommandText = "SELECT * FROM Users_DB WHERE UserName=@UserName";
                    command.Parameters.Add("@UserName", SqlDbType.NVarChar);
                    command.Parameters["@UserName"].Value = userName;
                }
                else
                {
                    command.CommandText = "SELECT * FROM Users_DB WHERE UserName IS NULL";
                }

                using (SqlCeDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new UsersDB();
                        item.UserId = (int)reader["UserID"];
                        item.UserName = reader["UserName"] as string;
                        item.PasswordHash = reader["PassHash"] as string;
                        // item.Friends = reader.IsDBNull(3) ? new List<UserClass>() : UserClass.RestoreFriends(reader["Friends"] as byte[]);
                        var taco = reader["Friends"] as byte[];
                        var taco2 = UserClass.RestoreFriends(taco);
                        item.Friends = taco2;
                        list.Add(item);
                    }
                }
            }

            mutex.ReleaseMutex();
            return list.Count > 0 ? list : null;
        }

        /// <summary>
        /// Retrieves the first set of items specified by count by UserName
        /// </summary>
        /// <param name="userName">
        /// UserName value
        /// </param>
        /// <param name="count">
        /// Number of records to be retrieved
        /// </param>
        /// <returns>
        /// The
        ///     <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<UsersDB> SelectByUserName(string userName, int count)
        {
            mutex.WaitOne();
            var list = new List<UsersDB>();
            using (var command = EntityBase.CreateCommand(this.Transaction))
            {
                if (userName != null)
                {
                    command.CommandText = "SELECT TOP(" + count + ") * FROM Users_DB WHERE UserName=@UserName";
                    command.Parameters.Add("@UserName", SqlDbType.NVarChar);
                    command.Parameters["@UserName"].Value = userName;
                }
                else
                {
                    command.CommandText = "SELECT TOP(" + count + ") * FROM Users_DB WHERE UserName IS NULL";
                }

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new UsersDB
                                       {
                                           UserId = (int)reader["UserID"], 
                                           UserName = reader.IsDBNull(1) ? null : reader["UserName"] as string, 
                                           PassHash = reader.IsDBNull(2) ? null : reader["PassHash"] as string, 
                                           Friends =
                                               reader.IsDBNull(3) ? null : reader["Friends"] as List<UserClass>
                                       };
                        list.Add(item);
                    }
                }
            }

            mutex.ReleaseMutex();
            return list.Count > 0 ? list : null;
        }

        /// <summary>
        ///     The to array.
        /// </summary>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>UsersDB[]</cref>
        ///     </see>
        ///     .
        /// </returns>
        public UsersDB[] ToArray()
        {
            var list = this.ToList();
            return list != null ? list.ToArray() : null;
        }

        /// <summary>
        /// The to array.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The
        ///     <see>
        ///         <cref>UsersDB[]</cref>
        ///     </see>
        ///     .
        /// </returns>
        public UsersDB[] ToArray(int count)
        {
            List<UsersDB> list = this.ToList(count);
            return list != null ? list.ToArray() : null;
        }

        /// <summary>
        ///     Retrieves all items as a generic collection
        /// </summary>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<UsersDB> ToList()
        {
            mutex.WaitOne();
            var list = new List<UsersDB>();
            using (var command = EntityBase.CreateCommand(this.Transaction))
            {
                command.CommandText = "SELECT * FROM Users_DB";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new UsersDB
                                       {
                                           UserId = (int)reader["UserID"], 
                                           UserName = reader.IsDBNull(1) ? null : reader["UserName"] as string, 
                                           PassHash = reader.IsDBNull(2) ? null : reader["PassHash"] as string, 
                                           Friends =
                                               reader.IsDBNull(3) ? null : reader["Friends"] as List<UserClass>
                                       };
                        list.Add(item);
                    }
                }
            }

            mutex.ReleaseMutex();
            return list.Count > 0 ? list : null;
        }

        /// <summary>
        /// Retrieves the first set of items specified by count as a generic collection
        /// </summary>
        /// <param name="count">
        /// Number of records to be retrieved
        /// </param>
        /// <returns>
        /// The
        ///     <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<UsersDB> ToList(int count)
        {
            mutex.WaitOne();
            var list = new List<UsersDB>();
            using (var command = EntityBase.CreateCommand(this.Transaction))
            {
                command.CommandText = string.Format("SELECT TOP({0}) * FROM Users_DB", count);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new UsersDB
                                       {
                                           UserId = (int)reader["UserID"], 
                                           UserName = reader.IsDBNull(1) ? null : reader["UserName"] as string, 
                                           PassHash = reader.IsDBNull(2) ? null : reader["PassHash"] as string, 
                                           Friends =
                                               reader.IsDBNull(3) ? null : reader["Friends"] as List<UserClass>
                                       };
                        list.Add(item);
                    }
                }
            }

            mutex.ReleaseMutex();

            return list.Count > 0 ? list : null;
        }

        /// <summary>
        /// Updates the item
        /// </summary>
        /// <param name="item">
        /// Item to update
        /// </param>
        public void Update(UsersDB item)
        {
            using (SqlCeCommand command = EntityBase.CreateCommand(this.Transaction))
            {
                command.CommandText =
                    "UPDATE [Users_DB] SET UserName = @UserName, PassHash = @PassHash, Friends = @Friends WHERE UserID = @UserID";

                command.Parameters.Add("@UserID", SqlDbType.Int);
                command.Parameters["@UserID"].Value = item.UserId;
                command.Parameters.Add("@UserName", SqlDbType.NVarChar);
                command.Parameters["@UserName"].Value = item.UserName != null ? (object)item.UserName : DBNull.Value;
                command.Parameters.Add("@PassHash", SqlDbType.NVarChar);
                command.Parameters["@PassHash"].Value = item.PassHash != null ? (object)item.PassHash : DBNull.Value;
                command.Parameters.Add("@Friends", SqlDbType.Image);
                command.Parameters["@Friends"].Value = item.Friends != null ? (object)item.Friends : DBNull.Value;
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void UpdateFriends(UserClass item)
        {
            mutex.WaitOne();
            using (SqlCeCommand command = EntityBase.CreateCommand(this.Transaction))
            {
                command.CommandText =
                    "UPDATE [Users_DB] SET  Friends = @Friends WHERE UserID = @UserID";

                command.Parameters.Add("@UserID", SqlDbType.Int);
                command.Parameters["@UserID"].Value = item.UserId;
                command.Parameters.Add("@Friends", SqlDbType.Image);
                command.Parameters["@Friends"].Value = UserClass.StoreFriends(item.Friends);
                command.ExecuteNonQuery();
            }

            mutex.ReleaseMutex();
        }

        /// <summary>
        /// Updates a collection of items
        /// </summary>
        /// <param name="items">
        /// Items to update
        /// </param>
        public void Update(IEnumerable<UsersDB> items)
        {
            using (var command = EntityBase.CreateCommand(this.Transaction))
            {
                command.CommandText =
                    "UPDATE [Users_DB] SET UserName = @UserName, PassHash = @PassHash, Friends = @Friends WHERE UserID = @UserID";
                command.Parameters.Add("@UserID", SqlDbType.Int);
                command.Parameters.Add("@UserName", SqlDbType.NVarChar);
                command.Parameters.Add("@PassHash", SqlDbType.NVarChar);
                command.Parameters.Add("@Friends", SqlDbType.Image);
                command.Prepare();

                foreach (var item in items)
                {
                    command.Parameters["@UserID"].Value = item.UserId;
                    command.Parameters["@UserName"].Value = item.UserName != null ? (object)item.UserName : DBNull.Value;
                    command.Parameters["@PassHash"].Value = item.PassHash != null ? (object)item.PassHash : DBNull.Value;
                    command.Parameters["@Friends"].Value = item.Friends != null ? (object)item.Friends : DBNull.Value;
                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion
    }
}