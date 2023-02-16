using Microsoft.Practices.EnterpriseLibrary.Data;
using project.FW.Dac;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.API.DA
{
    public class Api_DA : DataAccessBase
    {
        public Api_DA(Database DB) : base(DB) { }
        public Api_DA(Database DB, DbTransaction DBTrans) : base(DB, DBTrans) { }

        #region 로그인
        public DataTable Login(string userid, string password)
        {
            DbCommand cmd = DB.GetStoredProcCommand("ASP_LOGIN_NTR");

            DB.AddInParameter(cmd, "@pMEMBER_ID", DbType.String, userid);
            DB.AddInParameter(cmd, "@pPASS_WD", DbType.String, password);

            return ExecuteDataSet(cmd).Tables[0];
        }
        #endregion

        #region 관리자 등록
        public int SetAdminRegister(string id, string password, string name, string email)
        {
            DbCommand cmd = DB.GetStoredProcCommand("ASP_ADMIN_REGISTER_TRX");

            DB.AddInParameter(cmd, "@pID", DbType.String, id);
            DB.AddInParameter(cmd, "@pPW", DbType.String, password);
            DB.AddInParameter(cmd, "@pNAME", DbType.String, name);
            DB.AddInParameter(cmd, "@pEMAIL", DbType.String, email);

            DB.AddOutParameter(cmd, "@oRETURN_NO", DbType.Int32, 4);

            ExecuteNonQuery(cmd);

            return Convert.ToInt32(DB.GetParameterValue(cmd, "@oRETURN_NO"));
        }
        #endregion
    }
}
