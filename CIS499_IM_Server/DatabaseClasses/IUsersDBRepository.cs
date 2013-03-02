// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Sam Novak" file="IUsersDBRepository.cs">
//   CIS499 - 2013 - IM Server
// </copyright>
// <summary>
//   Represents the Users_DB repository
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CIS499_IM_Server.DatabaseClasses
{
    using System.Collections.Generic;

    using UserClass;

    /// <summary>
	/// Represents the Users_DB repository
	/// </summary>
	public interface IUsersDBRepository : IRepository<UsersDB>
	{
		/// <summary>
		/// Transaction instance created from <see cref="IDataRepository" />
		/// </summary>
		System.Data.SqlServerCe.SqlCeTransaction Transaction { get; set; }

		/// <summary>
		/// Retrieves a collection of items by UserID
		/// </summary>
		/// <param name="userId">
		/// UserID value
		/// </param>
		/// <returns>
		/// The <see cref="List{T}"/>.
		/// </returns>
		List<UsersDB> SelectByUserId(int userId);

        /// <summary>
        /// Retrieves the first set of items specified by count by UserID
        /// </summary>
        /// <param name="userId">
        /// UserID value
        /// </param>
        /// <param name="count">
        /// the number of records to be retrieved
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        List<UsersDB> SelectByUserId(int userId, int count);

        /// <summary>
        /// Retrieves a collection of items by UserName
        /// </summary>
        /// <param name="userName">
        /// UserName value
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        List<UsersDB> SelectByUserName(string userName);

        /// <summary>
        /// Retrieves the first set of items specified by count by UserName
        /// </summary>
        /// <param name="userName">
        /// UserName value
        /// </param>
        /// <param name="count">
        /// the number of records to be retrieved
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        List<UsersDB> SelectByUserName(string userName, int count);

        /// <summary>
        /// Retrieves a collection of items by PassHash
        /// </summary>
        /// <param name="passHash">
        /// PassHash value
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        List<UsersDB> SelectByPassHash(string passHash);

        /// <summary>
        /// Retrieves the first set of items specified by count by PassHash
        /// </summary>
        /// <param name="passHash">
        /// PassHash value
        /// </param>
        /// <param name="count">
        /// the number of records to be retrieved
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        List<UsersDB> SelectByPassHash(string passHash, int count);

        /// <summary>
        /// Retrieves a collection of items by Friends
        /// </summary>
        /// <param name="friends">
        /// Friends value
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        List<UsersDB> SelectByFriends(byte[] friends);

        /// <summary>
        /// Retrieves the first set of items specified by count by Friends
        /// </summary>
        /// <param name="friends">
        /// Friends value
        /// </param>
        /// <param name="count">
        /// the number of records to be retrieved
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        List<UsersDB> SelectByFriends(byte[] friends, int count);

		/// <summary>
		/// Delete records by UserID
		/// </summary>
		/// <param name="userId">
		/// UserID value
		/// </param>
		/// <returns>
		/// The <see cref="int"/>.
		/// </returns>
		int DeleteByUserId(int? userId);

		/// <summary>
		/// Delete records by UserName
		/// </summary>
		/// <param name="userName">
		/// UserName value
		/// </param>
		/// <returns>
		/// The <see cref="int"/>.
		/// </returns>
        int DeleteByUserName(string userName);

		/// <summary>
		/// Delete records by PassHash
		/// </summary>
		/// <param name="passHash">
		/// PassHash value
		/// </param>
		/// <returns>
		/// The <see cref="int"/>.
		/// </returns>
		int DeleteByPassHash(string passHash);

		/// <summary>
		/// Delete records by Friends
		/// </summary>
		/// <param name="friends">
		/// Friends value
		/// </param>
		/// <returns>
		/// The <see cref="int"/>.
		/// </returns>
		int DeleteByFriends(byte[] friends);

        /// <summary>
        /// Create new record without specifying a primary key
        /// </summary>
        /// <param name="userName">
        ///     The User Name.
        /// </param>
        /// <param name="passHash">
        ///     The Pass Hash.
        /// </param>
        /// <param name="friends">
        ///     The Friends.
        /// </param>
        void Create(string userName, string passHash, List<UserClass> friends);

		/// <summary>
		/// Create new record specifying all fields
		/// </summary>
		/// <param name="userId">
		/// The User ID.
		/// </param>
		/// <param name="userName">
		/// The User Name.
		/// </param>
		/// <param name="passHash">
		/// The Pass Hash.
		/// </param>
		/// <param name="friends">
		/// The Friends.
		/// </param>
		void Create(int? userId, string userName, string passHash, byte[] friends);
	}
}
