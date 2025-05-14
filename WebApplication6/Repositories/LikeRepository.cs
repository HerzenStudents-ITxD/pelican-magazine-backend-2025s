using Backend.DataAccess;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class LikeRepository
{
    private readonly ApplicationDbContext _context;

    public LikeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DbLike>> GetAllAsync()
    {
        return await _context.Likes
            .Include(l => l.User)
            .Include(l => l.Article)
            .ToListAsync();
    }

    public async Task<DbLike?> GetByIdAsync(Guid id)
    {
        return await _context.Likes
            .Include(l => l.User)
            .Include(l => l.Article)
            .FirstOrDefaultAsync(l => l.LikeId == id);
    }
    public async Task<DbLike?> GetByUserAndArticleAsync(Guid userId, Guid articleId)
    {
        return await _context.Likes
            .FirstOrDefaultAsync(l => l.UserId == userId && l.ArticleId == articleId);
    }

    public async Task<List<DbLike>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Likes
            .Include(l => l.Article)
            .Where(l => l.UserId == userId)
            .ToListAsync();
    }


    public async Task AddAsync(DbLike like)
    {
        like.LikedAt = DateTime.UtcNow;
        await _context.Likes.AddAsync(like);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var like = await GetByIdAsync(id);
        if (like != null)
        {
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid userId, Guid articleId)
    {
        return await _context.Likes
            .AnyAsync(l => l.UserId == userId && l.ArticleId == articleId);
    }

    public async Task<int> GetCountForArticleAsync(Guid articleId)
    {
        return await _context.Likes
            .CountAsync(l => l.ArticleId == articleId);
    }
}