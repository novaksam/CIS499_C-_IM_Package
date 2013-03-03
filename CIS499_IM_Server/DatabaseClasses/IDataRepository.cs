// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Sam Novak" file="IDataRepository.cs">
//   CIS499 - 2013 - IM Server
// </copyright>
// <summary>
//   Main Data Repository interface containing all table repositories
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CIS499_IM_Server.DatabaseClasses
{
    using System;
    using System.Data.SqlServerCe;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     Main Data Repository interface containing all table repositories
    /// </summary>
    public interface IDataRepository : IDisposable
    {
        #region Public Properties

        /// <summary>
        ///     Gets an instance of the IUsers_DBRepository
        /// </summary>
        IUsersDBRepository UsersDB { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Starts a SqlCeTransaction using the global SQLCE connection instance
        /// </summary>
        /// <returns>
        ///     The <see cref="SqlCeTransaction" />.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", 
            Justification = "Reviewed. Suppression is OK here.")]
        SqlCeTransaction BeginTransaction();

        /// <summary>
        ///     Commits the transaction
        /// </summary>
        void Commit();

        /// <summary>
        ///     Rollbacks the transaction
        /// </summary>
        void Rollback();

        #endregion
    }
}