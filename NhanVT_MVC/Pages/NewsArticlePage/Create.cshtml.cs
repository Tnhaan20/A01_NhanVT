using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AS1_BusinessModel;
using AS1_Repository;

namespace NhanVT_Assignment1.Pages.NewsArticlePage
{
    public class CreateModel : PageModel
    {
        
            private readonly INewsArticleRepository _newsArticleRepository;
            private readonly ICategoriesRepo _categoryRepository;
            private readonly IConfiguration _configuration;

            public CreateModel(INewsArticleRepository newsArticleRepository, ICategoriesRepo categoryRepository, IConfiguration configuration)
            {
                _newsArticleRepository = newsArticleRepository;
                _categoryRepository = categoryRepository;
                _configuration = configuration;
            }

            public IActionResult OnGet()
            {
                // Check login
                var email = HttpContext.Session.GetString("Email");
                if (string.IsNullOrEmpty(email))
                {
                    return RedirectToPage("/Login");
                }

                // Check role
                var role = HttpContext.Session.GetInt32("RoleId");
                var staffRole = 1;

                // Only Staff can create news
                if (role != staffRole)
                {
                    return RedirectToPage("/Unauthorized");
                }

                // Get categories for dropdown
                ViewData["CategoryId"] = new SelectList(_categoryRepository.getAllCategories(), "CategoryId", "CategoryName");

                // Get current user ID from session
                var accountId = HttpContext.Session.GetInt32("AccountId");
                if (accountId.HasValue)
                {
                    // Set default values
                    NewsArticle = new NewsArticle
                    {
                        CreatedDate = DateTime.Now,
                        CreatedById = (short)accountId.Value,
                        NewsStatus = true 
                    };
                }

                return Page();
            }

            [BindProperty]
            public NewsArticle NewsArticle { get; set; } = default!;

            [TempData]
            public string SuccessMessage { get; set; }

            [TempData]
            public string ErrorMessage { get; set; }

            public IActionResult OnPost()
{
    // Check login
    var email = HttpContext.Session.GetString("Email");
    if (string.IsNullOrEmpty(email))
    {
        return RedirectToPage("/Login");
    }

    // Check role
    var role = HttpContext.Session.GetInt32("RoleId");
    var staffRole = 1;

    // Only Staff can create news
    if (role != staffRole)
    {
        return RedirectToPage("/Unauthorized");
    }


    try
    {

        if (!ModelState.IsValid)
        {
            ViewData["CategoryId"] = new SelectList(_categoryRepository.GetList(), "Value", "Text");
        }
        var allNews = _newsArticleRepository.GetNewsArticles();
        int nextId = 1;
        
        if (allNews != null && allNews.Any())
        {
            var numericIds = allNews
                .Where(n => n.NewsArticleId != null)
                .Where(n => int.TryParse(n.NewsArticleId, out _))
                .Select(n => int.Parse(n.NewsArticleId));
            
            if (numericIds.Any())
            {
                nextId = numericIds.Max() + 1;
            }
        }
        
        // Set the NewsArticleId
        NewsArticle.NewsArticleId = nextId.ToString();
        
        // Set or update dates
        DateTime now = DateTime.Now;
        
        if (!NewsArticle.CreatedDate.HasValue || NewsArticle.CreatedDate == DateTime.MinValue)
        {
            NewsArticle.CreatedDate = now;
        }
        
        if (!NewsArticle.ModifiedDate.HasValue || NewsArticle.ModifiedDate == DateTime.MinValue)
        {
            NewsArticle.ModifiedDate = now;
        }
        
        // Set the CreatedById if not already set
        if (NewsArticle.CreatedById == 0)
        {
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (accountId.HasValue)
            {
                NewsArticle.CreatedById = (short)accountId.Value;
                NewsArticle.UpdatedById = (short)accountId.Value;
            }
        }
        
        NewsArticle.NewsStatus = true;
        
        _newsArticleRepository.AddNews(NewsArticle);
        
        SuccessMessage = "Success added";
        return RedirectToPage("./Index");
            }
    catch (Exception ex)
    {
        ErrorMessage = "Error " + ex.Message;
        ModelState.AddModelError("", ErrorMessage);
        ViewData["CategoryId"] = new SelectList(_categoryRepository.getAllCategories(), "CategoryId", "CategoryDesciption");
        return Page();
    }
}
        
    }
    
    }