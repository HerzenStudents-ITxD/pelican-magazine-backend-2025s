using Backend.DataAccess;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class ArticleThemeRepository
{
    private readonly ApplicationDbContext _context;

    public ArticleThemeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DbArticleTheme>> GetAllAsync()
    {
        return await _context.ArticleThemes
            .Include(at => at.Article)
            .Include(at => at.Theme)
            .ToListAsync();
    }

    public async Task<DbArticleTheme?> GetByIdAsync(int id)
    {
        return await _context.ArticleThemes
            .Include(at => at.Article)
            .Include(at => at.Theme)
            .FirstOrDefaultAsync(at => at.ArticleThemeId == id);
    }

    public async Task AddAsync(DbArticleTheme articleTheme)
    {
        await _context.ArticleThemes.AddAsync(articleTheme);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DbArticleTheme articleTheme)
    {
        _context.ArticleThemes.Update(articleTheme);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var articleTheme = await GetByIdAsync(id);
        if (articleTheme != null)
        {
            _context.ArticleThemes.Remove(articleTheme);
            await _context.SaveChangesAsync();
        }
    }
}