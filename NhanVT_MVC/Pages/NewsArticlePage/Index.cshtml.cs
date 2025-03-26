using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AS1_BusinessModel;
using AS1_Repository;

namespace NhanVT_Assignment1.Pages.NewsArticlePage
{
    public class IndexModel : PageModel
    {
        private readonly INewsArticleRepository _context;
        private readonly IAccountRepository _accountContext;
        private readonly ITagRepo _tagRepo;

        public IndexModel(INewsArticleRepository context, IAccountRepository accountContext, ITagRepo tagRepo)
        {
            _context = context;
            _accountContext = accountContext;
            _tagRepo = tagRepo;
        }

        public IList<NewsArticle> NewsArticle { get; set; } = default!;
        public string Email { get; set; } = default!;
        public Dictionary<int, string> UpdatedByNames { get; set; } = new Dictionary<int, string>();
        public Dictionary<string, List<string>> ArticleTagNames { get; set; } = new Dictionary<string, List<string>>();
        
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public IActionResult OnGet()
        {
            var email = HttpContext.Session.GetString("Email");
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Login");
            }
            else
            {
                Email = email;
            }
            
            // Get all news articles
            var articles = _context.GetNewsArticles();
            
            // Apply search filter if search string is provided
            if (!string.IsNullOrEmpty(SearchString))
            {
                SearchString = SearchString.ToLower();
                articles = articles.Where(a => 
                    (a.NewsTitle != null && a.NewsTitle.ToLower().Contains(SearchString)) ||
                    (a.Headline != null && a.Headline.ToLower().Contains(SearchString)) ||
                    (a.Category != null && a.Category.CategoryName != null && 
                     a.Category.CategoryName.ToLower().Contains(SearchString)) ||
                    (a.CreatedBy != null && a.CreatedBy.AccountName != null && 
                     a.CreatedBy.AccountName.ToLower().Contains(SearchString))
                ).ToList();
            }
            
            NewsArticle = articles;

            // Get account names for updaters
            foreach (NewsArticle article in NewsArticle)
            {
                if (article.UpdatedById.HasValue)
                {
                    var account = _accountContext.GetAccountById(article.UpdatedById.Value);
                    if (account != null)
                    {
                        UpdatedByNames[article.UpdatedById.Value] = account.AccountName;
                    }
                    else
                    {
                        UpdatedByNames[article.UpdatedById.Value] = "Unknown";
                    }
                }

                var tagNames = new List<string>();
                if (article.Tags != null && article.Tags.Any())
                {
                    foreach (var tag in article.Tags)
                    {
                        var tagDetails = _tagRepo.GetTagsId(tag.TagId);
                        if (tagDetails != null)
                        {
                            tagNames.Add(tagDetails.TagName); // Add the tag name to the list
                        }
                    }
                    ArticleTagNames[article.NewsArticleId] = tagNames;
                }
            }
            
            return Page();
        }
    }
}