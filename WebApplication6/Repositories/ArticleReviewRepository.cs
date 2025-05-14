using Backend.DataAccess;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class ArticleReviewRepository
{
    private readonly ApplicationDbContext _context;

    public ArticleReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DbArticleReview>> GetAllAsync()
    {
        return await _context.ArticleReviews
            .Include(ar => ar.Article)
            .Include(ar => ar.User)
            .ToListAsync();
    }

    public async Task<DbArticleReview?> GetByIdAsync(Guid id)
    {
        return await _context.ArticleReviews
            .Include(ar => ar.Article)
            .Include(ar => ar.User)
            .FirstOrDefaultAsync(ar => ar.ReviewId == id);
    }

    public async Task AddAsync(DbArticleReview articleReview)
    {
        articleReview.ReviewDate = DateTime.UtcNow;
        await _context.ArticleReviews.AddAsync(articleReview);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DbArticleReview articleReview)
    {
        _context.ArticleReviews.Update(articleReview);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var review = await GetByIdAsync(id);
        if (review != null)
        {
            _context.ArticleReviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<DbArticleReview>> GetByArticleIdAsync(Guid articleId)
    {
        return await _context.ArticleReviews
            .Where(ar => ar.ArticleId == articleId)
            .Include(ar => ar.User)
            .ToListAsync();
    }
}