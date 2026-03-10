using SocialAppBackend.Data;
using SocialAppBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualBasic;
using BCrypt.Net;

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
}