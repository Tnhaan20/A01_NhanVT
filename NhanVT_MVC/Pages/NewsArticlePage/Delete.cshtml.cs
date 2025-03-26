using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AS1_BusinessModel;
using AS1_Repository;
using Microsoft.AspNetCore.SignalR;
using NhanVT_MVC.Pages.Hubs;

namespace NhanVT_Assignment1.Pages.NewsArticlePage
{
    public class DeleteModel : PageModel
    {
        private readonly INewsArticleRepository _context;
        private readonly IHubContext<FunewsHub> _hubContext;

        public DeleteModel(INewsArticleRepository context, IHubContext<FunewsHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [BindProperty]
        public NewsArticle NewsArticle { get; set; } = default!;

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
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsarticle = _context.getNewsById(id);
            if (newsarticle != null)
            {
                NewsArticle = newsarticle;
                _context.RemoveNews(id);
                await _hubContext.Clients.All.SendAsync("Change");

            }

            return RedirectToPage("./Index");
        }
    }
}
