using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AS1_BusinessModel;
using AS1_Repository;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NhanVT_Assignment1.Pages.NewsArticlePage
{
    public class DetailsModel : PageModel
{
    private readonly INewsArticleRepository _context;
    private readonly IAccountRepository _accountContext;
    private readonly ITagRepo _tagRepo;

    public DetailsModel(INewsArticleRepository context, IAccountRepository accountContext, ITagRepo tagRepo)
    {
        _context = context;
        _accountContext = accountContext;
        _tagRepo = tagRepo;
    }

    public NewsArticle NewsArticle { get; set; } = default!;
    public string CreatedByName { get; set; } = "Unknown";
    public string UpdatedByName { get; set; } = "Unknown";
    public List<string> TagNames { get; set; } = new List<string>();

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
        else
        {
            NewsArticle = newsarticle;
            
            // Get creator name
            if (NewsArticle.CreatedById.HasValue)
            {
                var creator = _accountContext.GetAccountById(NewsArticle.CreatedById.Value);
                if (creator != null)
                {
                    CreatedByName = creator.AccountName;

                }
            }
            
            // Get updater name
            if (NewsArticle.UpdatedById.HasValue)
            {
                var updater = _accountContext.GetAccountById(NewsArticle.UpdatedById.Value);
                if (updater != null)
                {
                    UpdatedByName = updater.AccountName;
                }
            }
            
            // Get tag names
            if (NewsArticle.Tags != null && NewsArticle.Tags.Any())
            {
                foreach (var tag in NewsArticle.Tags)
                {
                    var tagDetails = _tagRepo.GetTagsId(tag.TagId);
                    if (tagDetails != null)
                    {
                        TagNames.Add(tagDetails.TagName);
                    }
                }
            }
        }
        return Page();
    }
}
}
