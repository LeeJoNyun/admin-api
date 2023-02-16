using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace project_admin.Jwt
{
    public static class clsJWT
    {
        static string plainTextSecurityKey = "0123456789abcdefghijklmnopqrstuvwxyz";
        static InMemorySymmetricSecurityKey signingKey = new InMemorySymmetricSecurityKey(Encoding.UTF8.GetBytes(plainTextSecurityKey));
        static SigningCredentials signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);
        static int lifetimeMinutes = 43200; //30일

        public static string createToken(int memberIdx,
                                         string memberId,
                                         string memberName)
        //string birth,
        //string sidoGrade,
        //string gunguGrade,
        //int sidoCode,
        //int gunguCode,
        //string sidoName,
        //string gunguName,
        //string phone,
        //string gender,
        //int clubIdx,
        //string clubName,
        //string name,
        //string profilePath
        //)
        {
            var claimsIdentity = new ClaimsIdentity(new List<Claim>()
           {
               new Claim("memberIdx", memberIdx.ToString()),
               new Claim("memberId", memberId),
               new Claim("memberName", memberName),
               //new Claim("birth",birth.ToString()),
               //new Claim("sido", sidoGrade),
               //new Claim("gungu", gunguGrade),
               //new Claim("sidoCode", sidoCode.ToString()),
               //new Claim("gunguCode", gunguCode.ToString()),
               //new Claim("sidoName", sidoName.ToString()),
               //new Claim("gunguName", gunguName.ToString()),
               //new Claim("phone", phone),
               //new Claim("gender", gender.ToString()),
               //new Claim("clubIdx", clubIdx.ToString()),
               //new Claim("clubName", clubName.ToString()),
               //new Claim("name", name.ToString()),
               //new Claim("profilePath", profilePath.ToString())
           }, "UserInfo"); ;


            var securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                AppliesToAddress = ConfigurationManager.AppSettings["MainUrl"],
                //TokenIssuerName = "Roacket",
                TokenIssuerName = "ADMIN",
                Subject = claimsIdentity,
                SigningCredentials = signingCredentials,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.TokenLifetimeInMinutes = lifetimeMinutes;
            var plainToken = tokenHandler.CreateToken(securityTokenDescriptor);
            var signedAndEncodeToken = tokenHandler.WriteToken(plainToken);

            return signedAndEncodeToken;
        }

        internal static object isValidToken(object jToken)
        {
            throw new System.NotImplementedException();
        }

        public static JwtSecurityToken isValidToken(string signedAndEncodedToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidAudiences = new string[]
                {
                ConfigurationManager.AppSettings["MainUrl"],
                },
                ValidIssuers = new string[]
                {
                //"Roacket"
                "ADMIN"
                },
                IssuerSigningKey = signingKey
            };

            SecurityToken validatedToken;

            tokenHandler.ValidateToken(signedAndEncodedToken,
                tokenValidationParameters, out validatedToken);

            return validatedToken as JwtSecurityToken;
        }

        public static JwtSecurityToken readToken(string signedAndEncodedToken)
        {

            var tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken validatedToken;

            validatedToken = tokenHandler.ReadToken(signedAndEncodedToken);

            return validatedToken as JwtSecurityToken;

        }
    }
}