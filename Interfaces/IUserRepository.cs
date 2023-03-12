using Dating_App.DTOs;
using Dating_App.Entities;
using Dating_App.Helpers;

namespace Dating_App.Interfaces;

public interface IUserRepository
{
    void Update(AppUser user);
    public Task<bool> SaveChanges();
    public Task<AppUser> GetUserByUsernameAsync(string username);
    public Task<IEnumerable<AppUser>> GetAllUsersAsync();

    public Task<PageList<MemberDto>> GetMembersAsync(UserParams userParams);
}