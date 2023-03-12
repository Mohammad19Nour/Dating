using Microsoft.AspNetCore.Identity;

namespace Dating_App.Entities;

public class AppRole : IdentityRole<int>
{
    public ICollection<AppUserRole> userRoles { get; set; }
}