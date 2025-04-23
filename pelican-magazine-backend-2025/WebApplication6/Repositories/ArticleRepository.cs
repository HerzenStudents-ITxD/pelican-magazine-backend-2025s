using Backend.DataAccess;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class ArticleRepository
{
    private readonly ApplicationDbContext _context;

    public ArticleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DbArticle>> GetAllAsync()
    {
        return await _context.Articles
            .Include(a => a.Author)
            .ToListAsync();
    }

    public async Task<DbArticle?> GetByIdAsync(Guid id)
    {
        return await _context.Articles
            .Include(a => a.Author)
            .FirstOrDefaultAsync(a => a.ArticleId == id);
    }

    public async Task AddAsync(DbArticle article)
    {
        await _context.Articles.AddAsync(article);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DbArticle article)
    {
        _context.Articles.Update(article);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var article = await GetByIdAsync(id);
        if (article != null)
        {
            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
        }
    }
}

