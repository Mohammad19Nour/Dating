
using System.Security.Claims;
using AutoMapper;
using Dating_App.DTOs;
using Dating_App.Entities;
using Dating_App.Extentions;
using Dating_App.Helpers;
using Dating_App.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dating_App.Controllers;
[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserRepository _repo;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UsersController(IUserRepository repo , IMapper mapper, IPhotoService photoService)
    {
        _repo = repo;
        _mapper = mapper;
        _photoService = photoService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
    {
        userParams.CurrentUserName = User.GetUsername();
        var currentUser = await _repo.GetUserByUsernameAsync(userParams.CurrentUserName);

        if (string.IsNullOrEmpty(userParams.Gender))
            userParams.Gender = currentUser.Gender == "male" ? "femal" : "male";
        
        var users = await _repo.GetMembersAsync(userParams);

        Response.AddPaginationHeader(users.CurrentPage , users.PageSize , 
            users.TotalCount , users.TotalPages);
        

        return Ok(users);
    }
    [Authorize(Roles = "Member")]
    [HttpGet("{username}", Name = "GetUser")]
    public async Task< ActionResult<MemberDto>> GetUserByUserName(string username)
    {
        var user = await _repo.GetUserByUsernameAsync(username);
        var usr = _mapper.Map<MemberDto>(user);
        return usr;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdate)
    {
        var username = User.GetUsername();
        var user = await _repo.GetUserByUsernameAsync(username);
        _mapper.Map(memberUpdate, user);
        _repo.Update(user);

        if (await _repo.SaveChanges()) return NoContent();

        return BadRequest("Failed to update user ");
    }

    [HttpPost("add-photo")]

    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var username = User.GetUsername();
        var user = await _repo.GetUserByUsernameAsync(username);
        var result = await _photoService.AddPhotoAsync(file);

        if (result.Error != null) return BadRequest(result.Error.Message);
        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if (user.Photos.Count() == 0)
        {
            photo.IsMain = true;
        } 
        
        user.Photos.Add(photo);

        if (await _repo.SaveChanges())
        {
            return CreatedAtRoute("GetUser",new{username = user.UserName},_mapper.Map<PhotoDto>(photo));
        }

        return BadRequest("Problem with adding photo");
    }

    [HttpPut("set-main-photo")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await _repo.GetUserByUsernameAsync(User.GetUsername());
        var photo = user.Photos.First(x => x.Id == photoId);
        var currentMain = user.Photos.First(x => x.IsMain);

        currentMain.IsMain = false;
        photo.IsMain = true;
        if (await _repo.SaveChanges()) return NoContent();

        return BadRequest("Failed to set main photo");
    }

    [HttpDelete("delete-photo/{photoId}")]

    public async Task<ActionResult> DeletPhoto(int photoId)
    {
        var user = await _repo.GetUserByUsernameAsync(User.GetUsername());
        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null) return NotFound();

        if (photo.IsMain) return BadRequest("You cannot delete your main photo");
        if (photo.PublicId != null)
        {
          var result = await _photoService.DeletePhotoAsync(photo.PublicId);

          if (result.Error != null) return BadRequest(result.Error.Message);
          
        }
        user.Photos.Remove(photo);
        if (await _repo.SaveChanges()) return Ok();
        
        return BadRequest("Failed to delete photo");
    }
}