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

        public IndexModel(INewsArticleRepository context)
        {
            _context = context;
        }

        public IList<NewsArticle> NewsArticle { get;set; } = default!;

        public string Email { get; set; } = default!;
       

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
            
            NewsArticle = _context.GetNewsArticles();
            return Page();
        }
    }
}
