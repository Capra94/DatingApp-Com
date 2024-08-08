using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Entities;
using API.Data;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using API.DTOs;



namespace API.Controllers;



[Authorize]
public class UsersController(IUserRepository userRepository) : BaseApiController
{



    [HttpGet]

    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
    {
        var users = await userRepository.GetMemebersAsync();



        return Ok(users);
    }



    [HttpGet("{username}")]

    public async Task<ActionResult<MemberDTO>> GetUsers(string username)
    {
        var user = await userRepository.GetMemberAsync(username);


        if (user == null) return NotFound();

        return user;
    }
}