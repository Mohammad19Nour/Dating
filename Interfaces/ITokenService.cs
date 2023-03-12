using Dating_App.Entities;

namespace Dating_App.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(AppUser user);
}