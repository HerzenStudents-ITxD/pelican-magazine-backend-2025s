using Backend.DataAccess;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class ArticleModerationRepository
{
    private readonly ApplicationDbContext _context;

    public ArticleModerationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DbArticleModeration>> GetAllAsync()
    {
        return await _context.ArticleModerations
            .Include(m => m.Article)
            .Include(m => m.Moderator)
            .ToListAsync();
    }

    public async Task<DbArticleModeration?> GetByIdAsync(Guid id)
    {
        return await _context.ArticleModerations
            .Include(m => m.Article)
            .Include(m => m.Moderator)
            .FirstOrDefaultAsync(m => m.ModerationId == id);
    }

    public async Task<List<DbArticleModeration>> GetByArticleIdAsync(Guid articleId)
    {
        return await _context.ArticleModerations
            .Where(m => m.ArticleId == articleId)
            .Include(m => m.Moderator)
            .OrderByDescending(m => m.ModerationDate)
            .ToListAsync();
    }

    public async Task<List<DbArticleModeration>> GetByModeratorIdAsync(Guid moderatorId)
    {
        return await _context.ArticleModerations
            .Where(m => m.ModeratorId == moderatorId)
            .Include(m => m.Article)
            .OrderByDescending(m => m.ModerationDate)
            .ToListAsync();
    }

    public async Task<List<DbArticleModeration>> GetPendingModerationsAsync()
    {
        return await _context.ArticleModerations
            .Where(m => m.Status == ModerationStatus.Pending)
            .Include(m => m.Article)
            .Include(m => m.Article.Author)
            .ToListAsync();
    }

    public async Task AddAsync(DbArticleModeration moderation)
    {
        await _context.ArticleModerations.AddAsync(moderation);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DbArticleModeration moderation)
    {
        _context.ArticleModerations.Update(moderation);
        await _context.SaveChangesAsync();
    }

    public async Task<DbArticleModeration?> GetLatestByArticleIdAsync(Guid articleId)
    {
        return await _context.ArticleModerations
            .Where(m => m.ArticleId == articleId)
            .OrderByDescending(m => m.ModerationDate)
            .FirstOrDefaultAsync();
    }
}