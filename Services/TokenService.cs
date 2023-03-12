using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dating_App.Entities;
using Dating_App.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Dating_App.Services;

public class TokenService :ITokenService
{
    private readonly UserManager<AppUser> _userManager;
    private SymmetricSecurityKey _Key;
    
    public TokenService(IConfiguration config , UserManager<AppUser> userManager)
    {
        _userManager = userManager;
        _Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
    }

    public async Task<string> CreateToken(AppUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
        };

        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role , r)));
        
        var creds = new SigningCredentials(_Key , SecurityAlgorithms.HmacSha384Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = creds
        };

        var tokenHanlder = new JwtSecurityTokenHandler();

        var token = tokenHanlder.CreateToken(tokenDescriptor);

        return tokenHanlder.WriteToken(token);
    } 
}