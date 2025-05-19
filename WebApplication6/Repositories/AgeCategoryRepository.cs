using Backend.DataAccess;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class AgeCategoryRepository
{
    private readonly ApplicationDbContext _context;

    public AgeCategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DbAgeCategory>> GetAllAsync()
    {
        return await _context.AgeCategories.ToListAsync();
    }

    public async Task<DbAgeCategory?> GetByIdAsync(Guid id) // <- Измените на Guid
    {
        return await _context.AgeCategories.FindAsync(id);
    }

    public async Task AddAsync(DbAgeCategory ageCategory)
    {
        await _context.AgeCategories.AddAsync(ageCategory);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DbAgeCategory ageCategory)
    {
        _context.AgeCategories.Update(ageCategory);
        await _context.SaveChangesAsync();
    }

    public async Task AddAsync(DbArticleAgeCategory articleAgeCategory)
    {
        if (articleAgeCategory == null)
            throw new ArgumentNullException(nameof(articleAgeCategory));

        await _context.ArticleAgeCategories.AddAsync(articleAgeCategory);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var ageCategory = await GetByIdAsync(id);
        if (ageCategory != null)
        {
            _context.AgeCategories.Remove(ageCategory);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<DbTheme?> GetByNameAsync(string name)
    {
        return await _context.Themes
            .FirstOrDefaultAsync(t => t.ThemeName == name);
    }


    
}