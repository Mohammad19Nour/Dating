using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dating_App.DTOs;
using Dating_App.Entities;
using Dating_App.Helpers;
using Dating_App.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dating_App.Data;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public UserRepository(DataContext context ,IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public void Update(AppUser user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }

    public async Task<bool> SaveChanges()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<AppUser> GetUserByUsernameAsync(string username)
    {
        return await _context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName == username.ToLower());
    }

    public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
    {
        return await _context.Users.Include(p => p.Photos).ToListAsync();
    }

    public async Task<PageList<MemberDto>> GetMembersAsync(UserParams userParams)
    {
        var query = _context.Users.AsQueryable();
        query = query.Where(u => u.UserName != userParams.CurrentUserName);
        query = query.Where(u => u.Gender == userParams.Gender);

        var minDob = DateTime.Today.AddYears(-userParams.MaxAge-1);
        var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

        query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

        query = userParams.OrderBy switch
        {
            "created" => query.OrderByDescending(u => u.Created),
            _ => query.OrderByDescending(u => u.LastActive)
        };
        
        var res = query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking();

        return await PageList<MemberDto>.CreateAsync(res , userParams.PageNumber , userParams.PageSize);

    }
}