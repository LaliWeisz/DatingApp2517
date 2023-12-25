using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace API;


[ApiController]
[Route("api/[controller]")]
public class AccountController : BaseApiController
{
    private readonly DataContext context;
    private readonly ITokenService tokenService;

    public AccountController(DataContext dataContext, ITokenService tokenService)
    {
        this.context = dataContext;
        this.tokenService = tokenService;
    }

    [HttpPost("register")] //POST: /api/account/register
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
    {
        if (await UserExist(registerDto.UserName)) return BadRequest("user name is taken");

        using var hmac = new HMACSHA512();
        var user = new AppUser
        {
            UserName = registerDto.UserName.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };
        this.context.Add(user);
        await this.context.SaveChangesAsync();
        return new UserDto
        {
            Username = user.UserName,
            Token = this.tokenService.CreateToken(user)
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> login(LoginDto loginDto)
    {
        var user = await this.context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);
        if (user == null) return Unauthorized("invalid user name");
        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        for (int i = 0; i < computeHash.Length; i++)
            if (computeHash[i] != user.PasswordHash[i]) return Unauthorized("invalid password");
        return new UserDto
        {
            Username = user.UserName,
            Token = this.tokenService.CreateToken(user)
        };
    }

    private async Task<bool> UserExist(string userName)
    {
        return await this.context.Users.AnyAsync(x => x.UserName == userName.ToLower());
    }
}