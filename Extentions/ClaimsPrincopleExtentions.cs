using System.Security.Claims;

namespace Dating_App.Extentions;

public static class ClaimsPrincopleExtentions
{
    public static string GetUsername(this ClaimsPrincipal user)
    {
        return  user.FindFirst(ClaimTypes.NameIdentifier)?.Value; // get the username used in token
    }
}