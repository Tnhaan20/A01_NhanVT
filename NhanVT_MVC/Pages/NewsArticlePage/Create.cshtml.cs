using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AS1_BusinessModel;
using AS1_Repository;
using Microsoft.AspNetCore.Http;

namespace NhanVT_Assignment1.Pages.NewsArticlePage
{
    public class CreateModel : PageModel
    {
        private readonly INewsArticleRepository _newsArticleRepository;
        private readonly ICategoriesRepo _categoryRepository;
        private readonly IConfiguration _configuration;
        private readonly ITagRepo _tagRepo;

        public CreateModel(
            INewsArticleRepository newsArticleRepository, 
            ICategoriesRepo categoryRepository, 
            IConfiguration configuration,
            ITagRepo tagRepo)
        {
            _newsArticleRepository = newsArticleRepository;
            _categoryRepository = categoryRepository;
            _configuration = configuration;
            _tagRepo = tagRepo;
        }

        [BindProperty]
        public NewsArticle NewsArticle { get; set; } = default!;

        [BindProperty]
        public List<int> SelectedTagIds { get; set; } = new List<int>();
        
        public List<Tag> AllTags { get; set; } = new List<Tag>();

        [TempData]
        public string SuccessMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

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

            // Load lookup data (categories and tags)
            LoadLookupData();

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
            var accountId = HttpContext.Session.GetInt32("AccountId");

            // Only Staff can create news
            if (role != staffRole)
            {
                return RedirectToPage("/Unauthorized");
            }

            try
            {
                
                    LoadLookupData();
                var allNews = _newsArticleRepository.GetNewsArticles();
                int nextId = 1;
                
                // Generate next ID
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
                string newId = nextId.ToString();
                NewsArticle.NewsArticleId = newId;
                
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
                    if (accountId.HasValue)
                    {
                        NewsArticle.CreatedById = (short)accountId.Value;
                    }
                }
                
                NewsArticle.UpdatedById = (short)accountId.Value;
                NewsArticle.NewsStatus = true;
                
                // Create a new empty Tags list if it doesn't exist
                if (NewsArticle.Tags == null)
                {
                    NewsArticle.Tags = new List<Tag>();
                }
                
                // Add the article first
                _newsArticleRepository.AddNews(NewsArticle);
                
                // Then update the tags separately
                if (SelectedTagIds != null && SelectedTagIds.Any())
                {
                    _newsArticleRepository.UpdateNewsArticleTags(newId, SelectedTagIds);
                }
                
                SuccessMessage = "Article successfully created with tags";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error: " + ex.Message;
                ModelState.AddModelError("", ErrorMessage);
                LoadLookupData();
                return Page();
            }
        }
        
        private void LoadLookupData()
        {
            // Get categories for dropdown
            ViewData["CategoryId"] = new SelectList(_categoryRepository.GetCategoryList(), "Value", "Text");
            
            // Get all available tags
            AllTags = _tagRepo.GetAllTag();
        }
    }
}