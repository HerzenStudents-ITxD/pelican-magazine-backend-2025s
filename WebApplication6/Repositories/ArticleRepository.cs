﻿using Backend.Contracts.Enums;
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
            .Include(a => a.ArticleAgeCategories)
                .ThenInclude(aac => aac.AgeCategory)
            .Include(a => a.ArticleThemes)
                .ThenInclude(at => at.Theme)
            .ToListAsync();
    }

    public async Task<DbArticle?> GetByIdAsync(Guid id)
    {
        return await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleAgeCategories)
                .ThenInclude(aac => aac.AgeCategory)
            .Include(a => a.ArticleThemes)
                .ThenInclude(at => at.Theme)
            .FirstOrDefaultAsync(a => a.ArticleId == id);
    }

    public async Task AddAsync(DbArticle article)
    {
        // Проверка обязательных полей
        if (article.AuthorId == Guid.Empty)
            throw new ArgumentException("AuthorId is required");

        // Проверка существования автора
        var authorExists = await _context.Users.AnyAsync(u => u.UserId == article.AuthorId);
        if (!authorExists)
            throw new ArgumentException("Author not found");

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

    public async Task<int> GetLikeCountAsync(Guid articleId)
    {
        return await _context.Likes.CountAsync(l => l.ArticleId == articleId);
    }

    public async Task<List<DbArticle>> GetByStatusAsync(ArticleStatus status)
    {
        return await _context.Articles
            .Where(a => a.Status == status)
            .Include(a => a.Author)
            .Include(a => a.ArticleAgeCategories)
                .ThenInclude(aac => aac.AgeCategory)
            .Include(a => a.ArticleThemes)
                .ThenInclude(at => at.Theme)
            .OrderByDescending(a => a.ChangedAt)
            .ToListAsync();
    }


    public async Task<List<DbArticle>> GetByAuthorIdAsync(Guid authorId)
    {
        return await _context.Articles
            .Where(a => a.AuthorId == authorId)
            .Include(a => a.Author)
            .Include(a => a.ArticleAgeCategories)
                .ThenInclude(aac => aac.AgeCategory)
            .Include(a => a.ArticleThemes)
                .ThenInclude(at => at.Theme)
            .OrderByDescending(a => a.ChangedAt)
            .ToListAsync();
    }

    public async Task<List<DbArticle>> GetPublishedByAuthorIdAsync(Guid authorId)
    {
        return await _context.Articles
            .Where(a => a.AuthorId == authorId && a.Status == ArticleStatus.Published)
            .Include(a => a.Author)
            .Include(a => a.ArticleAgeCategories)
                .ThenInclude(aac => aac.AgeCategory)
            .Include(a => a.ArticleThemes)
                .ThenInclude(at => at.Theme)
            .OrderByDescending(a => a.ChangedAt)
            .ToListAsync();
    }

    public async Task<List<DbArticle>> GetPendingModerationArticles()
    {
        return await _context.Articles
            .Where(a => a.Status == ArticleStatus.PendingModeration)
            .Include(a => a.Author)
            .OrderByDescending(a => a.ChangedAt)
            .ToListAsync();
    }


}