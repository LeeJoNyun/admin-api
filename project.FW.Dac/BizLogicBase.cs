using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Data;
using System.Data.Common;

namespace project.FW.Dac
{
    /// <summary>
    /// 비즈니스 로직
    /// </summary>
    public class BizLogicBase : IDisposable
    {
        protected Database DB = null;
        protected DbTransaction DbTrans = null;
        private DbConnection DbCon = null;

        protected BizLogicBase() : this("Master", false) { }
        protected BizLogicBase(string connectionString) : this(connectionString, false) { }
        protected BizLogicBase(bool isRequiredTransaction) : this("Master", isRequiredTransaction) { }
        protected BizLogicBase(string connectionString, bool isRequiredTransaction)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            DatabaseFactory.SetDatabaseProviderFactory(factory, false);
            DB = DatabaseFactory.CreateDatabase(connectionString);

            // 트랜젝션이 필요할 경우
            if (isRequiredTransaction)
            {
                DbCon = DB.CreateConnection();
            }
        }

        /// <summary>
        /// 트랜젝션 시작
        /// </summary>
        /// <returns></returns>
        protected Boolean BeginTransaction()
        {
            try
            {
                if (DbCon.State == ConnectionState.Closed) DbCon.Open();
                DbTrans = DbCon.BeginTransaction();
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 트랜젝션 취소
        /// </summary>
        /// <returns></returns>
        protected Boolean RollBackTransaction()
        {
            try
            {
                DbTrans.Rollback();
                if (DbCon.State == ConnectionState.Open) DbCon.Close();
                return false;
            }
            catch
            {
                return false;
            }
        }

        protected Boolean CommitTransaction()
        {
            try
            {
                DbTrans.Commit();
                if (DbCon.State == ConnectionState.Open) DbCon.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 자원 해제
        /// </summary>
        public void Dispose()
        {
            if (DbCon != null)
            {
                if (DbCon.State == ConnectionState.Open) DbCon.Close();
            }

            if (DB != null) DB = null;
            if (DbCon != null) DbCon = null;
            if (DbTrans != null) DbTrans = null;
        }
    }
}
