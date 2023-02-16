using project.API.DA;
using project_admin.Common;
using project_admin.Jwt;
using project_admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;

namespace project_admin.Controllers
{
    public class LoginController : ApiController
    {
        /// <remarks>
        /// 로그인
        /// 
        ///     {
        ///         "userid" : "admin",
        ///         "password" : "1234"
        ///     }
        ///     
        /// </remarks>
        /// <returns></returns>
        [Route("Login")]
        public object Post([FromBody] Login li)
        {
            DataTable dt = null;

            string pw = Crypto.fnGetSHA512(li.password);

            using (var biz = new Api_Biz())
            {
                dt = biz.Login(li.userid, pw);
            }
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (dt.Rows.Count > 0)
            {
                dic["success"] = true;
                dic["memberToken"] = clsJWT.createToken(
                                            Convert.ToInt32(dt.Rows[0]["ad_id"].ToString()),
                                            dt.Rows[0]["email"].ToString(),
                                            dt.Rows[0]["name"].ToString()

                                            );
            }
            else
            {
                dic["success"] = false;
                dic["message"] = "no data";
            }

            return dic;
        }
    }
}
