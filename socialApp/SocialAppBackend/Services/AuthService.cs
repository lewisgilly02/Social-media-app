using SocialAppBackend.Data;
using SocialAppBackend.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace SocialAppBackend.Services;

public class AuthService
{
    private readonly AppDbContext _db;

    public AuthService(AppDbContext db)
    {
        _db = db;
    }




    public async Task<RegisterResponseDto?> RegisterAsync(string username, string password)
    {   

        var existing = await _db.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (existing is not null) return null;

        string hash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {

            UserName = username,
            PasswordHash = hash
            
        };

        // user id is auto generated
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new RegisterResponseDto
        {
            UserName = user.UserName

        };
    }



    public async Task<LoginResponseDto?> LoginAsync(string username, string Incomingpassword)
    {
        var usernameExists = await _db.Users.FirstOrDefaultAsync(u => u.UserName == username);

        if (usernameExists is null) return null;

        bool passwordMatches = BCrypt.Net.BCrypt.Verify(Incomingpassword, usernameExists.PasswordHash);

        if (!passwordMatches) return null;

        return new LoginResponseDto
        {
            Token = GenerateToken(usernameExists)
        };

    }

    private string GenerateToken(User user)
    {
        var claims = new[]
        {   
            // JwtRegisteredClaimNames is basiaclly just constants for the claim key
            // its much safer than typing "sub", user.userid because typos
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
        };

        // convert secret key string into bytes, then wrap it in a security key object
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("secretkeyforsecrecywonttellanybody123lookatmeheeheeheesecrecyyoucantseefeeforemekeykeykeylongenoughformeeee")
        );

        // pair key with the algorithm to use it for signing

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "SocialApp",
            audience: "SocialApp",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials

        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

