using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcc_mypet_app.Services
{
    public class TokenService
    {
        public static string GetClaimValue(string token, string claimType)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken != null)
                {
                    var claim = jsonToken.Payload[claimType];
                    return claim?.ToString();
                }
            }
            catch (Exception ex)
            {
                // Logar ou manipular exceções (como tokens mal formados)
            }

            return null;
        }
        public static bool VerifyTokenValidity(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken != null)
                {
                    var expirationTime = jsonToken.ValidTo;
                    return expirationTime > DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                // Logar ou manipular exceções (como tokens mal formados)
            }

            return false;
        }
    }
}
