using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS1_BusinessModel;
using AS1_DAO;
using Microsoft.EntityFrameworkCore;

namespace AS1_Repository
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        public List<NewsArticle> GetNewsArticles()=>NewsArticleDAO.Instance.GetNewsArticles();

        public List<NewsArticle> GetNewsByDateRange(DateTime startDate, DateTime endDate) =>
            NewsArticleDAO.Instance.getNewsByDateRange(startDate, endDate);
        public void AddNews(NewsArticle newsArticle) => NewsArticleDAO.Instance.AddNews(newsArticle);



        public NewsArticle getNewsById(string newsId) => NewsArticleDAO.Instance.GetNewsId(newsId);

        public void RemoveNews(string newsId) => NewsArticleDAO.Instance.Delete(newsId);

        public void UpdateNews(NewsArticle news) => NewsArticleDAO.Instance.Update(news);

        public bool isInUsed(short cateId) => NewsArticleDAO.Instance.isInUsed(cateId);
    }
}
