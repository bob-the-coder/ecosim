using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Workflow.Enum;
using BusinessLogic.Workflow.Interfaces;
using DataLayer;
using DataLayer.Implementation;
using DataLayer.Repositories;

namespace BusinessLogic.Workflow
{
    internal class RepoUnitOfWork : IUnitOfWork
    {
        private const string ERROR_MESSAGE_TRANSACTION_NOT_FINALIZED = "The transaction was not finalized by the user";

        private static IDictionary<Type, Func<BaseDataRepository>> mRepositories;
        private readonly UnitOfWorkMode mMode;
        private Entities mContext;

        private DbContextTransaction mTransaction;
        private bool mIsTransactionFinalized;

        private readonly Type mScope;

        static RepoUnitOfWork()
        {
            InitializeRepositoriesIoc();
        }

        private RepoUnitOfWork(UnitOfWorkMode mode)
        {
            mMode = mode;
            InitializeUnitOfWork(mode);
        }

        private RepoUnitOfWork(UnitOfWorkMode mode, Type scope) : this(mode)
        {
            mScope = scope;
        }

        #region Factory logic

        /// <summary>
        /// Instantiate an object of type RepoUnitOfWork without a scope for logging purposes.
        /// This UnitOfWork will track object changes.
        /// </summary>
        /// <returns>An instance of the RepoUnitOfWork class</returns>
        public static RepoUnitOfWork NewTracking() => new RepoUnitOfWork(UnitOfWorkMode.Tracking);

        /// <summary>
        /// Instantiate an object of type RepoUnitOfWork using a scope for logging purposes.
        /// This UnitOfWork will track object changes.
        /// </summary>
        /// <returns>An instance of the RepoUnitOfWork class</returns>
        public static RepoUnitOfWork NewTracking<T>() where T : class => new RepoUnitOfWork(UnitOfWorkMode.Tracking, typeof(T));

        /// <summary>
        /// Instantiate an object of type RepoUnitOfWork without a scope for logging purposes.
        /// This UnitOfWork will not track object changes.
        /// </summary>
        /// <returns>An instance of the RepoUnitOfWork class</returns>
        public static RepoUnitOfWork New() => new RepoUnitOfWork(UnitOfWorkMode.NoTracking);

        /// <summary>
        /// Instantiate an object of type RepoUnitOfWork using a scope for logging purposes.
        /// This UnitOfWork will not track object changes.
        /// </summary>
        /// <returns>An instance of the RepoUnitOfWork class</returns>
        public static RepoUnitOfWork New<T>() where T : class => new RepoUnitOfWork(UnitOfWorkMode.NoTracking, typeof(T));

        #endregion

        #region Disposing Logic

        public void Dispose()
        {
            if (mMode == UnitOfWorkMode.Tracking && mTransaction != null)
            {
                mTransaction.Dispose();
                mTransaction = null;

                if (!mIsTransactionFinalized)
                {
                    if (mScope != null)
                    {
                        //Transaction not finalized
                    }
                    else
                    {
                        //Transaction not finalized
                    }
                }
            }

            if (mContext != null)
            {
                mContext.Dispose();
                mContext = null;
            }
        }

        #endregion

        #region Instance Repo Factory logic

        public T Repository<T>() where T : BaseDataRepository
        {
            var type = typeof(T);
            if (!mRepositories.ContainsKey(type))
            {
                throw new Exception("Repository not mapped!");
            }

            var repository = (T)mRepositories[type]();

            repository.Context = mContext;
            repository.IsEntityTrackingOn = mMode == UnitOfWorkMode.Tracking;

            return repository;
        }

        #endregion

        #region Static Repo Factory logic

        public static T CreateRepository<T>() where T : BaseDataRepository
        {
            var type = typeof(T);
            if (!mRepositories.ContainsKey(type))
            {
                throw new Exception("Repository not mapped!");
            }

            var repository = (T)mRepositories[type]();
            repository.IsEntityTrackingOn = false;

            return repository;
        }

        public static T CreateTrackingRepository<T>() where T : BaseDataRepository
        {
            var type = typeof(T);
            if (!mRepositories.ContainsKey(type))
            {
                throw new Exception("Repository not mapped!");
            }

            var repository = (T)mRepositories[type]();
            repository.IsEntityTrackingOn = true;

            return repository;
        }

        #endregion

        #region Transactions logic

        public void FinalizeTransaction(bool isTransactionSuccessful)
        {
            if (isTransactionSuccessful)
            {
                CommitTransaction();
            }
            else
            {
                RollbackTransaction();
            }
        }

        public void CommitTransaction()
        {
            if (mTransaction == null)
            {
                throw new NullReferenceException("An SQL transaction was not initialized to run the Commit action");
            }

            if (mIsTransactionFinalized)
            {
                throw new InvalidOperationException("Cannot commit a transaction that was already commited or aborted");
            }

            mIsTransactionFinalized = true;
            mTransaction.Commit();
        }

        public void RollbackTransaction()
        {
            if (mTransaction == null)
            {
                throw new NullReferenceException("An SQL transaction was not initialized to run the Rollback action");
            }

            if (mIsTransactionFinalized)
            {
                throw new InvalidOperationException("Cannot rollback a transaction that was already commited or aborted");
            }

            mIsTransactionFinalized = true;
            mTransaction.Rollback();
        }

        #endregion

        #region Initialization

        private void InitializeUnitOfWork(UnitOfWorkMode mode)
        {
            mContext = new Entities();

            if (mRepositories == null)
            {
                InitializeRepositoriesIoc();
            }

            if (mode == UnitOfWorkMode.Tracking)
            {
                mTransaction = mContext.Database.BeginTransaction();
            }
        }

        private static void InitializeRepositoriesIoc()
        {
            mRepositories = new Dictionary<Type, Func<BaseDataRepository>>
           {
                {typeof(NodeRepository), () => new NodeRepository() },
                {typeof(NeedRepository), () => new NeedRepository() },
                {typeof(NodeLinkRepository), () => new NodeLinkRepository() },
                {typeof(ProductRepository), () => new ProductRepository() },
                {typeof(ProductionRepository), () => new ProductionRepository() },
                {typeof(DecisionRepository), () => new DecisionRepository() },
                {typeof(DecisionChanceRepository), () => new DecisionChanceRepository() },
                {typeof(SimulationRepository), () => new SimulationRepository() },
                {typeof(SimulationLogRepository), () => new SimulationLogRepository() },
           };
        }

        #endregion
    }
}
