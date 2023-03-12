using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Dating_App.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dating_App.Data;

public class Seed
{
    public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        if (await userManager.Users.AnyAsync()) return;

        var userDate = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
        var users = JsonSerializer.Deserialize<List<AppUser>>(userDate);

        var roles = new List<AppRole>
        {
            new AppRole{Name = "Member"},
            new AppRole{Name = "Admin"},
            new AppRole{Name = "Moder"},
            
        };

        foreach (var role in roles)
        {
           await roleManager.CreateAsync(role);
        }
        foreach (var user in users)
        {
            user.UserName = user.UserName.ToLower();
          await userManager.CreateAsync(user , "Pa$0rd");
          await userManager.AddToRoleAsync(user,"Member");
        }

        var admin = new AppUser
        {
            UserName = "admin"
        };

        await userManager.CreateAsync(admin, "Admin1.");
        await userManager.AddToRolesAsync(admin,new [] {"Admin" , "Moder"});
    }
}