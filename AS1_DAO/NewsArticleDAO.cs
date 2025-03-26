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

        public static NewsArticleDAO Instance { get {
                if(instance == null)
                    instance = new NewsArticleDAO();
                
                return instance; } }

        public List<NewsArticle> GetNewsArticles() { 
            return _dbcontext.NewsArticles
                .Include(a => a.Category)
                .Include(s => s.CreatedBy)
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
            NewsArticle cur = GetNewsId(news.NewsArticleId);
            if (cur == null)
            {
                throw new Exception();
            }
            _dbcontext.Entry(cur).CurrentValues.SetValues(news);
            _dbcontext.SaveChanges();
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

    }
}
