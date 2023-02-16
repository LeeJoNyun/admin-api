using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace project.FW.Dac
{
    /// <summary>
    /// DataAccess 로직 베이스
    /// </summary>
    public class DataAccessBase : IDisposable
    {
        protected Database DB = null;
        protected DbTransaction DbTrans = null;

        protected DataAccessBase() : this(null) { }
        protected DataAccessBase(Database pDB) : this(pDB, null) { }
        protected DataAccessBase(Database pDB, DbTransaction pDbTrans)
        {
            DB = pDB;
            DbTrans = pDbTrans;
        }

        /// <summary>
        /// 쿼리 커맨드 리턴
        /// </summary>
        /// <param name="pSql"></param>
        /// <returns></returns>
        protected DbCommand QueryCommand(string pSql)
        {
            return DB.GetSqlStringCommand(pSql);
        }

        protected DbCommand QueryCommand()
        {
            return DB.GetSqlStringCommand(" SELECT 1 ");
        }

        /// <summary>
        /// 프로시져 커맨트 리턴
        /// </summary>
        /// <param name="pProcName"></param>
        /// <returns></returns>
        protected DbCommand ProcedureCommand(string pProcName)
        {
            return DB.GetStoredProcCommand(pProcName);
        }

        /// <summary>
        /// 페이징 쿼리 객체 생성
        /// </summary>
        /// <param name="pSql"></param>
        /// <returns></returns>
        protected PagingQuery CreatePagingQueryObject(string pSql)
        {
            PagingQuery oPQ = new PagingQuery();
            oPQ.DB = this.DB;
            oPQ.QueryString = pSql;

            return oPQ;
        }

        protected PagingQuery CreatePagingQueryObject()
        {
            return CreatePagingQueryObject(String.Empty);
        }


        /// <summary>
        /// 쿼리를 <see cref="T:System.Data.DataSet"/> 형식으로 리턴한다.
        /// </summary>
        /// <param name="cmd"><see cref="T:System.Data.Common.DbCommand"/> 객체</param>
        /// <returns></returns>
        protected DataSet ExecuteDataSet(DbCommand cmd)
        {
            try
            {
                return DB.ExecuteDataSet(cmd);
            }
            catch (Exception Exp)
            {
                //EventLog.WriteWarning(Exp.Message);
                throw Exp;
            }
        }

        /// <summary>
        /// ExecuretScalar
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected Object ExecuretScalar(DbCommand cmd)
        {
            try
            {
                return DB.ExecuteDataSet(cmd);
            }
            catch (Exception Exp)
            {
                //EventLog.WriteWarning(Exp.Message);
                throw Exp;
            }
        }

        /// <summary>
        /// ExecuteNonQuery
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected int ExecuteNonQuery(DbCommand cmd)
        {
            try
            {
                return DB.ExecuteNonQuery(cmd);
            }
            catch (Exception Exp)
            {
                //EventLog.WriteWarning(Exp.Message);
                throw Exp;
            }
        }

        /// <summary>
        /// ExecuteNonQuery
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected int ExecuteNonQuery(DbCommand cmd, DbTransaction trn)
        {
            try
            {
                return DB.ExecuteNonQuery(cmd, trn);
            }
            catch (Exception Exp)
            {
                //EventLog.WriteWarning(Exp.Message);
                throw Exp;
            }
        }

        /// <summary>
        /// ExecuteMultiDataSet
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="querys"></param>
        /// <returns></returns>
        protected DataSet ExecuteMultiDataSet(DbCommand cmd, List<string> querys)
        {
            try
            {
                // SQL 작성
                StringBuilder sb = new StringBuilder();
                foreach (string sQuery in querys)
                {
                    sb.Append(sQuery);
                }

                cmd.CommandText = sb.ToString();
                return DB.ExecuteDataSet(cmd);
            }
            catch (Exception Exp)
            {
                throw Exp;
            }
        }

        /// <summary>
        /// ExecuteSingleDataSet
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="querys"></param>
        /// <returns></returns>
        protected DataSet ExecuteSingleDataSet(DbCommand cmd, List<string> querys)
        {
            try
            {
                // SQL 작성
                StringBuilder sb = new StringBuilder();
                int i = 0;
                foreach (string sQuery in querys)
                {
                    if (i > 0) sb.Append(" UNION ALL ");
                    sb.Append(sQuery);
                    i++;
                }

                cmd.CommandText = sb.ToString();
                return DB.ExecuteDataSet(cmd);
            }
            catch (Exception Exp)
            {
                throw Exp;
            }
        }

        /// <summary>
        /// 쿼리를 <see cref="T:System.Data.IDataReader"/> 형식으로 리턴한다.
        /// </summary>
        /// <param name="cmd"><see cref="T:System.Data.Common.DbCommand"/> 객체</param>
        /// <returns></returns>
        protected IDataReader ExecuteReader(DbCommand cmd)
        {
            try
            {
                return DB.ExecuteReader(cmd);
            }
            catch (Exception Exp)
            {
                throw Exp;
            }
        }

        protected Object NullToDBNull(object value)
        {
            if (value == null) return DBNull.Value;
            return value;
        }


        #region 페이징 쿼리 클래스

        /// <summary>
        ///  페이징 쿼리 클래스
        /// </summary>
        public class PagingQuery
        {
            private List<PagingParameters> ParamameterList = new List<PagingParameters>();
            public string QueryString;
            public Database DB = null;

            /// <summary>
            /// 파라미터 추가
            /// </summary>
            /// <param name="pName"></param>
            /// <param name="pDbType"></param>
            /// <param name="pValue"></param>
            public void AddParameter(string pName, DbType pDbType, Object pValue)
            {
                ParamameterList.Add(new PagingParameters { Name = pName, DbType = pDbType, Value = pValue });
            }


            /// <summary>
            /// 파라미터가 추가된 쿼리 얻기
            /// </summary>
            /// <returns></returns>
            private String GetQueryIncludeParameter()
            {
                StringBuilder oSB = new StringBuilder();

                foreach (PagingParameters pp in ParamameterList)
                {
                    string dataType = "";

                    // 선언부 추가
                    switch (pp.DbType)
                    {
                        case DbType.AnsiString:
                        case DbType.AnsiStringFixedLength:
                        case DbType.String:
                            dataType = "VARCHAR(MAX)";
                            break;

                        case DbType.Byte:
                        case DbType.SByte:
                            dataType = "TINYINT";
                            break;

                        case DbType.Int16:
                        case DbType.UInt16:
                        case DbType.Single:
                            dataType = "SMALL";
                            break;

                        case DbType.Int32:
                        case DbType.UInt32:
                            dataType = "INT";
                            break;

                        case DbType.Int64:
                        case DbType.UInt64:
                            dataType = "BIGINT";
                            break;

                        case DbType.Currency:
                            dataType = "MONEY";
                            break;

                        case DbType.Decimal:
                        case DbType.VarNumeric:
                            dataType = "NUMERIC";
                            break;

                        case DbType.Double:
                            dataType = "FLOAT";
                            break;
                    }

                    oSB.Append(String.Format("DECLARE {0} {1};", pp.Name, dataType));

                    if (pp.Value == null) pp.Value = String.Empty;
                    oSB.Append(String.Format("SET {0}={1};", pp.Name, (dataType == "VARCHAR(MAX)") ? "'" + pp.Value.ToString() + "'" : pp.Value.ToString()));

                }

                //oSB.Append(QueryString);

                return oSB.ToString();

            }

            /// <summary>
            /// 페이징 쿼리 파라미터 클래스
            /// </summary>
            private class PagingParameters
            {
                public string Name;
                public DbType DbType;
                public Object Value;
            }
        }

        #endregion

        #region IDataReader 변환 관련 함수

        protected List<T> DataReaderToList<T>(IDataReader reader, Func<IDataReader, T> convertFunc)
        {
            List<T> returnData = new List<T>();

            while (reader.Read())
            {
                returnData.Add(convertFunc(reader));
            }

            return returnData;
        }


        #endregion

        public void Dispose()
        {
        }
    }
}
