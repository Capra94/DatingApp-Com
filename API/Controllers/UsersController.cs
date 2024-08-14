using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using API.DTOs;
using System.Security.Claims;
using API.Interfaces;
using API.Extensions;
using Microsoft.AspNetCore.Components.Web;
using API.Entities;
using API.Helpers;



namespace API.Controllers;



[Authorize]
public class UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService) : BaseApiController
{

    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers([FromQuery]UserParams userParams)
    {

        userParams.CurrentUsername = User.GetUsername();

        var users = await userRepository.GetMemebersAsync(userParams);

        Response.AddPaginationHeader(users);

        return Ok(users);
    }


    
    [HttpGet("{username}")]

    public async Task<ActionResult<MemberDTO>> GetUsers(string username)
    {
        var user = await userRepository.GetMemberAsync(username);

        if (user == null)
        {
            return NotFound();
        };

        return user;
    }


    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
    {

        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null)
        {
            return BadRequest("Could not find user");
        };

        mapper.Map(memberUpdateDTO, user);

        if (await userRepository.SaveAllAsync())
        {
            return NoContent();
        };

        return BadRequest("Failed to update the user");
    }



    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null)
        {
            return BadRequest("Cannot update user");
        };

        var result = await photoService.AddPhotoAsync(file);

        if (result.Error != null)
        {
            return BadRequest(result.Error.Message);
        };


        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if(user.Photos.Count == 0)
        {
            photo.IsMain = true;
        }

        user.Photos.Add(photo);

        if (await userRepository.SaveAllAsync())
        {
            return CreatedAtAction(nameof(GetUsers),
                new { username = user.UserName }, mapper.Map<PhotoDto>(photo));
        };

        return BadRequest("There was a problem with adding your photo.");

    }



    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null)
        {
            return BadRequest("Could not find user");
        }

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null || photo.IsMain)
        {
            return BadRequest("Cannot use this as main photo");
        }

        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
        if (currentMain != null)
        {
            currentMain.IsMain = false;
        }

        photo.IsMain = true;


        if (await userRepository.SaveAllAsync())
        {
            return NoContent();
        }

        return BadRequest("Problem setting main photo");
    }

    [HttpDelete("delete-photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null)
        {
            return BadRequest("User could not be found");
        }

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null || photo.IsMain)
        {
            return BadRequest("This Photo cannot be deleted");
        }

        if (photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);

            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }
        }

        user.Photos.Remove(photo);

        if (await userRepository.SaveAllAsync())
        {
            return Ok();
        }

        return BadRequest("Problem with deleting Photo");
    }
}