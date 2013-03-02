// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Sam Novak" file="DataRepository.cs">
//   CIS499 - 2013 - IM Server
// </copyright>
// <summary>
//   Main Data Repository implementation containing all default table repositories implementations
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace CIS499_IM_Server.DatabaseClasses
{
    using System.Data.SqlServerCe;

    /// <summary>
	/// Main Data Repository implementation containing all default table repositories implementations
	/// </summary>
	public class DataRepository : IDataRepository
	{
        /// <summary>
        /// The transaction.
        /// </summary>
        private SqlCeTransaction transaction;

		/// <summary>
		/// Initializes a new instance of the <see cref="DataRepository"/> class. 
		/// Creates an instance of DataRepository
		/// </summary>
		public DataRepository()
		{
			this.UsersDB = new UsersDBRepository();
		}

		/// <summary>
		/// Gets an instance of the IUsers_DBRepository
		/// </summary>
		public IUsersDBRepository UsersDB { get; private set; }

		/// <summary>
		/// Starts a Transaction using the Connection instance
		/// </summary>
		/// <returns>
		/// The <see cref="SqlCeTransaction"/>.
		/// </returns>
		public SqlCeTransaction BeginTransaction()
		{
		    if (this.transaction != null)
		    {
		        throw new System.InvalidOperationException(
		            "A transaction has already been started. Only one transaction is allowed");
		    }

		    this.transaction = EntityBase.Connection.BeginTransaction();
		    this.UsersDB.Transaction = this.transaction;
			return this.transaction;
		}

		/// <summary>
		/// Commits the transaction
		/// </summary>
		public void Commit()
		{
		    if (this.transaction == null)
		    {
		        throw new System.InvalidOperationException("No transaction has been started");
		    }

		    this.transaction.Commit();
		}

		/// <summary>
		/// Rollbacks the transaction
		/// </summary>
		public void Rollback()
		{
		    if (this.transaction == null)
		    {
		        throw new System.InvalidOperationException("No transaction has been started");
		    }

		    this.transaction.Rollback();
		}

		/// <summary>
		/// Releases the resources used. All uncommitted transactions are rolled back
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
		}

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
				if (this.transaction != null)
				{
					this.transaction.Dispose();
					this.transaction = null;
				}
			}

			this.disposed = true;
		}

        /// <summary>
        /// The disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Finalizes an instance of the <see cref="DataRepository"/> class. 
        /// </summary>
        ~DataRepository()
		{
			this.Dispose(false);
		}
	}
}
