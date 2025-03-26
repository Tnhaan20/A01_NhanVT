using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS1_BusinessModel;
using Microsoft.EntityFrameworkCore;

namespace AS1_DAO
{
    public class NewsArticleDAO
    {
        private FunewsManagementContext _dbcontext;
        private static NewsArticleDAO instance;

        public NewsArticleDAO()
        {
            _dbcontext = new FunewsManagementContext();
        }

        public static NewsArticleDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new NewsArticleDAO();

                return instance;
            }
        }

        public List<NewsArticle> GetNewsArticles()
        {
            return _dbcontext.NewsArticles
                .Include(a => a.Category)
                .Include(s => s.CreatedBy)
                .Include(t => t.Tags)
                .ToList();
        }

        public List<NewsArticle> getNewsByDateRange(DateTime startDate, DateTime endDate)
        {
            return _dbcontext.NewsArticles
                .Include(a => a.Category)
                .Include(a => a.CreatedBy)
                .Where(a => a.CreatedDate >= startDate && a.CreatedDate <= endDate)
                .OrderByDescending(a => a.CreatedDate)
                .ToList();
        }


        public NewsArticle GetNewsId(string id)
        {
            return _dbcontext.NewsArticles
                .Include(a => a.Tags)
                .Include(a => a.Category)
                .SingleOrDefault(a => a.NewsArticleId.Equals(id));
        }


        public void AddNews(NewsArticle news)
        {
            NewsArticle cur = GetNewsId(news.NewsArticleId);
            if (cur != null)
            {
                throw new Exception();
            }
            _dbcontext.NewsArticles.Add(news);
            _dbcontext.SaveChanges();
        }


        public void Update(NewsArticle news)
        {
            // Get the database context to track this entity
            using (var transaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    // Get the article with its tags
                    var cur = _dbcontext.NewsArticles
                        .Include(a => a.Tags)
                        .FirstOrDefault(a => a.NewsArticleId == news.NewsArticleId);

                    if (cur == null)
                    {
                        throw new Exception("NewsArticle not found");
                    }

                    // Update basic properties
                    cur.NewsTitle = news.NewsTitle;
                    cur.Headline = news.Headline;
                    cur.NewsContent = news.NewsContent;
                    cur.NewsSource = news.NewsSource;
                    cur.NewsStatus = news.NewsStatus;
                    cur.CategoryId = news.CategoryId;
                    cur.CreatedById = news.CreatedById;
                    cur.UpdatedById = news.UpdatedById;
                    cur.ModifiedDate = news.ModifiedDate;

                    // First, clear all existing tags from the article
                    cur.Tags.Clear();
                    _dbcontext.SaveChanges();

                    // Now add the new tags
                    if (news.Tags != null && news.Tags.Any())
                    {
                        foreach (var tagId in news.Tags.Select(t => t.TagId).Distinct())
                        {
                            // Find the tag in the database by ID
                            var dbTag = _dbcontext.Tags.Find(tagId);
                            if (dbTag != null)
                            {
                                // Add it to the article's tags
                                cur.Tags.Add(dbTag);
                            }
                        }
                    }

                    // Save changes with explicit transaction
                    _dbcontext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception($"Error updating article: {ex.Message}", ex);
                }
            }
        }

        public void Delete(string id)
        {
            NewsArticle cur = GetNewsId(id);
            if (cur != null)
            {
                _dbcontext.NewsArticles.Remove(cur);
                _dbcontext.SaveChanges(); // Delete the object
            }
        }

        public bool isInUsed(short cateId)
        {
            return _dbcontext.NewsArticles.Any(c => c.CategoryId == cateId);

        }

        public void UpdateArticleTags(string newsArticleId, List<int> tagIds)
        {
            // Find the article with its tags
            var article = _dbcontext.NewsArticles
                .Include(a => a.Tags)
                .FirstOrDefault(a => a.NewsArticleId == newsArticleId);

            if (article == null)
            {
                throw new Exception("Article not found");
            }

            // Store the original entity state
            var originalState = _dbcontext.Entry(article).State;

            try
            {
                // Clear existing tags
                article.Tags.Clear();

                // Add new tags if any
                if (tagIds != null && tagIds.Any())
                {
                    foreach (var tagId in tagIds)
                    {
                        // Get a clean reference to the tag
                        var tag = _dbcontext.Tags
                            .AsNoTracking()  // Important: Get without tracking
                            .FirstOrDefault(t => t.TagId == tagId);

                        if (tag != null)
                        {
                            // Create a new tracked instance
                            var trackedTag = _dbcontext.Tags.Find(tagId);
                            if (trackedTag != null)
                            {
                                article.Tags.Add(trackedTag);
                            }
                        }
                    }
                }

                // Save changes
                _dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                // Restore original state
                _dbcontext.Entry(article).State = originalState;
                throw new Exception($"Failed to update article tags: {ex.Message}", ex);
            }
        }
    }
}