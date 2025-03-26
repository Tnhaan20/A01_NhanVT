using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AS1_BusinessModel;
using AS1_Repository;
using Microsoft.AspNetCore.Http;
using NuGet.Packaging;

namespace NhanVT_Assignment1.Pages.NewsArticlePage
{
    public class EditModel : PageModel
    {
        private readonly INewsArticleRepository _context;
        private readonly IAccountRepository _accountContext;
        private readonly ICategoriesRepo _categoryContext;
        private readonly ITagRepo _tagRepo;

        public EditModel(
            INewsArticleRepository context,
            ICategoriesRepo categoryContext,
            IAccountRepository accountContext,
            ITagRepo tagRepo)
        {
            _context = context;
            _categoryContext = categoryContext;
            _accountContext = accountContext;
            _tagRepo = tagRepo;
        }

        [BindProperty]
        public NewsArticle NewsArticle { get; set; } = default!;
        public string CreatorName { get; set; } = "Unknown";

        // For tag management
        public List<Tag> AllTags { get; set; } = new List<Tag>();

        [BindProperty]
        public List<int> SelectedTagIds { get; set; } = new List<int>();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsarticle = _context.getNewsById(id);
            if (newsarticle == null)
            {
                return NotFound();
            }

            NewsArticle = newsarticle;

            // Get creator name
            if (NewsArticle.CreatedById.HasValue)
            {
                var creator = _accountContext.GetAccountById(NewsArticle.CreatedById.Value);
                if (creator != null)
                {
                    CreatorName = creator.AccountName;
                }
            }

            // Get all available tags
            AllTags = _tagRepo.GetAllTag();

            // Get current tags of this article
            if (NewsArticle.Tags != null && NewsArticle.Tags.Any())
            {
                SelectedTagIds = NewsArticle.Tags.Select(t => t.TagId).ToList();
            }

            ViewData["CategoryId"] = new SelectList(_categoryContext.GetCategoryList(), "Value", "Text");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            LoadLookupData();

            try
            {
                var accountId = HttpContext.Session.GetInt32("AccountId");
                if (accountId.HasValue)
                {
                    NewsArticle.UpdatedById = (short)accountId.Value;
                    NewsArticle.ModifiedDate = DateTime.Now;
                }

                var existingArticle = _context.getNewsById(NewsArticle.NewsArticleId.ToString());
                if (existingArticle == null)
                {
                    return NotFound();
                }


                existingArticle.NewsTitle = NewsArticle.NewsTitle;
                existingArticle.Headline = NewsArticle.Headline;
                existingArticle.NewsContent = NewsArticle.NewsContent;
                existingArticle.NewsSource = NewsArticle.NewsSource;
                existingArticle.NewsStatus = NewsArticle.NewsStatus;
                existingArticle.CategoryId = NewsArticle.CategoryId;
                existingArticle.ModifiedDate = NewsArticle.ModifiedDate;
                existingArticle.UpdatedById = NewsArticle.UpdatedById;


                if (existingArticle.Tags == null)
                {
                    existingArticle.Tags = new List<Tag>();
                }
                else
                {
                    existingArticle.Tags.Clear();
                }

                // Thêm các tags mới vào collection hiện tại
                if (SelectedTagIds != null && SelectedTagIds.Any())
                {

                    foreach (var tagId in SelectedTagIds)
                    {
                        var tag = _tagRepo.GetTagsId(tagId);

                        if (tag != null)
                        {
                            existingArticle.Tags.Add(tag);
                        }
                        else
                        {
                            // Nếu _tagRepo.GetTagsId không trả về tag, thử tạo reference
                            var tagRef = new Tag { TagId = tagId };
                            existingArticle.Tags.Add(tagRef);
                        }
                    }
                }




                _context.UpdateNews(existingArticle);
                _context.UpdateNewsArticleTags(existingArticle.NewsArticleId, SelectedTagIds);

                var updatedArticle = _context.getNewsById(NewsArticle.NewsArticleId.ToString());

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                ModelState.AddModelError("", $"Unable to save changes: {ex.Message}");
                LoadLookupData();
                return Page();
            }
        }

        private void LoadLookupData()
        {
            ViewData["CategoryId"] = new SelectList(_categoryContext.GetCategoryList(), "Value", "Text");
            AllTags = _tagRepo.GetAllTag();

        }
    }
}