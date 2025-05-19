using Backend.DataAccess;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class ThemeRepository
{
    private readonly ApplicationDbContext _context;

    public ThemeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DbTheme>> GetAllAsync()
    {
        return await _context.Themes.ToListAsync();
    }

    public async Task<DbTheme?> GetByIdAsync(Guid id)
    {
        return await _context.Themes.FindAsync(id);
    }

    public async Task AddAsync(DbTheme theme)
    {
        await _context.Themes.AddAsync(theme);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DbTheme theme)
    {
        _context.Themes.Update(theme);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var theme = await GetByIdAsync(id);
        if (theme != null)
        {
            _context.Themes.Remove(theme);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<DbAgeCategory?> GetByNameAsync(string name)
    {
        return await _context.AgeCategories
            .FirstOrDefaultAsync(a => a.CategoryName == name);
    }
}