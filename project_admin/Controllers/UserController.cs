using project.API.DA;
using project_admin.Common;
using project_admin.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace project_admin.Controllers
{
    public class UserController : ApiController
    {
        /// <remarks>
        /// 관리자 등록
        /// 
        ///     {
        ///         "userid" : "admin",
        ///         "password" : "1234",
        ///         "name" : "김아무개",
        ///         "email" : "abcd2@naver.com"
        ///     }
        ///     
        /// </remarks>
        /// <returns></returns>
        [Route("User")]
        public object Post([FromBody] User u)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            int result = 0;
            string pw = Crypto.fnGetSHA512(u.password);
            try
            {
                using (var biz = new Api_Biz())
                {
                    result = biz.SetAdminRegister(u.userid, pw, u.name, u.email);

                }
            }
            catch (Exception e)
            {
                dic["success"] = false;
                dic["message"] = e.ToString();
                return dic;
            }

            if(result == 0)
            {
                dic["success"] = true;
                dic["message"] = "가입완료";
            }
            else if (result == -2)
            {
                dic["success"] = false;
                dic["message"] = "이미 사용중인 아이디 입니다";
            }

            return dic;

            
        }

    }
}
