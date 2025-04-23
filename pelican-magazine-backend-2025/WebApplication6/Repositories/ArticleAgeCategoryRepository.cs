using Backend.DataAccess;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class ArticleAgeCategoryRepository
{
    private readonly ApplicationDbContext _context;

    public ArticleAgeCategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DbArticleAgeCategory>> GetAllAsync()
    {
        return await _context.ArticleAgeCategories
            .Include(aac => aac.Article)
            .Include(aac => aac.AgeCategory)
            .ToListAsync();
    }

    public async Task<DbArticleAgeCategory?> GetByIdAsync(int id)
    {
        return await _context.ArticleAgeCategories
            .Include(aac => aac.Article)
            .Include(aac => aac.AgeCategory)
            .FirstOrDefaultAsync(aac => aac.ArticleAgeId == id);
    }

    public async Task AddAsync(DbArticleAgeCategory articleAgeCategory)
    {
        await _context.ArticleAgeCategories.AddAsync(articleAgeCategory);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DbArticleAgeCategory articleAgeCategory)
    {
        _context.ArticleAgeCategories.Update(articleAgeCategory);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var articleAgeCategory = await GetByIdAsync(id);
        if (articleAgeCategory != null)
        {
            _context.ArticleAgeCategories.Remove(articleAgeCategory);
            await _context.SaveChangesAsync();
        }
    }
}
