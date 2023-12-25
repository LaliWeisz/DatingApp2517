using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API;

public class TokenService : ITokenService
{
  private readonly SymmetricSecurityKey _key;

  public TokenService(IConfiguration config)
  {
    _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
  }

  public string CreateToken(AppUser user)
  {
    var claims = new List<Claim>//הפרטים האישיים של היוזר להכנסה לתוכן הטוקן
    {
      new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
    };

    var cred = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);//הראש של הטוקן

    var tokenDescriptor = new SecurityTokenDescriptor//התוכן של הטוקן
    {
      Subject = new ClaimsIdentity(claims),
      Expires = DateTime.Now.AddDays(7),
      SigningCredentials = cred
    };

    var tokenHandler = new JwtSecurityTokenHandler();//החתימה של הטוקן

    var token = tokenHandler.CreateToken(tokenDescriptor);//יצירת הטוקן המלא
    return tokenHandler.WriteToken(token);
  }
}
