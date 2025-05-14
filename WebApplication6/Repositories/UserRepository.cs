using Backend.Contracts.Requests;
using Backend.DataAccess;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class UserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DbUser>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<DbUser?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task AddAsync(DbUser user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DbUser user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<DbUser?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<DbUser?> GetByVerificationTokenAsync(string token)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.EmailVerificationToken == token);
    }

    public async Task UpdateProfileAsync(Guid userId, UpdateUserRequest request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return;

        if (request.Nickname != null) user.Nickname = request.Nickname;
        if (request.Year != null) user.Year = request.Year;
        if (request.Course != null) user.Course = request.Course;
        if (request.Degree != null) user.Degree = request.Degree;
        if (request.UserType.HasValue)
        {
            user.UserType = request.UserType.Value;
        }

        await _context.SaveChangesAsync();
    }

    public async Task SetAdminStatusAsync(Guid userId, bool isAdmin)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.IsAdmin = isAdmin;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateAvatarAsync(Guid userId, string avatarUrl)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.ProfileImg = avatarUrl;
            await _context.SaveChangesAsync();
        }
    }
}