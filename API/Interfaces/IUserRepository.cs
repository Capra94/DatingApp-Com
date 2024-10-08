



using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API;


public interface IUserRepository
{
    void Update(AppUser user);
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser?> GetUserByIdAsync(int id);
    Task<AppUser?> GetUserByUsernameAsync(string username);

    Task<PagedList<MemberDTO>> GetMemebersAsync(UserParams userParams);

    Task<MemberDTO?> GetMemberAsync(String username);
    
}