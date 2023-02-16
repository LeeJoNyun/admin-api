using project.FW.Dac;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.API.DA
{
    public class Api_Biz : BizLogicBase
    {
        public Api_Biz() : base("my_DB", true) { }

        #region 로그인
        public DataTable Login(string userid, string password)
        {
            DataTable dt = null;

            try
            {
                using (var da = new Api_DA(DB))
                {
                    dt = da.Login(userid, password);
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }

            return dt;
        }
        #endregion

        #region 관리자 등록
        public int SetAdminRegister(string id, string password, string name, string email)
        {
            int result = 0;
            try
            {
                using (var da = new Api_DA(DB))
                {
                    BeginTransaction();
                    result = da.SetAdminRegister(id, password, name, email);
                    CommitTransaction();
                }

            }
            catch (Exception exp)
            {
                RollBackTransaction();
                return result;
            }

            return result;
        }
        #endregion
    }
}
