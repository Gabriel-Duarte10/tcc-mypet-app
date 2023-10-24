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
        public int? GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                return null;
            }

            // Supõe-se que a chave para o ID do usuário é "UserId" no corpo do token.
            // Se a chave for diferente, ajuste isso de acordo.
            var userIdClaim = jsonToken.Claims.First(claim => claim.Type == "Id");
            if (userIdClaim == null)
            {
                return null;
            }
            int userId = int.Parse(userIdClaim.Value);
            return userId;
        }
    }
}
