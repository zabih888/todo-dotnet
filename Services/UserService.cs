using Microsoft.AspNetCore.Identity;
using TodoApi.Data;
using TodoApi.Models;
using Microsoft.EntityFrameworkCore;

public class UserService
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher<User> _hasher = new();

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<User> CreateUser(string userName, string password)
    {
        var user = new User(){UserName = userName};
        user.PasswordHash = _hasher.HashPassword(user, password);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<User?> ValidateUser(string userName, string password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.UserName == userName);
        if(user == null) return null ;

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if(result ==PasswordVerificationResult.Failed)
        {
            return null;
        }
        return user;

    }
};