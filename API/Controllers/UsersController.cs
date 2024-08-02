using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Entities;
using API.Data;



namespace API;


[ApiController]
[Route("api/[controller]")]

public class UsersController(DataContext context) : ControllerBase
{

    [HttpGet]

    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers() 
    {
        var users = await context.Users.ToListAsync();

        return users;
    }


    [HttpGet("{id:int}")]

    public async Task<ActionResult<AppUser>> GetUsers(int id)
    {
        var user = await context.Users.FindAsync(id);


        if (user == null) return NotFound();

        return user;
    }
}