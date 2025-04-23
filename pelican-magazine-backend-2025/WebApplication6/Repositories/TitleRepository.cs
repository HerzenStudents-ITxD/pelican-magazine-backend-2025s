using Backend.DataAccess;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class TitleRepository
{
    private readonly ApplicationDbContext _context;

    public TitleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DbTitle>> GetAllAsync()
    {
        return await _context.Titles
            .Include(t => t.User)
            .Include(t => t.Article)
            .ToListAsync();
    }

    public async Task<DbTitle?> GetByIdAsync(int id)
    {
        return await _context.Titles
            .Include(t => t.User)
            .Include(t => t.Article)
            .FirstOrDefaultAsync(t => t.TitleId == id);
    }

    public async Task AddAsync(DbTitle title)
    {
        await _context.Titles.AddAsync(title);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DbTitle title)
    {
        _context.Titles.Update(title);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var title = await GetByIdAsync(id);
        if (title != null)
        {
            _context.Titles.Remove(title);
            await _context.SaveChangesAsync();
        }
    }
}