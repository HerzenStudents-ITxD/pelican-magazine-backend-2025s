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

    public async Task<DbArticleAgeCategory?> GetByIdAsync(Guid id)
    {
        return await _context.ArticleAgeCategories
            .Include(aac => aac.Article)
            .Include(aac => aac.AgeCategory)
            .FirstOrDefaultAsync(aac => aac.ArticleAgeId == id);
    }

    public async Task AddAsync(DbArticleAgeCategory articleAgeCategory)
    {
        // Проверяем, существует ли возрастная категория
        var categoryExists = await _context.AgeCategories
            .AnyAsync(c => c.AgeCategoryId == articleAgeCategory.AgeCategoryId);

        // Проверяем, существует ли статья
        var articleExists = await _context.Articles
            .AnyAsync(a => a.ArticleId == articleAgeCategory.ArticleId);

        if (!categoryExists || !articleExists)
        {
            throw new ArgumentException(
                categoryExists
                    ? "Article not found"
                    : "AgeCategory not found");
        }

        await _context.ArticleAgeCategories.AddAsync(articleAgeCategory);
        await _context.SaveChangesAsync(); 
    }

    public async Task UpdateAsync(DbArticleAgeCategory articleAgeCategory)
    {
        _context.ArticleAgeCategories.Update(articleAgeCategory);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var item = await GetByIdAsync(id);
        if (item != null)
        {
            _context.ArticleAgeCategories.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<DbArticleAgeCategory>> GetByArticleIdAsync(Guid articleId)
    {
        return await _context.ArticleAgeCategories
            .Where(aac => aac.ArticleId == articleId)
            .Include(aac => aac.AgeCategory)
            .ToListAsync();
    }
}

