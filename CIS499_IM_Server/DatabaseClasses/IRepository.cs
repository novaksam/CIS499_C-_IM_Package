// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Sam Novak" file="IRepository.cs">
//   CIS499 - 2013 - IM Server
// </copyright>
// <summary>
//   Base Repository interface defining the basic and commonly used data access methods
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CIS499_IM_Server.DatabaseClasses
{
    using System.Collections.Generic;

    /// <summary>
    /// Base Repository interface defining the basic and commonly used data access methods
    /// </summary>
    /// <typeparam name="T">
    /// Object type
    /// </typeparam>
    public interface IRepository<T>
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the number of records in the table
        /// </summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        int Count();

        /// <summary>
        /// Inserts the item to the table
        /// </summary>
        /// <param name="item">
        /// Item to be inserted to the database
        /// </param>
        void Create(T item);

        /// <summary>
        /// Populates the table with a collection of items
        /// </summary>
        /// <param name="items">
        /// Items to be inserted to the database
        /// </param>
        void Create(IEnumerable<T> items);

        /// <summary>
        /// Deletes the item
        /// </summary>
        /// <param name="item">
        /// Item to be deleted from the database
        /// </param>
        void Delete(T item);

        /// <summary>
        /// Deletes a collection of item
        /// </summary>
        /// <param name="items">
        /// Items to be deleted from the database
        /// </param>
        void Delete(IEnumerable<T> items);

        /// <summary>
        ///     Purges the contents of the table
        /// </summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        int Purge();

        /// <summary>
        ///     Retrieves all items as an array of T
        /// </summary>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>T[]</cref>
        ///     </see>
        ///     .
        /// </returns>
        T[] ToArray();

        /// <summary>
        /// Retrieves the first set of items specific by count as an array of T
        /// </summary>
        /// <param name="count">
        /// Number of records to be retrieved
        /// </param>
        /// <returns>
        /// The
        ///     <see>
        ///         <cref>T[]</cref>
        ///     </see>
        ///     .
        /// </returns>
        T[] ToArray(int count);

        /// <summary>
        ///     Retrieves all items as a generic collection
        /// </summary>
        /// <returns>
        ///     The <see cref="List{T}" />.
        /// </returns>
        List<T> ToList();

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
        List<T> ToList(int count);

        /// <summary>
        /// Updates the item
        /// </summary>
        /// <param name="item">
        /// Item to be updated on the database
        /// </param>
        void Update(T item);

        /// <summary>
        /// Updates a collection items
        /// </summary>
        /// <param name="items">
        /// Items to be updated on the database
        /// </param>
        void Update(IEnumerable<T> items);

        #endregion
    }
}