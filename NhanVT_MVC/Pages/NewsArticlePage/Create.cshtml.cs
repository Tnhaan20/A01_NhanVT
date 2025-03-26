using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AS1_BusinessModel;
using AS1_Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using NhanVT_MVC.Pages.Hubs;

namespace NhanVT_Assignment1.Pages.NewsArticlePage
{
    public class CreateModel : PageModel
    {
        private readonly INewsArticleRepository _newsArticleRepository;
        private readonly ICategoriesRepo _categoryRepository;
        private readonly IConfiguration _configuration;
        private readonly ITagRepo _tagRepo;
        private readonly IHubContext<FunewsHub> _hubContext;

        public CreateModel(
            INewsArticleRepository newsArticleRepository,
            ICategoriesRepo categoryRepository,
            IConfiguration configuration,
            ITagRepo tagRepo,
            IHubContext<FunewsHub> hubContext)
        {
            _newsArticleRepository = newsArticleRepository;
            _categoryRepository = categoryRepository;
            _configuration = configuration;
            _tagRepo = tagRepo;
            _hubContext = hubContext;
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
            try
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
            catch (Exception ex)
            {
                ErrorMessage = "Error loading page: " + ex.Message;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
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

               

                    LoadLookupData();
                // Set up the new article
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
                if (!NewsArticle.CreatedById.HasValue || NewsArticle.CreatedById == 0)
                {
                    if (accountId.HasValue)
                    {
                        NewsArticle.CreatedById = (short)accountId.Value;
                    }
                }

                if (accountId.HasValue)
                {
                    NewsArticle.UpdatedById = (short)accountId.Value;
                }

                NewsArticle.NewsStatus = true;

                NewsArticle.Tags = new List<Tag>();

                _newsArticleRepository.AddNews(NewsArticle);

                await _hubContext.Clients.All.SendAsync("Change");

                // Then update tags in a separate operation
                if (SelectedTagIds != null && SelectedTagIds.Any())
                {
                    // Fixed the concurrency issue by using a separate operation
                    _newsArticleRepository.UpdateNewsArticleTags(newId, SelectedTagIds);

                    // IMPORTANT: Wait for SignalR to complete again
                    await _hubContext.Clients.All.SendAsync("Change");
                }

                SuccessMessage = "Article successfully created";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error creating article: " + ex.Message;
                ModelState.AddModelError("", ErrorMessage);
                LoadLookupData();
                return Page();
            }
        }

        private void LoadLookupData()
        {
            // Load categories for dropdown
            ViewData["CategoryId"] = new SelectList(_categoryRepository.GetCategoryList(), "Value", "Text");

            // Load all available tags
            AllTags = _tagRepo.GetAllTag();
        }
    }
}