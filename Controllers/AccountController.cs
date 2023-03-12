using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Dating_App.Data;
using Dating_App.DTOs;
using Dating_App.Entities;
using Dating_App.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dating_App.Controllers;

public class AccountController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser>signInManager, ITokenService tokenService, IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto register)
    {
        if (await UserExists(register.UserName)) return BadRequest("Username is taken");

        var user = _mapper.Map<AppUser>(register);

        user.UserName = register.UserName.ToLower();

        var res = await _userManager.CreateAsync(user, register.Password);

        if (!res.Succeeded) return BadRequest(res.Errors);

        var roleRes = await _userManager.AddToRoleAsync(user, "Member");

        if (roleRes.Succeeded == false) return BadRequest(roleRes.Errors);

        return new UserDto
        {
            UserName = user.UserName,
            Token = await _tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    private async Task<bool> UserExists(String username)
    {
        return await
            _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        if (await UserExists(loginDto.UserName)) ;

        var user = await _userManager.Users.Include(p => p.Photos)
            .FirstOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());

        if (user == null) return Unauthorized("Invalid username");

        var res = await _signInManager.CheckPasswordSignInAsync(user,loginDto.Password , false);

        if (!res.Succeeded) return Unauthorized();
        
        return new UserDto
        {
            UserName = user.UserName,
            Token = await _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
        ;
    }
}