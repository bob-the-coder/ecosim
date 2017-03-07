using System;
using Logging;
namespace DatabaseHandler.Repositories
{
    public abstract class BaseDataRepository : IDisposable
    {
        private const string CONTEXT_NULL_REFERENCE_EXCEPTION_MESSAGE = "Tried to use repository with null context";
        private EcoSimEntities mContext;

        public abstract bool IsEntityTrackingOn { get; set; }

        public virtual EcoSimEntities Context
        {
            get { return mContext; }
            set
            {
                if (value == null)
                {
                    LogHelper.LogException<BaseDataRepository>(CONTEXT_NULL_REFERENCE_EXCEPTION_MESSAGE);
                    throw new NullReferenceException(CONTEXT_NULL_REFERENCE_EXCEPTION_MESSAGE);
                }

                value.Configuration.LazyLoadingEnabled = false;

                mContext = value;
            }
        }

        #region Disposing logic

        public void Dispose()
        {
            Context.Dispose();
        }

        #endregion
    }
}